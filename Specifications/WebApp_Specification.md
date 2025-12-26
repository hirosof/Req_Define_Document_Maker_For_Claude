# Webアプリ版 仕様書

## 1. 概要

### 1.1 プロジェクト名
要求仕様書作成ツール for Claude（Webアプリ版）

### 1.2 目的
Claudeに依頼するための要求仕様書を効率的に作成し、プロジェクト要件を体系的に整理するWebアプリケーション。

### 1.3 対象ユーザー
- ソフトウェア開発者
- プロジェクトマネージャー
- Claudeを利用して開発を進めたい個人・チーム

---

## 2. 技術スタック

### 2.1 言語
- HTML5
- CSS3
- JavaScript (ES6+, Vanilla)

### 2.2 フレームワーク/ライブラリ
- なし（Pure HTML/CSS/JS）

### 2.3 動作環境
- モダンブラウザ（Chrome, Edge, Firefox, Safari）
- ローカルファイルとして実行可能（サーバー不要）

---

## 3. ファイル構成

```
WebApp/
├── index.html      # メインHTML（フォーム画面）
├── style.css       # スタイルシート
├── script.js       # メインロジック（イベント処理、UI制御）
└── template.js     # テンプレート生成（要求仕様書、プロンプト）
```

---

## 4. 機能仕様

### 4.1 入力フォーム

#### 4.1.1 基本情報
| 項目名 | 入力形式 | オプション |
|--------|---------|-----------|
| プロジェクト名 | テキスト入力 | お任せ / 別途議論 |
| 概要/目的 | テキストエリア | お任せ / 別途議論 |
| 対象ユーザー | テキスト入力 | お任せ / 別途議論 |

#### 4.1.2 技術要件
| 項目名 | 入力形式 | オプション |
|--------|---------|-----------|
| アプリケーション種類 | セレクト（CLI/デスクトップ/Web/その他） | お任せ / 別途議論 |
| 使用言語 | テキスト入力 | お任せ / 別途議論 |
| フレームワーク/ライブラリ | テキスト入力 | お任せ / 別途議論 |
| 開発環境 | テキスト入力 | お任せ / 別途議論 |
| 本番環境 | セレクト（開発環境と同じ/カスタム） | お任せ / 別途議論 |
| 制約事項 | テキストエリア | お任せ / 別途議論 |

#### 4.1.3 機能要件
| 項目名 | 入力形式 | オプション |
|--------|---------|-----------|
| 主要機能 | テキストエリア | お任せ / 別途議論 |
| 画面/UI要件 | テキストエリア | お任せ / 別途議論 |
| データ要件 | テキストエリア | お任せ / 別途議論 |

#### 4.1.4 その他
| 項目名 | 入力形式 | オプション |
|--------|---------|-----------|
| 参考資料 | テキストエリア | お任せ / 別途議論 |

### 4.2 オプションボタン機能

#### 4.2.1 動作仕様
- 「お任せ」ボタンをクリック
  - 入力欄を無効化（グレーアウト）
  - ボタンがアクティブ状態（紫色）に変化
  - 内部状態に「お任せ」を記録

- 「別途議論」ボタンをクリック
  - 入力欄を無効化（グレーアウト）
  - ボタンがアクティブ状態（紫色）に変化
  - 内部状態に「別途議論」を記録

- 再度同じボタンをクリック
  - 選択を解除
  - 入力欄を有効化
  - ボタンを非アクティブ状態に戻す

#### 4.2.2 実装（JavaScript）
```javascript
handleOptionClick(event) {
    const button = event.target;
    const field = button.dataset.field;
    const value = button.dataset.value;
    const input = document.getElementById(field);

    // トグル処理
    if (button.classList.contains('active')) {
        button.classList.remove('active');
        input.disabled = false;
        input.classList.remove('option-selected');
        delete _fieldStates[field];
    } else {
        // 同じフィールドの他のボタンを非アクティブ化
        const siblings = button.parentElement.querySelectorAll('.option-btn');
        siblings.forEach(btn => btn.classList.remove('active'));

        button.classList.add('active');
        input.disabled = true;
        input.classList.add('option-selected');
        _fieldStates[field] = value;
    }
}
```

### 4.3 出力機能

#### 4.3.1 一次要求仕様書（Markdown）
- 入力内容をMarkdown形式で出力
- オプション選択された項目は「お任せ」「別途議論を希望」と表記
- 日付を自動挿入
- プレビュー表示（`<pre>`タグ内）

#### 4.3.2 Claude向けプロンプト
- 要求仕様書の内容を含む
- Claudeへの指示を含むプロンプト形式
- プレビュー表示（`<pre>`タグ内）

### 4.4 エクスポート機能

#### 4.4.1 クリップボードコピー
- ブラウザのClipboard APIを使用
- コピー成功時にアラート表示
- エラー時のハンドリング

```javascript
function copyToClipboard(text, type) {
    navigator.clipboard.writeText(text).then(() => {
        alert(`${typeName}をクリップボードにコピーしました。`);
    }).catch(err => {
        console.error('コピーに失敗しました:', err);
        alert('コピーに失敗しました。');
    });
}
```

#### 4.4.2 ファイル保存
- Blob APIを使用してMarkdownファイルを生成
- ファイル名: `要求仕様書.md` / `Claude向けプロンプト.txt`
- ダウンロードダイアログを表示

```javascript
function downloadFile(content, filename) {
    const blob = new Blob([content], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}
```

### 4.5 クリアボタン
- 全入力欄をクリア
- 全オプションボタンを非アクティブ化
- 確認ダイアログを表示
- 出力エリアを非表示
- ページトップにスクロール

### 4.6 ダークモード

