using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Пишет срезы в Docs/Balance: Levels/<имя LevelConfig>.json на каждый уровень и Reference.json.
    // Файл перезаписывается, только если содержимое изменилось. Папка Levels принадлежит экспорту:
    // срезы уровней, которых больше нет, удаляются.
    public static class LevelSliceExporter
    {
        public const string LEVELS_FOLDER = "Docs/Balance/Levels";
        public const string REFERENCE_FILE = "Docs/Balance/Reference.json";

        private const string SLICE_EXTENSION = ".json";
        private const string SLICE_SEARCH_PATTERN = "*.json";
        private const string WINDOWS_LINE_BREAK = "\r\n";
        private const string UNIX_LINE_BREAK = "\n";

        private static readonly Encoding _utf8WithoutBom = new UTF8Encoding(false);

        public static string ProjectRoot => Path.GetDirectoryName(Application.dataPath);

        // Возвращает список изменённых файлов; пустой список — срезы уже актуальны.
        public static List<string> ExportAll()
        {
            List<string> changes = new();
            GameConfig gameConfig = LevelEditorAssets.FindGameConfig();
            LevelsListConfig campaign = LevelEditorAssets.FindLevelsList();
            HashSet<string> sliceFileNames = new(StringComparer.OrdinalIgnoreCase);

            foreach (LevelConfig level in LevelEditorAssets.FindLevelConfigs())
            {
                string slicePath = GetSlicePath(level.name);
                string sliceJson = CreateSliceJson(level, gameConfig, campaign);

                sliceFileNames.Add(Path.GetFileName(slicePath));
                WriteIfChanged(slicePath, sliceJson, changes);
            }

            BalanceReference reference = LevelSliceSerializer.CreateReference(
                gameConfig,
                LevelEditorAssets.FindEnemyConfigs(),
                LevelEditorAssets.FindTowerConfigs(),
                LevelEditorAssets.FindTowersList());

            WriteIfChanged(REFERENCE_FILE, LevelSliceSerializer.ToJson(reference), changes);
            DeleteStaleSlices(sliceFileNames, changes);

            return changes;
        }

        public static string CreateSliceJson(LevelConfig level, GameConfig gameConfig, LevelsListConfig campaign)
        {
            LevelPrefabInfo prefab = LevelPrefabReader.Read(level);
            LevelStats stats = LevelStatsCalculator.Calculate(level, prefab, gameConfig);
            List<LevelIssue> issues = LevelValidator.Validate(level, prefab);
            int? campaignNumber = LevelEditorAssets.GetCampaignNumber(campaign, level);
            LevelSlice slice = LevelSliceSerializer.CreateSlice(level, campaignNumber, prefab, stats, issues);

            return LevelSliceSerializer.ToJson(slice);
        }

        public static string GetSlicePath(string levelName) => $"{LEVELS_FOLDER}/{levelName}{SLICE_EXTENSION}";

        public static string ToFullPath(string projectRelativePath)
            => Path.GetFullPath(Path.Combine(ProjectRoot, projectRelativePath));

        private static void WriteIfChanged(string relativePath, string content, List<string> changes)
        {
            string fullPath = ToFullPath(relativePath);
            bool isExisting = File.Exists(fullPath);

            if (isExisting && IsSameText(File.ReadAllText(fullPath), content))
                return;

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, content, _utf8WithoutBom);

            changes.Add(isExisting ? $"обновлён {relativePath}" : $"создан {relativePath}");
        }

        private static void DeleteStaleSlices(HashSet<string> sliceFileNames, List<string> changes)
        {
            string levelsFolder = ToFullPath(LEVELS_FOLDER);

            if (Directory.Exists(levelsFolder) == false)
                return;

            foreach (string filePath in Directory.GetFiles(levelsFolder, SLICE_SEARCH_PATTERN))
            {
                string fileName = Path.GetFileName(filePath);

                if (sliceFileNames.Contains(fileName))
                    continue;

                File.Delete(filePath);
                changes.Add($"удалён {LEVELS_FOLDER}/{fileName}");
            }
        }

        // Git может отдать файл с другими переводами строк — их различие изменением не считается.
        private static bool IsSameText(string first, string second)
            => NormalizeLineBreaks(first) == NormalizeLineBreaks(second);

        private static string NormalizeLineBreaks(string text) => text.Replace(WINDOWS_LINE_BREAK, UNIX_LINE_BREAK);
    }
}
