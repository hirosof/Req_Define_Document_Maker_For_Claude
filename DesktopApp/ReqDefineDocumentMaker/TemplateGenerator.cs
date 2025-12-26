using System.Text;

namespace ReqDefineDocumentMaker;

/// <summary>
/// 要求仕様書とプロンプトを生成するクラス
/// </summary>
public static class TemplateGenerator
{
    /// <summary>
    /// 要求仕様書（Markdown）を生成
    /// </summary>
    public static string GenerateSpecification(Dictionary<string, string> formData)
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var sb = new StringBuilder();

        sb.AppendLine("# 要求仕様書");
        sb.AppendLine();
        sb.AppendLine($"**作成日:** {today}");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // 基本情報
        sb.AppendLine("## 1. 基本情報");
        sb.AppendLine();
        sb.AppendLine("### 1.1 プロジェクト名");
        sb.AppendLine(GetValueOrDefault(formData, "projectName"));
        sb.AppendLine();
        sb.AppendLine("### 1.2 概要/目的");
        sb.AppendLine(GetValueOrDefault(formData, "overview"));
        sb.AppendLine();
        sb.AppendLine("### 1.3 対象ユーザー");
        sb.AppendLine(GetValueOrDefault(formData, "targetUser"));
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // 技術要件
        sb.AppendLine("## 2. 技術要件");
        sb.AppendLine();
        sb.AppendLine("### 2.1 アプリケーション種類");
        sb.AppendLine(GetValueOrDefault(formData, "appType"));
        sb.AppendLine();
        sb.AppendLine("### 2.2 使用言語");
        sb.AppendLine(GetValueOrDefault(formData, "language"));
        sb.AppendLine();
        sb.AppendLine("### 2.3 フレームワーク/ライブラリ");
        sb.AppendLine(GetValueOrDefault(formData, "framework"));
        sb.AppendLine();
        sb.AppendLine("### 2.4 開発環境");
        sb.AppendLine(GetValueOrDefault(formData, "devEnvironment"));
        sb.AppendLine();
        sb.AppendLine("### 2.5 本番環境");
        sb.AppendLine(GetValueOrDefault(formData, "prodEnvironment"));
        sb.AppendLine();
        sb.AppendLine("### 2.6 制約事項");
        sb.AppendLine(GetValueOrDefault(formData, "constraints"));
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // 機能要件
        sb.AppendLine("## 3. 機能要件");
        sb.AppendLine();
        sb.AppendLine("### 3.1 主要機能");
        sb.AppendLine(GetValueOrDefault(formData, "mainFeatures"));
        sb.AppendLine();
        sb.AppendLine("### 3.2 画面/UI要件");
        sb.AppendLine(GetValueOrDefault(formData, "uiRequirements"));
        sb.AppendLine();
        sb.AppendLine("### 3.3 データ要件");
        sb.AppendLine(GetValueOrDefault(formData, "dataRequirements"));
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // その他
        sb.AppendLine("## 4. その他");
        sb.AppendLine();
        sb.AppendLine("### 4.1 参考資料");
        sb.AppendLine(GetValueOrDefault(formData, "references"));
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();

        // 議論が必要な項目
        sb.AppendLine("## 5. 議論が必要な項目");
        sb.AppendLine();
        sb.AppendLine(GenerateDiscussionItems(formData));
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("## 備考");
        sb.AppendLine();

