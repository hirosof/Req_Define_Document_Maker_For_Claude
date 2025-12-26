# デスクトップアプリ版 仕様書

## 1. 概要

### 1.1 プロジェクト名
要求仕様書作成ツール for Claude（デスクトップアプリ版）

### 1.2 目的
Claudeに依頼するための要求仕様書を効率的に作成し、プロジェクト要件を体系的に整理するWindowsデスクトップアプリケーション。

### 1.3 対象ユーザー
- ソフトウェア開発者
- プロジェクトマネージャー
- Claudeを利用して開発を進めたい個人・チーム
- デスクトップアプリケーションを好むユーザー

---

## 2. 技術スタック

### 2.1 言語
- C# 12

### 2.2 フレームワーク
- WPF (Windows Presentation Foundation)
- .NET 8

### 2.3 動作環境
- Windows 10 / 11
- .NET 8 Runtime

---

## 3. ファイル構成

```
DesktopApp/ReqDefineDocumentMaker/
├── App.xaml                    # アプリケーションリソース定義
├── App.xaml.cs                 # アプリケーションエントリーポイント
├── MainWindow.xaml             # メインウィンドウUI定義（XAML）
├── MainWindow.xaml.cs          # メインウィンドウロジック
├── TemplateGenerator.cs        # テンプレート生成クラス
└── ReqDefineDocumentMaker.csproj  # プロジェクトファイル
```

---

## 4. 機能仕様

### 4.1 入力フォーム（TabControl構成）

#### 4.1.1 要件入力タブ

##### 基本情報
| 項目名 | コントロール | オプション |
|--------|------------|-----------|
| プロジェクト名 | TextBox | お任せ / 別途議論 |
| 概要/目的 | TextBox (MultiLine) | お任せ / 別途議論 |
| 対象ユーザー | TextBox | お任せ / 別途議論 |

##### 技術要件
| 項目名 | コントロール | オプション |
|--------|------------|-----------|
| アプリケーション種類 | ComboBox（CLI/デスクトップ/Web/その他） | お任せ / 別途議論 |
| 使用言語 | TextBox | お任せ / 別途議論 |
| フレームワーク/ライブラリ | TextBox | お任せ / 別途議論 |
| 開発環境 | TextBox (MultiLine) | お任せ / 別途議論 |
| 本番環境 | ComboBox（開発環境と同じ/カスタム） | お任せ / 別途議論 |
| 制約事項 | TextBox (MultiLine) | お任せ / 別途議論 |

##### 機能要件
| 項目名 | コントロール | オプション |
|--------|------------|-----------|
| 主要機能 | TextBox (MultiLine) | お任せ / 別途議論 |
| 画面/UI要件 | TextBox (MultiLine) | お任せ / 別途議論 |
| データ要件 | TextBox (MultiLine) | お任せ / 別途議論 |

##### その他
| 項目名 | コントロール | オプション |
|--------|------------|-----------|
| 参考資料 | TextBox (MultiLine) | お任せ / 別途議論 |

##### アクションボタン
- 「要求仕様書を生成」ボタン
- 「クリア」ボタン

#### 4.1.2 生成結果タブ
- 一次要求仕様書（Markdown）表示エリア（TextBox, ReadOnly, MultiLine）
- Claude向けプロンプト表示エリア（TextBox, ReadOnly, MultiLine）
- 各エリアにコピー・保存ボタン

### 4.2 オプションボタン機能

#### 4.2.1 動作仕様
- 「お任せ」ボタンをクリック
  - 対応する入力コントロールを無効化
  - ボタンの背景色を変更（アクティブ状態）
  - 内部状態（Dictionary）に「お任せ」を記録

- 「別途議論」ボタンをクリック
  - 対応する入力コントロールを無効化
  - ボタンの背景色を変更（アクティブ状態）
  - 内部状態（Dictionary）に「別途議論」を記録

- 再度同じボタンをクリック
  - 選択を解除
  - 入力コントロールを有効化
  - ボタンを通常状態に戻す
  - 内部状態から削除

#### 4.2.2 実装（C#）
```csharp
private readonly Dictionary<string, string> _fieldStates = new();

private void HandleOptionClick(string fieldName, string optionValue,
    Control targetControl, Button clickedButton)
{
    // 同じフィールドの他のボタンを検索
    var parentPanel = clickedButton.Parent as StackPanel;
    var siblingButtons = parentPanel.Children.OfType<Button>()
        .Where(b => b != clickedButton);

    // トグル処理
    if (_fieldStates.ContainsKey(fieldName) &&
        _fieldStates[fieldName] == optionValue)
    {
        // 解除
        _fieldStates.Remove(fieldName);
        targetControl.IsEnabled = true;
        clickedButton.Background = Brushes.White;
    }
    else
    {
        // 他のボタンを非アクティブ化
        foreach (var btn in siblingButtons)
        {
            btn.Background = Brushes.White;
        }

        // アクティブ化
        _fieldStates[fieldName] = optionValue;
        targetControl.IsEnabled = false;
        clickedButton.Background = new SolidColorBrush(Color.FromRgb(0x66, 0x7e, 0xea));
    }
}
```

