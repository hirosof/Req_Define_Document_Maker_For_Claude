// 要求仕様書とプロンプトを生成するテンプレート関数

function generateSpecification(formData) {
    const today = new Date().toISOString().split('T')[0];

    let spec = `# 要求仕様書

**作成日:** ${today}

---

## 1. 基本情報

### 1.1 プロジェクト名
${formData.projectName || '（未記入）'}

### 1.2 概要/目的
${formData.overview || '（未記入）'}

### 1.3 対象ユーザー
${formData.targetUser || '（未記入）'}

---

## 2. 技術要件

### 2.1 アプリケーション種類
${formData.appType || '（未記入）'}

### 2.2 使用言語
${formData.language || '（未記入）'}

### 2.3 フレームワーク/ライブラリ
${formData.framework || '（未記入）'}

### 2.4 開発環境
${formData.devEnvironment || '（未記入）'}

### 2.5 本番環境
${formData.prodEnvironment || '（未記入）'}

### 2.6 制約事項
${formData.constraints || '（未記入）'}

---

## 3. 機能要件

### 3.1 主要機能
${formData.mainFeatures || '（未記入）'}

### 3.2 画面/UI要件
${formData.uiRequirements || '（未記入）'}

### 3.3 データ要件
${formData.dataRequirements || '（未記入）'}

---

## 4. その他

### 4.1 参考資料
${formData.references || '（未記入）'}

---

## 5. 議論が必要な項目

${generateDiscussionItems(formData)}

---

## 備考

`;

    return spec;
}

function generateDiscussionItems(formData) {
    const discussionItems = [];
    const fieldNames = {
        projectName: 'プロジェクト名',
        overview: '概要/目的',
        targetUser: '対象ユーザー',
        appType: 'アプリケーション種類',
        language: '使用言語',
        framework: 'フレームワーク/ライブラリ',
        devEnvironment: '開発環境',
        prodEnvironment: '本番環境',
        constraints: '制約事項',
        mainFeatures: '主要機能',
        uiRequirements: '画面/UI要件',
        dataRequirements: 'データ要件',
        references: '参考資料'
    };

    for (const [key, value] of Object.entries(formData)) {
        if (value === '別途議論') {
            discussionItems.push(`- **${fieldNames[key]}**: 詳細について議論が必要`);
        } else if (value === 'お任せ') {
            discussionItems.push(`- **${fieldNames[key]}**: Claude に最適な提案を依頼`);
        }
    }

    if (discussionItems.length === 0) {
        return '特になし';
    }

    return discussionItems.join('\n');
}

function generatePrompt(formData) {
    const hasDiscussion = Object.values(formData).some(v => v === '別途議論' || v === 'お任せ');

    let prompt = `以下の要求仕様書に基づいて、プロジェクトの実装について議論したいと思います。

`;

    // 「お任せ」や「別途議論」がある場合の指示
    if (hasDiscussion) {
        prompt += `まず、以下の点について確認・提案をお願いします：

`;

        const fieldNames = {
            projectName: 'プロジェクト名',
            overview: '概要/目的',
            targetUser: '対象ユーザー',
            appType: 'アプリケーション種類',
            language: '使用言語',
            framework: 'フレームワーク/ライブラリ',
            devEnvironment: '開発環境',
            prodEnvironment: '本番環境',
            constraints: '制約事項',
            mainFeatures: '主要機能',
            uiRequirements: '画面/UI要件',
            dataRequirements: 'データ要件',
            references: '参考資料'
        };

        for (const [key, value] of Object.entries(formData)) {
            if (value === '別途議論') {
                prompt += `- **${fieldNames[key]}**: 詳細について議論したい\n`;
            } else if (value === 'お任せ') {
                prompt += `- **${fieldNames[key]}**: 最適な提案をお願いします\n`;
            }
        }

        prompt += `\n議論の後、二次要求仕様書を作成してください。\n\n`;
    } else {
        prompt += `内容を確認の上、二次要求仕様書を作成してください。\n\n`;
    }

    prompt += `---

# 要求仕様書

## 1. 基本情報

**プロジェクト名:** ${formData.projectName || '（未記入）'}

**概要/目的:**
${formData.overview || '（未記入）'}

**対象ユーザー:** ${formData.targetUser || '（未記入）'}

## 2. 技術要件

**アプリケーション種類:** ${formData.appType || '（未記入）'}

**使用言語:** ${formData.language || '（未記入）'}

**フレームワーク/ライブラリ:** ${formData.framework || '（未記入）'}

**開発環境:**
${formData.devEnvironment || '（未記入）'}

**本番環境:**
${formData.prodEnvironment || '（未記入）'}

**制約事項:**
${formData.constraints || '（未記入）'}

## 3. 機能要件

**主要機能:**
${formData.mainFeatures || '（未記入）'}

**画面/UI要件:**
${formData.uiRequirements || '（未記入）'}

**データ要件:**
${formData.dataRequirements || '（未記入）'}

## 4. その他

**参考資料:**
${formData.references || '（未記入）'}
`;

    return prompt;
}
