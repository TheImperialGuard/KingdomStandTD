using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Newtonsoft.Json;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Импорт среза в LevelConfig: разбор JSON → враги по имени ассета → дифф с текущими данными →
    // проверка и сравнение статистики на копии конфига в памяти → применение одной Undo-группой и сохранение ассета.
    // Импортируются startGold, startPlayerHealth и stages; справочные блоки среза пропускаются.
    // Окно редактора вставляет уровень из буфера обмена через Preview с разобранным срезом: срез может быть
    // от другого уровня, а применяет его само окно — шагом Undo и без сохранения.
    public static class LevelSliceImporter
    {
        public static LevelSliceImportResult Preview(LevelConfig config, string json) => Run(config, json, isApplying: false);

        public static LevelSliceImportResult Preview(LevelConfig config, LevelSlice slice, LevelSliceEnemies enemies)
        {
            LevelSliceImportResult result = new(config.name);
            Check(config, slice, enemies, result);

            return result;
        }

        public static LevelSliceImportResult Import(LevelConfig config, string json) => Run(config, json, isApplying: true);

        public static string GetUndoName(LevelConfig config) => $"Импорт среза {config.name}";

        private static LevelSliceImportResult Run(LevelConfig config, string json, bool isApplying)
        {
            LevelSliceImportResult result = new(config.name);

            if (TryParse(json, result, out LevelSlice slice) == false)
                return result;

            if (slice.Level != config.name)
            {
                result.Errors.Add($"Срез относится к уровню {slice.Level}, а импорт идёт в {config.name}");
                return result;
            }

            LevelSliceEnemies enemies = new(LevelEditorAssets.FindEnemyConfigs());
            Check(config, slice, enemies, result);

            if (result.HasErrors || result.HasChanges == false || isApplying == false)
                return result;

            Apply(config, slice, enemies.ByName);
            result.IsApplied = true;

            return result;
        }

        // Сначала структура и враги, затем дифф; если данные меняются — проверка и статистика на копии конфига.
        private static void Check(LevelConfig config, LevelSlice slice, LevelSliceEnemies enemies, LevelSliceImportResult result)
        {
            result.Errors.AddRange(LevelSliceChecks.CheckStages(slice.Stages, enemies));

            if (result.HasErrors)
                return;

            LevelSlice currentSlice = LevelSliceSerializer.CreateDataSlice(config);
            result.Changes.AddRange(LevelSliceComparer.Compare(currentSlice, slice));

            if (result.HasChanges)
                CheckOnCopy(config, slice, enemies.ByName, result);
        }

        private static bool TryParse(string json, LevelSliceImportResult result, out LevelSlice slice)
        {
            try
            {
                slice = LevelSliceSerializer.ParseSlice(json);
            }
            catch (JsonException exception)
            {
                slice = null;
                result.Errors.Add($"Ошибка разбора JSON: {exception.Message}");
                return false;
            }

            if (slice == null)
            {
                result.Errors.Add("В файле нет данных среза");
                return false;
            }

            return true;
        }

        // Срез применяется к копии конфига в памяти: копию проверяет тот же валидатор, что и ассеты,
        // а её статистика сравнивается с текущей — так видно, что изменится, ещё до применения.
        private static void CheckOnCopy(
            LevelConfig config,
            LevelSlice slice,
            IReadOnlyDictionary<string, CharacterConfig> enemies,
            LevelSliceImportResult result)
        {
            LevelConfig copy = Object.Instantiate(config);

            try
            {
                using SerializedObject serializedCopy = new(copy);

                LevelSliceApplier.Apply(serializedCopy, slice, enemies);
                serializedCopy.ApplyModifiedPropertiesWithoutUndo();

                LevelPrefabInfo prefab = LevelPrefabReader.Read(config);
                GameConfig gameConfig = LevelEditorAssets.FindGameConfig();

                AddIssues(LevelValidator.Validate(copy, prefab), result);

                LevelStats currentStats = LevelStatsCalculator.Calculate(config, prefab, gameConfig);
                LevelStats newStats = LevelStatsCalculator.Calculate(copy, prefab, gameConfig);
                result.AnalysisChanges.AddRange(LevelStatsComparer.Compare(currentStats, newStats));
            }
            finally
            {
                Object.DestroyImmediate(copy);
            }
        }

        private static void AddIssues(IEnumerable<LevelIssue> issues, LevelSliceImportResult result)
        {
            foreach (LevelIssue issue in issues)
            {
                if (issue.IsError)
                    result.Errors.Add(issue.ToString());
                else
                    result.Warnings.Add(issue.ToString());
            }
        }

        private static void Apply(LevelConfig config, LevelSlice slice, IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            using (SerializedObject serializedConfig = new(config))
            {
                LevelSliceApplier.Apply(serializedConfig, slice, enemies);
                serializedConfig.ApplyModifiedProperties();
            }

            Undo.SetCurrentGroupName(GetUndoName(config));
            Undo.CollapseUndoOperations(undoGroup);

            AssetDatabase.SaveAssetIfDirty(config);
        }
    }
}
