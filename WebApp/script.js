// グローバル変数
let generatedSpec = '';
let generatedPrompt = '';

// DOM読み込み完了時の初期化
document.addEventListener('DOMContentLoaded', function() {
    initializeEventListeners();
    initializeTheme();
});

// イベントリスナーの初期化
function initializeEventListeners() {
    // オプションボタンのイベント
    const optionButtons = document.querySelectorAll('.option-btn');
    optionButtons.forEach(btn => {
        btn.addEventListener('click', handleOptionClick);
    });

    // アプリケーション種類の変更イベント
    const appTypeSelect = document.getElementById('appType');
    if (appTypeSelect) {
        appTypeSelect.addEventListener('change', handleAppTypeChange);
    }

    // 本番環境の変更イベント
    const prodEnvironmentSelect = document.getElementById('prodEnvironmentSelect');
    if (prodEnvironmentSelect) {
        prodEnvironmentSelect.addEventListener('change', handleProdEnvironmentChange);
    }

    // 生成ボタン
    document.getElementById('generateBtn').addEventListener('click', generateDocuments);

    // クリアボタン
    document.getElementById('clearBtn').addEventListener('click', clearForm);

    // コピーボタン
    document.getElementById('copySpecBtn').addEventListener('click', () => copyToClipboard(generatedSpec, 'spec'));
    document.getElementById('copyPromptBtn').addEventListener('click', () => copyToClipboard(generatedPrompt, 'prompt'));

    // ダウンロードボタン
    document.getElementById('downloadSpecBtn').addEventListener('click', () => downloadFile(generatedSpec, '要求仕様書.md'));
    document.getElementById('downloadPromptBtn').addEventListener('click', () => downloadFile(generatedPrompt, 'Claude向けプロンプト.txt'));

    // テーマ切り替えボタン
    document.getElementById('themeToggle').addEventListener('click', toggleTheme);
}

// アプリケーション種類の変更処理
function handleAppTypeChange(e) {
    const appTypeOther = document.getElementById('appTypeOther');
    if (e.target.value === 'Other') {
        appTypeOther.style.display = 'block';
    } else {
        appTypeOther.style.display = 'none';
        appTypeOther.value = '';
    }
}

// 本番環境の変更処理
function handleProdEnvironmentChange(e) {
    const prodEnvironment = document.getElementById('prodEnvironment');
    if (e.target.value === 'Custom') {
        prodEnvironment.style.display = 'block';
    } else {
        prodEnvironment.style.display = 'none';
        prodEnvironment.value = '';
    }
}

// オプションボタンのクリック処理
function handleOptionClick(e) {
    const button = e.target;
    const fieldName = button.dataset.field;
    const value = button.dataset.value;
    const field = document.getElementById(fieldName);
    const optionButtons = button.parentElement.querySelectorAll('.option-btn');

    // 同じボタンをクリックした場合は解除
    if (button.classList.contains('active')) {
        button.classList.remove('active');
        field.value = '';
        field.classList.remove('option-selected');
        field.disabled = false;

        // appTypeの場合は「その他」入力欄も非表示に
        if (fieldName === 'appType') {
            const appTypeOther = document.getElementById('appTypeOther');
            appTypeOther.style.display = 'none';
            appTypeOther.value = '';
        }
        // prodEnvironmentの場合は入力欄も非表示に
        if (fieldName === 'prodEnvironment') {
            const prodEnvironmentSelect = document.getElementById('prodEnvironmentSelect');
            const prodEnvironmentTextarea = document.getElementById('prodEnvironment');
            prodEnvironmentSelect.value = '';
            prodEnvironmentTextarea.style.display = 'none';
            prodEnvironmentTextarea.value = '';
        }
        return;
    }

    // 他のボタンを非アクティブに
    optionButtons.forEach(btn => btn.classList.remove('active'));

    // クリックしたボタンをアクティブに
    button.classList.add('active');
    field.value = value;
    field.classList.add('option-selected');
    field.disabled = true;

    // appTypeの場合は「その他」入力欄を非表示に
    if (fieldName === 'appType') {
        const appTypeOther = document.getElementById('appTypeOther');
        appTypeOther.style.display = 'none';
        appTypeOther.value = '';
    }
    // prodEnvironmentの場合は入力欄を非表示に
    if (fieldName === 'prodEnvironment') {
        const prodEnvironmentSelect = document.getElementById('prodEnvironmentSelect');
        const prodEnvironmentTextarea = document.getElementById('prodEnvironment');
        prodEnvironmentSelect.value = '';
        prodEnvironmentTextarea.style.display = 'none';
        prodEnvironmentTextarea.value = '';
    }
}

