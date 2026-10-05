using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Команды балансировки для вызова из Unity CLI и меню. Возвращают готовый текст отчёта.
    // Путь к JSON — абсолютный или от корня проекта.
    public static class LevelBalanceCommands
    {
        private const string VALIDATE_MENU = "Tools/Levels/Проверить все уровни";
        private const string EXPORT_MENU = "Tools/Levels/Обновить срезы";
        private const string ITEM_INDENT = "  ";

        public static string ExportAll()
        {
            List<string> changes = LevelSliceExporter.ExportAll();

            if (changes.Count == 0)
                return "Срезы актуальны, изменений нет.";

            StringBuilder text = new();
            AppendSection(text, "Срезы обновлены", changes);

            return text.ToString();
        }

        public static string ValidateAll() => BuildValidationReport(out _, out _);

        // Отчёт проверки в консоль: с ошибками — как ошибка, с одними предупреждениями — как предупреждение.
        public static void LogValidationReport()
        {
            string report = BuildValidationReport(out int errorsCount, out int warningsCount);

            if (errorsCount > 0)
                Debug.LogError(report);
            else if (warningsCount > 0)
                Debug.LogWarning(report);
            else
                Debug.Log(report);
        }

        public static string PreviewImport(string levelName, string jsonPath) => RunImport(levelName, jsonPath, isApplying: false);

        public static string Import(string levelName, string jsonPath) => RunImport(levelName, jsonPath, isApplying: true);

        [MenuItem(VALIDATE_MENU)]
        private static void ValidateAllFromMenu() => LogValidationReport();

        [MenuItem(EXPORT_MENU)]
        private static void ExportAllFromMenu()
        {
            string report = ExportAll();
            Debug.Log(report);
        }

        private static string BuildValidationReport(out int errorsCount, out int warningsCount)
        {
            errorsCount = 0;
            warningsCount = 0;

            LevelsListConfig campaign = LevelEditorAssets.FindLevelsList();
            StringBuilder levelsText = new();

            foreach (LevelConfig level in LevelEditorAssets.FindLevelConfigs())
            {
                LevelPrefabInfo prefab = LevelPrefabReader.Read(level);
                List<LevelIssue> issues = LevelValidator.Validate(level, prefab);
                int levelErrorsCount = issues.Count(issue => issue.IsError);
                int levelWarningsCount = issues.Count - levelErrorsCount;

                errorsCount += levelErrorsCount;
                warningsCount += levelWarningsCount;

                string campaignText = DescribeCampaign(campaign, level);
                string issuesCountText = DescribeIssuesCount(levelErrorsCount, levelWarningsCount);
                levelsText.AppendLine($"{level.name} ({campaignText}): {issuesCountText}");

                foreach (LevelIssue issue in issues)
                {
                    string severityText = issue.IsError ? "Ошибка" : "Предупреждение";
                    string issueText = issue.ToString();
                    levelsText.AppendLine($"{ITEM_INDENT}{severityText}: {issueText}");
                }
            }

            StringBuilder report = new();
            report.AppendLine($"Проверка уровней: ошибок {errorsCount}, предупреждений {warningsCount}.");
            report.Append(levelsText);

            return report.ToString();
        }

        private static string DescribeCampaign(LevelsListConfig campaign, LevelConfig level)
        {
            int? campaignNumber = LevelEditorAssets.GetCampaignNumber(campaign, level);

            if (campaignNumber.HasValue == false)
                return "вне кампании";

            return $"кампания, №{campaignNumber.Value}";
        }

        private static string DescribeIssuesCount(int errorsCount, int warningsCount)
        {
            if (errorsCount == 0 && warningsCount == 0)
                return "замечаний нет";

            return $"ошибок {errorsCount}, предупреждений {warningsCount}";
        }

        private static string RunImport(string levelName, string jsonPath, bool isApplying)
        {
            LevelConfig config = LevelEditorAssets.FindLevelConfig(levelName);

            if (config == null)
                return $"Уровень {levelName} не найден среди LevelConfig.";

            string fullPath = Path.IsPathRooted(jsonPath) ? jsonPath : LevelSliceExporter.ToFullPath(jsonPath);

            if (File.Exists(fullPath) == false)
                return $"Файл не найден: {fullPath}";

            string json = File.ReadAllText(fullPath);
            LevelSliceImportResult result = isApplying
                ? LevelSliceImporter.Import(config, json)
                : LevelSliceImporter.Preview(config, json);

            string importReport = DescribeImport(result, config, isApplying);

            if (result.IsApplied == false)
                return importReport;

            // Срезы обновляются сразу: автоэкспорт ждёт тика редактора, а свёрнутый редактор может не тикать.
            string exportReport = ExportAll();

            return importReport + exportReport;
        }

        private static string DescribeImport(LevelSliceImportResult result, LevelConfig config, bool isApplying)
        {
            StringBuilder text = new();
            string modeText = isApplying ? "импорт среза" : "предпросмотр импорта среза";

            text.AppendLine($"{result.LevelName}: {modeText}");

            if (result.HasChanges)
                AppendSection(text, "Изменения", result.Changes);

            if (result.AnalysisChanges.Count > 0)
                AppendSection(text, "Анализ после правки", result.AnalysisChanges);

            if (result.HasErrors)
                AppendSection(text, "Ошибки", result.Errors);

            if (result.Warnings.Count > 0)
                AppendSection(text, "Предупреждения после импорта", result.Warnings);

            text.AppendLine(DescribeImportOutcome(result, config));

            return text.ToString();
        }

        private static string DescribeImportOutcome(LevelSliceImportResult result, LevelConfig config)
        {
            if (result.HasErrors)
                return "Итог: импорт отклонён, ассет не изменён.";

            if (result.HasChanges == false)
                return "Итог: нет изменений.";

            if (result.IsApplied == false)
                return "Итог: предпросмотр, ничего не применено.";

            string undoName = LevelSliceImporter.GetUndoName(config);

            return $"Итог: применено и сохранено. Отменить: Edit → Undo «{undoName}».";
        }

        private static void AppendSection(StringBuilder text, string title, IReadOnlyCollection<string> lines)
        {
            text.AppendLine($"{title} ({lines.Count}):");

            foreach (string line in lines)
                text.AppendLine($"{ITEM_INDENT}{line}");
        }
    }
}