#### 4.6.1 機能概要
- ヘッダー右上に切り替えボタン（🌙/☀️）を配置
- ボタンクリックでライト⇔ダーク切り替え
- アイコンが動的に変化（🌙→☀️→🌙）
- 設定をローカルストレージに保存

#### 4.6.2 実装方式
- CSS変数を使用した動的テーマ切り替え
- `body.dark-mode`クラスで色を上書き

**CSS変数定義:**
```css
:root {
    /* ライトモード */
    --bg-primary: #f5f5f5;
    --bg-secondary: #ffffff;
    --text-primary: #333;
    --text-secondary: #555;
    --border-color: #ddd;
    --input-disabled-bg: #f0f0f0;
    --input-disabled-border: #999;
    --shadow-color: rgba(0, 0, 0, 0.1);
    --button-hover-bg: rgba(0, 0, 0, 0.05);
}

body.dark-mode {
    /* ダークモード */
    --bg-primary: #1e1e1e;
    --bg-secondary: #2d2d2d;
    --text-primary: #e0e0e0;
    --text-secondary: #b0b0b0;
    --border-color: #444;
    --input-disabled-bg: #3a3a3a;
    --input-disabled-border: #555;
    --shadow-color: rgba(0, 0, 0, 0.3);
    --button-hover-bg: rgba(255, 255, 255, 0.1);
}
```

**JavaScript実装:**
```javascript
// テーマの初期化
function initializeTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark-mode');
        updateThemeIcon(true);
    }
}

// テーマの切り替え
function toggleTheme() {
    const isDarkMode = document.body.classList.toggle('dark-mode');
    localStorage.setItem('theme', isDarkMode ? 'dark' : 'light');
    updateThemeIcon(isDarkMode);
}

// アイコンの更新
function updateThemeIcon(isDarkMode) {
    const themeIcon = document.querySelector('.theme-icon');
    themeIcon.textContent = isDarkMode ? '☀️' : '🌙';
}
```

#### 4.6.3 対応コンポーネント
- ヘッダー
- フォームセクション背景
- 入力欄（TextBox, TextArea, Select）
- オプションボタン
- アクションボタン
- 出力エリア
- スクロールバー

---

## 5. UI/UXデザイン

### 5.1 レイアウト
- 中央寄せデザイン（max-width: 1000px）
- レスポンシブ対応（モバイル表示最適化）
- カードベースのセクション分割

### 5.2 カラーパレット

#### 5.2.1 ライトモード
- プライマリカラー: #667eea（紫）
- 背景色（プライマリ）: #f5f5f5
- 背景色（セカンダリ）: #ffffff
- テキスト色（プライマリ）: #333333
- テキスト色（セカンダリ）: #555555
- ボーダー色: #dddddd

#### 5.2.2 ダークモード
- プライマリカラー: #667eea（紫）※変更なし
- 背景色（プライマリ）: #1e1e1e
- 背景色（セカンダリ）: #2d2d2d
- テキスト色（プライマリ）: #e0e0e0
- テキスト色（セカンダリ）: #b0b0b0
- ボーダー色: #444444

### 5.3 タイポグラフィ
- フォントファミリー: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif
- 出力エリア（コード）: 'Consolas', 'Monaco', monospace

### 5.4 アニメーション
- ホバー時のボタン拡大（transform: scale(1.05)）
- テーマ切り替え時のスムーズトランジション（0.3s）
- スクロール時のスムーズスクロール（smooth scroll）

---

## 6. データフロー

```
[ユーザー入力]
    ↓
[FormDataオブジェクト生成]
    ↓
[template.js: generateSpecification()]
    ↓
[Markdown形式の要求仕様書]
    ↓
[画面表示 / クリップボード / ファイル保存]
```

---

## 7. エラーハンドリング

### 7.1 入力バリデーション
- 全項目が空の場合: 「少なくとも1つの項目を入力してください。」とアラート表示

### 7.2 クリップボード操作
- コピー失敗時: 「コピーに失敗しました。」とアラート表示
- console.errorでエラーログ出力

### 7.3 ファイル保存
- ブラウザのダウンロード機能に依存
- エラー時は自動的にブラウザの標準エラーハンドリング

---

## 8. ブラウザ互換性

### 8.1 サポート対象
- Chrome 90+
- Edge 90+
- Firefox 88+
- Safari 14+

### 8.2 必要なAPI
- Clipboard API（クリップボードコピー）
- Blob API（ファイル保存）
- localStorage（ダークモード設定保存）

---

## 9. パフォーマンス

### 9.1 最適化
- Pure JavaScriptによる軽量実装
- 外部依存なし（高速読み込み）
- CSSアニメーションの適切な使用

### 9.2 ファイルサイズ
- index.html: 約15KB
- style.css: 約8KB
- script.js: 約7KB
- template.js: 約5KB
- **合計: 約35KB**（非常に軽量）

---

## 10. セキュリティ

### 10.1 対策
- すべてクライアントサイドで完結（サーバー不要）
- ユーザーデータはローカルにのみ保存（プライバシー保護）
- XSS対策: textContentを使用（innerHTML使用なし）

### 10.2 制限事項
- HTTPSでない環境ではClipboard APIが動作しない場合あり

---

## 11. 今後の拡張可能性

- 過去の仕様書の保存・読み込み機能（localStorage利用）
- テンプレートのカスタマイズ機能
- プロジェクトタイプ別のプリセット
- エクスポート形式の追加（JSON, YAML等）
- 多言語対応（i18n）

---

## 12. 変更履歴

| 日付 | バージョン | 変更内容 |
|------|-----------|---------|
| 2025-12-27 | 1.0.0 | 初版作成 |
| 2025-12-27 | 1.1.0 | ダークモード機能追加 |
