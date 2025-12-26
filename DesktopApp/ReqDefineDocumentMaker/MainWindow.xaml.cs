using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace ReqDefineDocumentMaker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // フィールドの状態を管理するディクショナリ
    private readonly Dictionary<string, string> _fieldStates = new();
    private string _generatedSpec = "";
    private string _generatedPrompt = "";
    private bool _isDarkMode = false;

    public MainWindow()
    {
        InitializeComponent();
        LoadTheme();
    }

    #region オプションボタンのイベントハンドラ

    // プロジェクト名
    private void ProjectNameOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("projectName", "お任せ", ProjectNameTextBox, sender as Button);
    }

    private void ProjectNameDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("projectName", "別途議論", ProjectNameTextBox, sender as Button);
    }

    // 概要/目的
    private void OverviewOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("overview", "お任せ", OverviewTextBox, sender as Button);
    }

    private void OverviewDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("overview", "別途議論", OverviewTextBox, sender as Button);
    }

    // 対象ユーザー
    private void TargetUserOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("targetUser", "お任せ", TargetUserTextBox, sender as Button);
    }

    private void TargetUserDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("targetUser", "別途議論", TargetUserTextBox, sender as Button);
    }

    // アプリケーション種類
    private void AppTypeOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("appType", "お任せ", AppTypeComboBox, sender as Button);
        AppTypeOtherTextBox.Visibility = Visibility.Collapsed;
        AppTypeOtherTextBox.Text = "";
    }

    private void AppTypeDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("appType", "別途議論", AppTypeComboBox, sender as Button);
        AppTypeOtherTextBox.Visibility = Visibility.Collapsed;
        AppTypeOtherTextBox.Text = "";
    }

    // 使用言語
    private void LanguageOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("language", "お任せ", LanguageTextBox, sender as Button);
    }

    private void LanguageDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("language", "別途議論", LanguageTextBox, sender as Button);
    }

    // フレームワーク/ライブラリ
    private void FrameworkOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("framework", "お任せ", FrameworkTextBox, sender as Button);
    }

    private void FrameworkDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("framework", "別途議論", FrameworkTextBox, sender as Button);
    }

    // 開発環境
    private void DevEnvironmentOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("devEnvironment", "お任せ", DevEnvironmentTextBox, sender as Button);
    }

    private void DevEnvironmentDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("devEnvironment", "別途議論", DevEnvironmentTextBox, sender as Button);
    }

    // 本番環境
    private void ProdEnvironmentOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("prodEnvironment", "お任せ", ProdEnvironmentComboBox, sender as Button);
        ProdEnvironmentTextBox.Visibility = Visibility.Collapsed;
        ProdEnvironmentTextBox.Text = "";
    }

    private void ProdEnvironmentDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("prodEnvironment", "別途議論", ProdEnvironmentComboBox, sender as Button);
        ProdEnvironmentTextBox.Visibility = Visibility.Collapsed;
        ProdEnvironmentTextBox.Text = "";
    }

    // 制約事項
    private void ConstraintsOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("constraints", "お任せ", ConstraintsTextBox, sender as Button);
    }

    private void ConstraintsDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("constraints", "別途議論", ConstraintsTextBox, sender as Button);
    }

    // 主要機能
    private void MainFeaturesOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("mainFeatures", "お任せ", MainFeaturesTextBox, sender as Button);
    }

    private void MainFeaturesDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("mainFeatures", "別途議論", MainFeaturesTextBox, sender as Button);
    }

    // 画面/UI要件
    private void UiRequirementsOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("uiRequirements", "お任せ", UiRequirementsTextBox, sender as Button);
    }

    private void UiRequirementsDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("uiRequirements", "別途議論", UiRequirementsTextBox, sender as Button);
    }

    // データ要件
    private void DataRequirementsOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("dataRequirements", "お任せ", DataRequirementsTextBox, sender as Button);
    }

    private void DataRequirementsDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("dataRequirements", "別途議論", DataRequirementsTextBox, sender as Button);
    }

    // 参考資料
    private void ReferencesOmakase_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("references", "お任せ", ReferencesTextBox, sender as Button);
    }

    private void ReferencesDiscuss_Click(object sender, RoutedEventArgs e)
    {
        HandleOptionClick("references", "別途議論", ReferencesTextBox, sender as Button);
    }

    #endregion

    #region UI制御メソッド

    // オプションボタンのクリック処理
    private void HandleOptionClick(string fieldName, string value, Control control, Button? button)
    {
        if (button == null) return;

        // すでに選択されている場合は解除
        if (_fieldStates.ContainsKey(fieldName) && _fieldStates[fieldName] == value)
        {
            _fieldStates.Remove(fieldName);
            control.IsEnabled = true;
            control.Background = Brushes.White;
            button.Background = Brushes.White;
            button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ddd")!);

            if (control is TextBox textBox)
            {
                textBox.Text = "";
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.SelectedIndex = 0;
            }
        }
        else
        {
            // 新規選択または変更
            _fieldStates[fieldName] = value;
            control.IsEnabled = false;
            control.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f0f0f0")!);

            // 同じフィールドの他のボタンをリセット
            ResetSiblingButtons(button, fieldName);

            // クリックしたボタンをアクティブに
            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#667eea")!);
            button.Foreground = Brushes.White;
            button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#667eea")!);
        }
    }

    // 同じフィールドの他のオプションボタンをリセット
    private void ResetSiblingButtons(Button currentButton, string fieldName)
    {
        var parent = VisualTreeHelper.GetParent(currentButton) as StackPanel;
        if (parent == null) return;

        foreach (var child in parent.Children)
        {
            if (child is Button btn && btn != currentButton)
            {
                btn.Background = Brushes.White;
                btn.Foreground = Brushes.Black;
                btn.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ddd")!);
            }
        }
    }

    // アプリケーション種類の変更
    private void AppTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AppTypeOtherTextBox == null) return; // 初期化前のイベント発火を防ぐ

        if (AppTypeComboBox.SelectedItem is ComboBoxItem item && item.Tag?.ToString() == "Other")
        {
            AppTypeOtherTextBox.Visibility = Visibility.Visible;
        }
        else
        {
            AppTypeOtherTextBox.Visibility = Visibility.Collapsed;
            AppTypeOtherTextBox.Text = "";
        }
    }

    // 本番環境の変更
    private void ProdEnvironmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProdEnvironmentTextBox == null) return; // 初期化前のイベント発火を防ぐ

        if (ProdEnvironmentComboBox.SelectedItem is ComboBoxItem item && item.Tag?.ToString() == "Custom")
        {
            ProdEnvironmentTextBox.Visibility = Visibility.Visible;
        }
        else
        {
            ProdEnvironmentTextBox.Visibility = Visibility.Collapsed;
            ProdEnvironmentTextBox.Text = "";
        }
    }

    #endregion

    #region メインボタンのイベントハンドラ

    // 生成ボタン
    private void GenerateButton_Click(object sender, RoutedEventArgs e)
    {
        var formData = GetFormData();

        // 少なくとも1つの項目が入力されているかチェック
        if (formData.Values.All(string.IsNullOrWhiteSpace))
        {
            MessageBox.Show("少なくとも1つの項目を入力してください。", "入力エラー",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // 仕様書とプロンプトを生成
        _generatedSpec = TemplateGenerator.GenerateSpecification(formData);
        _generatedPrompt = TemplateGenerator.GeneratePrompt(formData);

        // 出力エリアに表示
        SpecOutputTextBox.Text = _generatedSpec;
        PromptOutputTextBox.Text = _generatedPrompt;

        // 出力タブを有効化して切り替え
        OutputTab.IsEnabled = true;
        OutputTab.IsSelected = true;

        MessageBox.Show("要求仕様書とプロンプトを生成しました。", "生成完了",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // クリアボタン
    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("入力内容をすべてクリアしますか？", "確認",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        // すべてのテキストボックスをクリア
        ProjectNameTextBox.Text = "";
        OverviewTextBox.Text = "";
        TargetUserTextBox.Text = "";
        LanguageTextBox.Text = "";
        FrameworkTextBox.Text = "";
        DevEnvironmentTextBox.Text = "";
        ConstraintsTextBox.Text = "";
        MainFeaturesTextBox.Text = "";
        UiRequirementsTextBox.Text = "";
        DataRequirementsTextBox.Text = "";
        ReferencesTextBox.Text = "";
        AppTypeOtherTextBox.Text = "";
        ProdEnvironmentTextBox.Text = "";

        // ComboBoxをリセット
        AppTypeComboBox.SelectedIndex = 0;
        ProdEnvironmentComboBox.SelectedIndex = 0;

        // 状態をクリア
        _fieldStates.Clear();

        // すべてのコントロールを有効化
        ProjectNameTextBox.IsEnabled = true;
        OverviewTextBox.IsEnabled = true;
        TargetUserTextBox.IsEnabled = true;
        AppTypeComboBox.IsEnabled = true;
        LanguageTextBox.IsEnabled = true;
        FrameworkTextBox.IsEnabled = true;
        DevEnvironmentTextBox.IsEnabled = true;
        ProdEnvironmentComboBox.IsEnabled = true;
        ConstraintsTextBox.IsEnabled = true;
        MainFeaturesTextBox.IsEnabled = true;
        UiRequirementsTextBox.IsEnabled = true;
        DataRequirementsTextBox.IsEnabled = true;
        ReferencesTextBox.IsEnabled = true;

        // 背景色をリセット
        ResetControlBackground(ProjectNameTextBox);
        ResetControlBackground(OverviewTextBox);
        ResetControlBackground(TargetUserTextBox);
        ResetControlBackground(AppTypeComboBox);
        ResetControlBackground(LanguageTextBox);
        ResetControlBackground(FrameworkTextBox);
        ResetControlBackground(DevEnvironmentTextBox);
        ResetControlBackground(ProdEnvironmentComboBox);
        ResetControlBackground(ConstraintsTextBox);
        ResetControlBackground(MainFeaturesTextBox);
        ResetControlBackground(UiRequirementsTextBox);
        ResetControlBackground(DataRequirementsTextBox);
        ResetControlBackground(ReferencesTextBox);

        // すべてのオプションボタンをリセット
        ResetAllOptionButtons();

        // 出力エリアを無効化
        OutputTab.IsEnabled = false;
    }

    // コントロールの背景色をリセット
    private void ResetControlBackground(Control control)
    {
        control.Background = Brushes.White;
    }

    // すべてのオプションボタンをリセット
    private void ResetAllOptionButtons()
    {
        ResetButtonsInParent(ProjectNameTextBox);
        ResetButtonsInParent(OverviewTextBox);
        ResetButtonsInParent(TargetUserTextBox);
        ResetButtonsInParent(AppTypeComboBox);
        ResetButtonsInParent(LanguageTextBox);
        ResetButtonsInParent(FrameworkTextBox);
        ResetButtonsInParent(DevEnvironmentTextBox);
        ResetButtonsInParent(ProdEnvironmentComboBox);
        ResetButtonsInParent(ConstraintsTextBox);
        ResetButtonsInParent(MainFeaturesTextBox);
        ResetButtonsInParent(UiRequirementsTextBox);
        ResetButtonsInParent(DataRequirementsTextBox);
        ResetButtonsInParent(ReferencesTextBox);
    }

    private void ResetButtonsInParent(Control control)
    {
        var parent = VisualTreeHelper.GetParent(control);
        while (parent != null && parent is not Grid)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        if (parent is Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is StackPanel panel)
                {
                    foreach (var button in panel.Children.OfType<Button>())
                    {
                        button.Background = Brushes.White;
                        button.Foreground = Brushes.Black;
                        button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ddd")!);
                    }
                }
            }
        }
    }

    #endregion

    #region 出力タブのボタンハンドラ

    // 仕様書をクリップボードにコピー
    private void CopySpecButton_Click(object sender, RoutedEventArgs e)
    {
        CopyToClipboardViaTextBox(_generatedSpec, "要求仕様書", SpecOutputTextBox);
    }

    // プロンプトをクリップボードにコピー
    private void CopyPromptButton_Click(object sender, RoutedEventArgs e)
    {
        CopyToClipboardViaTextBox(_generatedPrompt, "プロンプト", PromptOutputTextBox);
    }

    // TextBoxを経由してクリップボードにコピー（WPFコントロール経由で競合を回避）
    private void CopyToClipboardViaTextBox(string text, string itemName, TextBox textBox)
    {
        try
        {
            // TextBoxの内容を全選択してコピー
            // WPFのコントロール経由なのでウイルス対策ソフトとの競合が起きにくい
            textBox.SelectAll();
            textBox.Copy();

            MessageBox.Show($"{itemName}をクリップボードにコピーしました。", "コピー完了",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"コピーに失敗しました: {ex.Message}", "エラー",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // 仕様書を保存
    private void SaveSpecButton_Click(object sender, RoutedEventArgs e)
    {
        SaveToFile(_generatedSpec, "要求仕様書.md", "Markdownファイル (*.md)|*.md");
    }

    // プロンプトを保存
    private void SavePromptButton_Click(object sender, RoutedEventArgs e)
    {
        SaveToFile(_generatedPrompt, "Claude向けプロンプト.txt", "テキストファイル (*.txt)|*.txt");
    }

    // ファイル保存の共通処理
    private void SaveToFile(string content, string defaultFileName, string filter)
    {
        var dialog = new SaveFileDialog
        {
            FileName = defaultFileName,
            Filter = filter
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllText(dialog.FileName, content, Encoding.UTF8);
                MessageBox.Show("ファイルを保存しました。", "保存完了",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    #endregion

    #region データ取得

    // フォームデータの取得
    private Dictionary<string, string> GetFormData()
    {
        var formData = new Dictionary<string, string>();

        // 基本情報
        formData["projectName"] = GetFieldValue("projectName", ProjectNameTextBox.Text);
        formData["overview"] = GetFieldValue("overview", OverviewTextBox.Text);
        formData["targetUser"] = GetFieldValue("targetUser", TargetUserTextBox.Text);

        // アプリケーション種類
        string appTypeValue = "";
        if (_fieldStates.ContainsKey("appType"))
        {
            appTypeValue = _fieldStates["appType"];
        }
        else if (AppTypeComboBox.SelectedItem is ComboBoxItem appTypeItem)
        {
            var tag = appTypeItem.Tag?.ToString();
            if (tag == "Other")
            {
                appTypeValue = AppTypeOtherTextBox.Text.Trim();
            }
            else if (tag == "CLI")
            {
                appTypeValue = "CLIツール（コンソールアプリケーション）";
            }
            else if (tag == "Desktop")
            {
                appTypeValue = "デスクトップアプリケーション";
            }
            else if (tag == "Web")
            {
                appTypeValue = "Webアプリケーション";
            }
        }
        formData["appType"] = appTypeValue;

        // 技術要件
        formData["language"] = GetFieldValue("language", LanguageTextBox.Text);
        formData["framework"] = GetFieldValue("framework", FrameworkTextBox.Text);
        formData["devEnvironment"] = GetFieldValue("devEnvironment", DevEnvironmentTextBox.Text);

        // 本番環境
        string prodEnvValue = "";
        if (_fieldStates.ContainsKey("prodEnvironment"))
        {
            prodEnvValue = _fieldStates["prodEnvironment"];
        }
        else if (ProdEnvironmentComboBox.SelectedItem is ComboBoxItem prodEnvItem)
        {
            var tag = prodEnvItem.Tag?.ToString();
            if (tag == "SameAsDev")
            {
                prodEnvValue = "開発環境と同じ";
            }
            else if (tag == "Custom")
            {
                prodEnvValue = ProdEnvironmentTextBox.Text.Trim();
            }
        }
        formData["prodEnvironment"] = prodEnvValue;

        formData["constraints"] = GetFieldValue("constraints", ConstraintsTextBox.Text);

        // 機能要件
        formData["mainFeatures"] = GetFieldValue("mainFeatures", MainFeaturesTextBox.Text);
        formData["uiRequirements"] = GetFieldValue("uiRequirements", UiRequirementsTextBox.Text);
        formData["dataRequirements"] = GetFieldValue("dataRequirements", DataRequirementsTextBox.Text);

        // その他
        formData["references"] = GetFieldValue("references", ReferencesTextBox.Text);

        return formData;
    }

    // フィールド値を取得（オプション選択がある場合はそちらを優先）
    private string GetFieldValue(string fieldName, string inputValue)
    {
        if (_fieldStates.ContainsKey(fieldName))
        {
            return _fieldStates[fieldName];
        }
        return inputValue.Trim();
    }

    #endregion

    #region テーマ関連

    // テーマの読み込み（起動時）
    private void LoadTheme()
    {
        // レジストリから設定を読み込み
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\hirosof\ReqDefineDocumentMakerForClaude");
            if (key != null)
            {
                var theme = key.GetValue("Theme")?.ToString();
                if (theme == "Dark")
                {
                    _isDarkMode = true;
                    ApplyTheme();
                }
            }
        }
        catch
        {
            // エラーの場合はデフォルト（ライトモード）を使用
        }
    }

    // テーマの保存
    private void SaveTheme()
    {
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\hirosof\ReqDefineDocumentMakerForClaude");
            key.SetValue("Theme", _isDarkMode ? "Dark" : "Light");
        }
        catch
        {
            // エラーの場合は無視
        }
    }

    // ダークモード切り替えボタンのクリックイベント
    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _isDarkMode = !_isDarkMode;
        ApplyTheme();
        SaveTheme();
    }

    // テーマを適用
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
            // ライトモード
            resources["BackgroundPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0xf5, 0xf5, 0xf5));
            resources["BackgroundSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            resources["TextPrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));
            resources["TextSecondaryBrush"] = new SolidColorBrush(Color.FromRgb(0x55, 0x55, 0x55));
            resources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(0xdd, 0xdd, 0xdd));
            resources["InputDisabledBackgroundBrush"] = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0));
            resources["InputDisabledBorderBrush"] = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99));
        }

        // アイコンを更新
        UpdateThemeIcon();

        // ウィンドウ背景を更新
        this.Background = resources["BackgroundPrimaryBrush"] as SolidColorBrush;
    }

    // テーマアイコンを更新
    private void UpdateThemeIcon()
    {
        // ボタンのテンプレート内のTextBlockを探す
        if (ThemeToggleButton.Template.FindName("ThemeIcon", ThemeToggleButton) is TextBlock iconTextBlock)
        {
            iconTextBlock.Text = _isDarkMode ? "☀️" : "🌙";
        }
    }

    #endregion
}