// フォームデータの取得
function getFormData() {
    const formData = {};
    const fields = [
        'projectName',
        'overview',
        'targetUser',
        'mainFeatures',
        'uiRequirements',
        'dataRequirements',
        'language',
        'framework',
        'devEnvironment',
        'constraints',
        'references'
    ];

    fields.forEach(field => {
        const element = document.getElementById(field);
        formData[field] = element.value.trim();
    });

    // アプリケーション種類の処理
    const appType = document.getElementById('appType');
    const appTypeOther = document.getElementById('appTypeOther');

    // オプションボタンで「お任せ」「別途議論」が選択されているかチェック
    const appTypeButtons = document.querySelectorAll('.option-btn[data-field="appType"]');
    let appTypeOptionSelected = false;
    appTypeButtons.forEach(btn => {
        if (btn.classList.contains('active')) {
            formData.appType = btn.dataset.value;
            appTypeOptionSelected = true;
        }
    });

    // オプションボタンが選択されていない場合はselectの値を使う
    if (!appTypeOptionSelected) {
        if (appType.value === 'Other' && appTypeOther.value.trim()) {
            formData.appType = appTypeOther.value.trim();
        } else if (appType.value === 'CLI') {
            formData.appType = 'CLIツール（コンソールアプリケーション）';
        } else if (appType.value === 'Desktop') {
            formData.appType = 'デスクトップアプリケーション';
        } else if (appType.value === 'Web') {
            formData.appType = 'Webアプリケーション';
        } else {
            formData.appType = appType.value;
        }
    }

    // 本番環境の処理
    const prodEnvironmentSelect = document.getElementById('prodEnvironmentSelect');
    const prodEnvironmentTextarea = document.getElementById('prodEnvironment');

    // オプションボタンで「お任せ」「別途議論」が選択されているかチェック
    const prodEnvironmentButtons = document.querySelectorAll('.option-btn[data-field="prodEnvironment"]');
    let prodEnvironmentOptionSelected = false;
    prodEnvironmentButtons.forEach(btn => {
        if (btn.classList.contains('active')) {
            formData.prodEnvironment = btn.dataset.value;
            prodEnvironmentOptionSelected = true;
        }
    });

    // オプションボタンが選択されていない場合はselectの値を使う
    if (!prodEnvironmentOptionSelected) {
        if (prodEnvironmentSelect.value === 'SameAsDev') {
            formData.prodEnvironment = '開発環境と同じ';
        } else if (prodEnvironmentSelect.value === 'Custom' && prodEnvironmentTextarea.value.trim()) {
            formData.prodEnvironment = prodEnvironmentTextarea.value.trim();
        } else {
            formData.prodEnvironment = prodEnvironmentSelect.value;
        }
    }

    return formData;
}

// ドキュメント生成
function generateDocuments() {
    const formData = getFormData();

    // 少なくとも1つの項目が入力されているかチェック
    const hasInput = Object.values(formData).some(value => value !== '');

    if (!hasInput) {
        alert('少なくとも1つの項目を入力してください。');
        return;
    }

    // 仕様書とプロンプトを生成
    generatedSpec = generateSpecification(formData);
    generatedPrompt = generatePrompt(formData);

    // 出力エリアに表示
    document.getElementById('specOutput').textContent = generatedSpec;
    document.getElementById('promptOutput').textContent = generatedPrompt;

    // 出力エリアを表示
    document.getElementById('outputArea').style.display = 'block';

    // 出力エリアまでスクロール
    document.getElementById('outputArea').scrollIntoView({ behavior: 'smooth' });
}

// フォームのクリア
function clearForm() {
    if (!confirm('入力内容をすべてクリアしますか？')) {
        return;
    }

    // すべての入力フィールドをクリア
    const inputs = document.querySelectorAll('input[type="text"], textarea');
    inputs.forEach(input => {
        input.value = '';
        input.disabled = false;
        input.classList.remove('option-selected');
    });

    // selectフィールドをクリア
    const selects = document.querySelectorAll('select');
    selects.forEach(select => {
        select.value = '';
        select.disabled = false;
        select.classList.remove('option-selected');
    });

    // 「その他」入力欄を非表示に
    const appTypeOther = document.getElementById('appTypeOther');
    if (appTypeOther) {
        appTypeOther.style.display = 'none';
        appTypeOther.value = '';
    }

    // 本番環境の入力欄を非表示に
    const prodEnvironmentTextarea = document.getElementById('prodEnvironment');
    if (prodEnvironmentTextarea) {
        prodEnvironmentTextarea.style.display = 'none';
        prodEnvironmentTextarea.value = '';
    }

    // すべてのオプションボタンを非アクティブに
    const optionButtons = document.querySelectorAll('.option-btn');
    optionButtons.forEach(btn => btn.classList.remove('active'));

    // 出力エリアを非表示
    document.getElementById('outputArea').style.display = 'none';

    // ページトップにスクロール
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// クリップボードにコピー
function copyToClipboard(text, type) {
    navigator.clipboard.writeText(text).then(() => {
        const typeName = type === 'spec' ? '要求仕様書' : 'プロンプト';
        alert(`${typeName}をクリップボードにコピーしました。`);
    }).catch(err => {
        console.error('コピーに失敗しました:', err);
        alert('コピーに失敗しました。');
    });
}

// ファイルダウンロード
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

// ダークモード関連の関数

// テーマの初期化（ローカルストレージから読み込み）
function initializeTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark-mode');
        updateThemeIcon(true);
    } else {
        updateThemeIcon(false);
    }
}

// テーマの切り替え
function toggleTheme() {
    const isDarkMode = document.body.classList.toggle('dark-mode');
    localStorage.setItem('theme', isDarkMode ? 'dark' : 'light');
    updateThemeIcon(isDarkMode);
}

// テーマアイコンの更新
function updateThemeIcon(isDarkMode) {
    const themeIcon = document.querySelector('.theme-icon');
    themeIcon.textContent = isDarkMode ? '☀️' : '🌙';
}