### 4.3 出力機能

#### 4.3.1 一次要求仕様書（Markdown）
- `TemplateGenerator.GenerateSpecification()`メソッドで生成
- Dictionary形式のフォームデータを受け取り
- Markdown形式のstringを返す
- オプション選択された項目は「お任せ」「別途議論を希望」と表記
- 日付を自動挿入

#### 4.3.2 Claude向けプロンプト
- `TemplateGenerator.GeneratePrompt()`メソッドで生成
- 要求仕様書の内容を含む
- Claudeへの指示を含むプロンプト形式

### 4.4 エクスポート機能

#### 4.4.1 クリップボードコピー
**実装方式: TextBoxBase.Copy()を使用**

```csharp
private void CopyToClipboardViaTextBox(string text, string itemName, TextBox textBox)
{
    try
    {
        // TextBoxの内容を全選択してコピー
        // WPFコントロール経由なのでウイルス対策ソフトとの競合が起きにくい
        textBox.SelectAll();
        textBox.Copy();

        MessageBox.Show($"{itemName}をクリップボードにコピーしました。",
            "コピー完了", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"コピーに失敗しました: {ex.Message}",
            "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

**利点:**
- ウイルス対策ソフトのクリップボード保護機能との競合を回避
- シンプルで安定した実装
- リトライロジック不要

#### 4.4.2 ファイル保存
- SaveFileDialogを使用
- ファイル名: `要求仕様書.md` / `Claude向けプロンプト.txt`
- UTF-8エンコーディングで保存

```csharp
private void SaveToFile(string content, string defaultFileName)
{
    var dialog = new SaveFileDialog
    {
        FileName = defaultFileName,
        Filter = defaultFileName.EndsWith(".md")
            ? "Markdown files (*.md)|*.md|All files (*.*)|*.*"
            : "Text files (*.txt)|*.txt|All files (*.*)|*.*"
    };

    if (dialog.ShowDialog() == true)
    {
        try
        {
            File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
            MessageBox.Show($"ファイルを保存しました。\n{dialog.FileName}",
                "保存完了", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存に失敗しました: {ex.Message}",
                "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
```

### 4.5 クリアボタン
- 全入力コントロールをクリア
- 全オプションボタンを非アクティブ化
- 内部状態（Dictionary）をクリア
- 確認ダイアログを表示（MessageBox）
- 「その他」入力欄・本番環境カスタム入力欄を非表示
- 出力タブを非表示（Visibility.Collapsed）

### 4.6 ダークモード

#### 4.6.1 機能概要
- ヘッダー右上に切り替えボタン（🌙/☀️）を配置
- ボタンクリックでライト⇔ダーク切り替え
- アイコンが動的に変化（🌙→☀️→🌙）
- 設定をレジストリに保存
- 次回起動時も設定を維持

#### 4.6.2 レジストリキー
```
HKEY_CURRENT_USER\Software\hirosof\ReqDefineDocumentMakerForClaude
値名: Theme
値: "Light" または "Dark"
```

#### 4.6.3 実装方式
**DynamicResourceによる動的テーマ切り替え**

**App.xamlでデフォルト色を定義:**
```xml
<Application.Resources>
    <ResourceDictionary>
        <!-- ライトモード（デフォルト） -->
        <SolidColorBrush x:Key="BackgroundPrimaryBrush" Color="#F5F5F5"/>
        <SolidColorBrush x:Key="BackgroundSecondaryBrush" Color="#FFFFFF"/>
        <SolidColorBrush x:Key="TextPrimaryBrush" Color="#333333"/>
        <SolidColorBrush x:Key="TextSecondaryBrush" Color="#555555"/>
        <SolidColorBrush x:Key="BorderBrush" Color="#DDDDDD"/>
        <SolidColorBrush x:Key="InputDisabledBackgroundBrush" Color="#F0F0F0"/>
        <SolidColorBrush x:Key="InputDisabledBorderBrush" Color="#999999"/>
    </ResourceDictionary>
</Application.Resources>
```

**C#でテーマを動的に変更:**
```csharp
private void ApplyTheme()
{
    var resources = Application.Current.Resources;

    if (_isDarkMode)
    {
        // ダークモード
        resources["BackgroundPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0x1e, 0x1e, 0x1e));
        resources["BackgroundSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(0x2d, 0x2d, 0x2d));
        resources["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0xe0, 0xe0, 0xe0));
        resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(0xb0, 0xb0, 0xb0));
        resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x44));
        resources["InputDisabledBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0x3a, 0x3a, 0x3a));
        resources["InputDisabledBorderBrush"] = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
    }
    else
    {
        // ライトモード（デフォルト色に戻す）
        resources["BackgroundPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0xf5, 0xf5, 0xf5));
        // ... 以下同様
    }

    UpdateThemeIcon();
    this.Background = resources["BackgroundPrimaryBrush"] as SolidColorBrush;
}
```

#### 4.6.4 カスタムコントロールテンプレート

**ComboBox:**
- ToggleButton、ContentPresenter、Popupで構成
- ドロップダウンリストの背景色・文字色をテーマに対応
- ハイライト時は紫色（#667eea）

**ComboBoxItem:**
- 背景色・文字色をDynamicResourceで指定
- IsHighlighted時は紫色背景

**TabItem:**
- Border、ContentPresenterで構成
- 選択状態（IsSelected=true）: 紫色のボーダー、背景色変更
- 非選択状態: 薄い文字色（TextSecondaryBrush）

#### 4.6.5 対応コンポーネント
- Window背景
- TextBox（入力欄）
- ComboBox（ドロップダウン・アイテム含む）
- TabControl・TabItem
- ScrollViewer
- Label（TextSecondaryBrush使用）
- オプションボタン（OptionButtonStyle）

---

## 5. UI/UXデザイン

### 5.1 ウィンドウ仕様
- タイトル: 「要求仕様書作成ツール for Claude」
- サイズ: 1200x800px
- 起動位置: 画面中央（WindowStartupLocation="CenterScreen"）
- リサイズ可能

### 5.2 レイアウト
- Grid構成（2行）
  - Row 0: ヘッダー（Auto）
  - Row 1: メインコンテンツ（TabControl）
- ヘッダー: 紫色のグラデーション背景（#667eea - #764ba2）
- ScrollViewer使用（縦スクロール対応）

### 5.3 カラーパレット

#### 5.3.1 ライトモード
- プライマリカラー: #667eea（紫）
- 背景色（プライマリ）: #f5f5f5
- 背景色（セカンダリ）: #ffffff
- テキスト色（プライマリ）: #333333
- テキスト色（セカンダリ）: #555555
- ボーダー色: #dddddd
- 入力欄無効時背景: #f0f0f0
- 入力欄無効時ボーダー: #999999

#### 5.3.2 ダークモード
- プライマリカラー: #667eea（紫）※変更なし
- 背景色（プライマリ）: #1e1e1e
- 背景色（セカンダリ）: #2d2d2d
- テキスト色（プライマリ）: #e0e0e0
- テキスト色（セカンダリ）: #b0b0b0
- ボーダー色: #444444
- 入力欄無効時背景: #3a3a3a
- 入力欄無効時ボーダー: #555555

### 5.4 タイポグラフィ
- 標準フォント: Segoe UI
- セクションヘッダー: 18px, Bold, #667eea
- ラベル: SemiBold
- 出力エリア（コード）: Consolas（モノスペースフォント）

### 5.5 スタイル定義

**SectionHeaderStyle:**
```xml
<Style x:Key="SectionHeaderStyle" TargetType="TextBlock">
    <Setter Property="FontSize" Value="18"/>
    <Setter Property="FontWeight" Value="Bold"/>
    <Setter Property="Foreground" Value="#667eea"/>
    <Setter Property="Margin" Value="0,10,0,10"/>
</Style>
```

**LabelStyle:**
```xml
<Style x:Key="LabelStyle" TargetType="TextBlock">
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="Margin" Value="0,5,0,5"/>
    <Setter Property="Foreground" Value="{DynamicResource TextSecondaryBrush}"/>
</Style>
```

**OptionButtonStyle:**
```xml
<Style x:Key="OptionButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="10,5"/>
    <Setter Property="Margin" Value="5,0"/>
    <Setter Property="MinWidth" Value="80"/>
    <Setter Property="Background" Value="{DynamicResource BackgroundSecondaryBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource BorderBrush}"/>
    <Setter Property="BorderThickness" Value="2"/>
    <Setter Property="Foreground" Value="{DynamicResource TextPrimaryBrush}"/>
</Style>
```

**PrimaryButtonStyle:**
```xml
<Style x:Key="PrimaryButtonStyle" TargetType="Button">
    <Setter Property="Padding" Value="20,10"/>
    <Setter Property="FontWeight" Value="Bold"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="Margin" Value="10"/>
    <Setter Property="Background" Value="#667eea"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="BorderThickness" Value="0"/>
</Style>
```

---

## 6. データフロー

```
[ユーザー入力（WPFコントロール）]
    ↓
[GetFormData() → Dictionary<string, string>]
    ↓
[TemplateGenerator.GenerateSpecification()]
    ↓
[Markdown形式の要求仕様書（string）]
    ↓
[TextBox表示 / クリップボード / ファイル保存]
```

---

## 7. エラーハンドリング

### 7.1 入力バリデーション
- 全項目が空の場合: MessageBoxで警告表示
  - 「少なくとも1つの項目を入力してください。」

### 7.2 クリップボード操作
- `TextBoxBase.Copy()`使用により、エラーはほぼ発生しない
- 万が一のエラー時: MessageBoxでエラーメッセージ表示

### 7.3 ファイル保存
- `File.WriteAllText()`のIOException等をキャッチ
- エラー時: MessageBoxでエラーメッセージ表示

### 7.4 レジストリアクセス
- `Registry.CurrentUser.OpenSubKey()`でnullチェック
- エラー時: デフォルト（ライトモード）を使用

### 7.5 起動時クラッシュ対策
- `InitializeComponent()`中のイベント発火を防ぐnullチェック
  - `AppTypeComboBox_SelectionChanged`内で`AppTypeOtherTextBox == null`チェック
  - `ProdEnvironmentComboBox_SelectionChanged`内で`ProdEnvironmentTextBox == null`チェック

### 7.6 グローバル例外ハンドラー
```csharp
// App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    DispatcherUnhandledException += App_DispatcherUnhandledException;
    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
}
```

---

## 8. システム要件

### 8.1 必須要件
- OS: Windows 10 (1809以降) または Windows 11
- .NET 8 Runtime（Desktop Runtime）

### 8.2 推奨要件
- メモリ: 4GB以上
- ディスク空き容量: 100MB以上

---

## 9. パフォーマンス

### 9.1 起動時間
- 約1-2秒（.NET 8のコールドスタート）

### 9.2 メモリ使用量
- 約50-80MB（アイドル時）

### 9.3 ファイル保存速度
- 即座（数MB程度のテキストは瞬時）

---

## 10. セキュリティ

### 10.1 対策
- すべてローカルで完結（ネットワーク通信なし）
- ユーザーデータはレジストリにテーマ設定のみ保存
- ファイル保存はユーザーが明示的に選択したパスのみ

### 10.2 制限事項
- レジストリへの書き込み権限が必要（通常は問題なし）

---

## 11. 既知の問題と解決策

### 11.1 クリップボードコピーエラー（環境依存）
**問題:** ウイルス対策ソフトの「クリップボード保護」機能が干渉

**解決策:** `TextBoxBase.Copy()`を使用することで競合を回避

**技術詳細:**
- `Clipboard.SetText()`や`Clipboard.SetDataObject()`を直接使用すると、ウイルス対策ソフト（特にブラウザ関連のクリップボード保護）との競合が発生
- WPFコントロール経由の`TextBoxBase.Copy()`は異なる処理パスを通るため競合しにくい

---

## 12. 今後の拡張可能性

- 過去の仕様書の保存・読み込み機能（アプリケーション設定フォルダ利用）
- テンプレートのカスタマイズ機能（XAML動的読み込み）
- プロジェクトタイプ別のプリセット（JSONファイル管理）
- 複数プロジェクトの管理機能
- Claude APIとの直接連携機能
- マルチ言語対応（リソースファイル利用）

---

## 13. ビルド・配布

### 13.1 ビルドコマンド
```bash
dotnet build
```

### 13.2 リリースビルド
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

### 13.3 実行ファイル
- パス: `bin\Debug\net8.0-windows\ReqDefineDocumentMaker.exe`
- リリース版: `bin\Release\net8.0-windows\win-x64\publish\`

### 13.4 配布方法
- 単一実行ファイル + .NET 8 Runtime依存
- または、self-contained版（.NET Runtimeを含む）

---

## 14. テスト

### 14.1 単体テスト
- `TemplateGenerator.cs`のテスト（xUnit等）

### 14.2 統合テスト
- UI操作の手動テスト
- クリップボードコピー動作確認
- ファイル保存動作確認
- ダークモード切り替え確認

### 14.3 環境テスト
- Windows 10 / 11での動作確認
- ウイルス対策ソフト有効時の動作確認

---

## 15. 変更履歴

| 日付 | バージョン | 変更内容 |
|------|-----------|---------|
| 2025-12-27 | 1.0.0 | 初版作成 |
| 2025-12-27 | 1.1.0 | クリップボードコピー方式変更（TextBoxBase.Copy()） |
| 2025-12-27 | 1.2.0 | ダークモード機能追加 |
| 2025-12-27 | 1.2.1 | ComboBox・TabItemのダークモード完全対応 |
| 2025-12-27 | 1.2.2 | レジストリキー変更（`Software\hirosof\ReqDefineDocumentMakerForClaude`） |