        return sb.ToString();
    }

    /// <summary>
    /// Claude向けプロンプトを生成
    /// </summary>
    public static string GeneratePrompt(Dictionary<string, string> formData)
    {
        var sb = new StringBuilder();
        var hasDiscussion = formData.Values.Any(v => v == "別途議論" || v == "お任せ");

        sb.AppendLine("以下の要求仕様書に基づいて、プロジェクトの実装について議論したいと思います。");
        sb.AppendLine();

        // 「お任せ」や「別途議論」がある場合の指示
        if (hasDiscussion)
        {
            sb.AppendLine("まず、以下の点について確認・提案をお願いします：");
            sb.AppendLine();

            var fieldNames = GetFieldNames();

            foreach (var kvp in formData)
            {
                if (kvp.Value == "別途議論")
                {
                    sb.AppendLine($"- **{fieldNames[kvp.Key]}**: 詳細について議論したい");
                }
                else if (kvp.Value == "お任せ")
                {
                    sb.AppendLine($"- **{fieldNames[kvp.Key]}**: 最適な提案をお願いします");
                }
            }

            sb.AppendLine();
            sb.AppendLine("議論の後、二次要求仕様書を作成してください。");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("内容を確認の上、二次要求仕様書を作成してください。");
            sb.AppendLine();
        }

        sb.AppendLine("---");
        sb.AppendLine();
        sb.AppendLine("# 要求仕様書");
        sb.AppendLine();

        // 基本情報
        sb.AppendLine("## 1. 基本情報");
        sb.AppendLine();
        sb.AppendLine($"**プロジェクト名:** {GetValueOrDefault(formData, "projectName")}");
        sb.AppendLine();
        sb.AppendLine("**概要/目的:**");
        sb.AppendLine(GetValueOrDefault(formData, "overview"));
        sb.AppendLine();
        sb.AppendLine($"**対象ユーザー:** {GetValueOrDefault(formData, "targetUser")}");
        sb.AppendLine();

        // 技術要件
        sb.AppendLine("## 2. 技術要件");
        sb.AppendLine();
        sb.AppendLine($"**アプリケーション種類:** {GetValueOrDefault(formData, "appType")}");
        sb.AppendLine();
        sb.AppendLine($"**使用言語:** {GetValueOrDefault(formData, "language")}");
        sb.AppendLine();
        sb.AppendLine($"**フレームワーク/ライブラリ:** {GetValueOrDefault(formData, "framework")}");
        sb.AppendLine();
        sb.AppendLine("**開発環境:**");
        sb.AppendLine(GetValueOrDefault(formData, "devEnvironment"));
        sb.AppendLine();
        sb.AppendLine("**本番環境:**");
        sb.AppendLine(GetValueOrDefault(formData, "prodEnvironment"));
        sb.AppendLine();
        sb.AppendLine("**制約事項:**");
        sb.AppendLine(GetValueOrDefault(formData, "constraints"));
        sb.AppendLine();

        // 機能要件
        sb.AppendLine("## 3. 機能要件");
        sb.AppendLine();
        sb.AppendLine("**主要機能:**");
        sb.AppendLine(GetValueOrDefault(formData, "mainFeatures"));
        sb.AppendLine();
        sb.AppendLine("**画面/UI要件:**");
        sb.AppendLine(GetValueOrDefault(formData, "uiRequirements"));
        sb.AppendLine();
        sb.AppendLine("**データ要件:**");
        sb.AppendLine(GetValueOrDefault(formData, "dataRequirements"));
        sb.AppendLine();

        // その他
        sb.AppendLine("## 4. その他");
        sb.AppendLine();
        sb.AppendLine("**参考資料:**");
        sb.AppendLine(GetValueOrDefault(formData, "references"));

        return sb.ToString();
    }

    /// <summary>
    /// 議論が必要な項目を生成
    /// </summary>
    private static string GenerateDiscussionItems(Dictionary<string, string> formData)
    {
        var items = new List<string>();
        var fieldNames = GetFieldNames();

        foreach (var kvp in formData)
        {
            if (kvp.Value == "別途議論")
            {
                items.Add($"- **{fieldNames[kvp.Key]}**: 詳細について議論が必要");
            }
            else if (kvp.Value == "お任せ")
            {
                items.Add($"- **{fieldNames[kvp.Key]}**: Claude に最適な提案を依頼");
            }
        }

        return items.Count == 0 ? "特になし" : string.Join(Environment.NewLine, items);
    }

    /// <summary>
    /// 値を取得（空の場合は「（未記入）」を返す）
    /// </summary>
    private static string GetValueOrDefault(Dictionary<string, string> formData, string key)
    {
        if (formData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        return "（未記入）";
    }

    /// <summary>
    /// フィールド名のマッピングを取得
    /// </summary>
    private static Dictionary<string, string> GetFieldNames()
    {
        return new Dictionary<string, string>
        {
            { "projectName", "プロジェクト名" },
            { "overview", "概要/目的" },
            { "targetUser", "対象ユーザー" },
            { "appType", "アプリケーション種類" },
            { "language", "使用言語" },
            { "framework", "フレームワーク/ライブラリ" },
            { "devEnvironment", "開発環境" },
            { "prodEnvironment", "本番環境" },
            { "constraints", "制約事項" },
            { "mainFeatures", "主要機能" },
            { "uiRequirements", "画面/UI要件" },
            { "dataRequirements", "データ要件" },
            { "references", "参考資料" }
        };
    }
}
