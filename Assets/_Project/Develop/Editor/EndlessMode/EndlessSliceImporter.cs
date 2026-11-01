using System.Collections.Generic;
using Assets._Project.Develop.Editor.LevelEditor;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Newtonsoft.Json;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Импорт среза бесконечного уровня: startGold и startPlayerHealth — в LevelConfig уровня, блок endless — в
    // EndlessModeConfig режима. Волны среза не импортируются: их собирает генератор. Порядок тот же, что у
    // LevelSliceImporter: разбор → враги по имени → дифф → проверка и сравнение статистики выгружаемых волн на копиях
    // конфигов в памяти → применение одной Undo-группой и сохранение обоих ассетов.
    public static class EndlessSliceImporter
    {
        private const int MinWaveNumber = 1;
        private const int MinKindsPerWave = 1;
        private const int MinBossWavesStep = 1;
        private const int MinEnemiesToSkipWave = 0;
        private const float MinShare = 0f;
        private const float MaxShare = 1f;
        private const string HealthWithMultiplierLabel = "HP с множителем";

        public static LevelSliceImportResult Preview(LevelConfig level, string json) => Run(level, json, isApplying: false);

        public static LevelSliceImportResult Import(LevelConfig level, string json) => Run(level, json, isApplying: true);

        private static LevelSliceImportResult Run(LevelConfig level, string json, bool isApplying)
        {
            LevelSliceImportResult result = new(level.name);
            EndlessModeConfig config = LevelEditorAssets.FindEndlessModeConfig(level);

            if (config == null)
            {
                result.Errors.Add($"Уровень {level.name} не задан ни в одном EndlessModeConfig: настройки режима импортировать некуда");
                return result;
            }

            if (TryParse(json, level, result, out LevelSlice slice, out EndlessSettingsSlice settings) == false)
                return result;

            LevelSliceEnemies enemies = new(LevelEditorAssets.FindEnemyConfigs());
            CheckSettings(settings, enemies, result);

            if (result.HasErrors)
                return result;

            LevelSlice currentEconomy = CreateEconomySlice(level.name, level.StartGold, level.StartPlayerHealth);
            LevelSlice newEconomy = CreateEconomySlice(level.name, slice.StartGold, slice.StartPlayerHealth);

            result.Changes.AddRange(LevelSliceComparer.Compare(currentEconomy, newEconomy));
            result.Changes.AddRange(EndlessSettingsComparer.Compare(EndlessSliceSerializer.CreateSettings(config), settings));

            if (result.HasChanges)
                CheckOnCopies(level, config, slice, settings, enemies.ByName, result);

            if (result.HasErrors || result.HasChanges == false || isApplying == false)
                return result;

            Apply(level, config, slice, settings, enemies.ByName);
            result.IsApplied = true;

            return result;
        }

        private static bool TryParse(
            string json,
            LevelConfig level,
            LevelSliceImportResult result,
            out LevelSlice slice,
            out EndlessSettingsSlice settings)
        {
            settings = null;

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

            if (slice.Level != level.name)
            {
                result.Errors.Add($"Срез относится к уровню {slice.Level}, а импорт идёт в {level.name}");
                return false;
            }

            if (slice.Endless == null)
            {
                result.Errors.Add("В срезе нет блока endless — настроек режима");
                return false;
            }

            try
            {
                settings = LevelSliceSerializer.FromToken<EndlessSettingsSlice>(slice.Endless);
            }
            catch (JsonException exception)
            {
                result.Errors.Add($"Ошибка разбора блока endless: {exception.Message}");
                return false;
            }

            return true;
        }

        // Структуру таблиц (пустые, ключи не по возрастанию) проверяет валидатор режима на копии; здесь — то, что
        // инспектор не дал бы ввести, и враги по имени
        private static void CheckSettings(EndlessSettingsSlice settings, LevelSliceEnemies enemies, LevelSliceImportResult result)
        {
            CheckTable(settings.EnemiesCount, "enemiesCount", result);
            CheckTable(settings.DelayBetweenSpawns, "delayBetweenSpawns", result);
            CheckTable(settings.RestTime, "restTime", result);
            CheckTable(settings.HealthMultiplier, "healthMultiplier", result);

            for (int i = 0; i < settings.Tiers.Count; i++)
                CheckTier(settings.Tiers[i], $"tiers[{i}]", enemies, result);

            for (int i = 0; i < settings.Bosses.Count; i++)
                CheckEnemy(settings.Bosses[i], $"bosses[{i}]", enemies, result);

            if (settings.BossWavesStep < MinBossWavesStep)
                result.Errors.Add($"bossWavesStep: {settings.BossWavesStep}, нужно не меньше {MinBossWavesStep}");

            if (settings.BossWaveEnemiesShare < MinShare || settings.BossWaveEnemiesShare > MaxShare)
                result.Errors.Add($"bossWaveEnemiesShare: {settings.BossWaveEnemiesShare}, нужно от {MinShare} до {MaxShare}");

            if (settings.MaxEnemiesToSkipWave < MinEnemiesToSkipWave)
                result.Errors.Add($"maxEnemiesToSkipWave: {settings.MaxEnemiesToSkipWave}, нужно не меньше {MinEnemiesToSkipWave}");
        }

        private static void CheckTier(EndlessTierSlice tier, string path, LevelSliceEnemies enemies, LevelSliceImportResult result)
        {
            if (tier == null)
            {
                result.Errors.Add($"{path}: пустой тир");
                return;
            }

            CheckTable(tier.Share, $"{path}.share", result);

            if (tier.KindsPerWave < MinKindsPerWave)
                result.Errors.Add($"{path}.kindsPerWave: {tier.KindsPerWave}, нужно не меньше {MinKindsPerWave}");

            for (int i = 0; i < tier.Enemies.Count; i++)
            {
                EndlessTierEnemySlice tierEnemy = tier.Enemies[i];
                string enemyPath = $"{path}.enemies[{i}]";

                if (tierEnemy == null)
                {
                    result.Errors.Add($"{enemyPath}: пустой элемент");
                    continue;
                }

                CheckEnemy(tierEnemy.Enemy, enemyPath, enemies, result);

                if (tierEnemy.FirstWave < MinWaveNumber)
                    result.Errors.Add($"{enemyPath}.firstWave: {tierEnemy.FirstWave}, нужно не меньше {MinWaveNumber}");
            }
        }

        private static void CheckTable(WaveValueTableSlice table, string path, LevelSliceImportResult result)
        {
            for (int i = 0; i < table.Keys.Count; i++)
            {
                WaveValueKeySlice key = table.Keys[i];

                if (key == null)
                    result.Errors.Add($"{path}.keys[{i}]: пустой ключ");
                else if (key.Wave < MinWaveNumber)
                    result.Errors.Add($"{path}.keys[{i}].wave: {key.Wave}, нужно не меньше {MinWaveNumber}");
            }
        }

        // null — пустой элемент списка, его допускает и инспектор; генератор такой элемент пропускает
        private static void CheckEnemy(string enemyName, string path, LevelSliceEnemies enemies, LevelSliceImportResult result)
        {
            if (enemyName == null)
                return;

            string problem = enemies.FindProblem(enemyName);

            if (problem != null)
            {
                result.Errors.Add($"{path}: {problem}");
                return;
            }

            if (enemies.ByName[enemyName] is MeleeConfig == false)
                result.Errors.Add($"{path}: {enemyName} — не MeleeConfig, режим не может его создать");
        }

        // Срез применяется к копиям конфигов в памяти: копию режима проверяет его валидатор, а статистика выгружаемых волн
        // сравнивается с текущей — так видно, что изменится, ещё до применения.
        private static void CheckOnCopies(
            LevelConfig level,
            EndlessModeConfig config,
            LevelSlice slice,
            EndlessSettingsSlice settings,
            IReadOnlyDictionary<string, CharacterConfig> enemies,
            LevelSliceImportResult result)
        {
            LevelConfig levelCopy = Object.Instantiate(level);
            EndlessModeConfig configCopy = Object.Instantiate(config);

            levelCopy.name = level.name;
            configCopy.name = config.name;

            try
            {
                using (SerializedObject serializedLevel = new(levelCopy))
                {
                    LevelSliceApplier.ApplyEconomy(serializedLevel, slice);
                    serializedLevel.ApplyModifiedPropertiesWithoutUndo();
                }

                using (SerializedObject serializedConfig = new(configCopy))
                {
                    EndlessSettingsApplier.Apply(serializedConfig, settings, enemies);
                    serializedConfig.ApplyModifiedPropertiesWithoutUndo();
                }

                LevelPrefabInfo prefab = LevelPrefabReader.Read(level);
                List<LevelIssue> issues = LevelValidator.Validate(levelCopy, prefab);

                bool canGenerate = EndlessEditorWaves.TryCreateGenerator(configCopy, issues, out EndlessWavesGenerator newGenerator);

                AddIssues(issues, result);

                if (canGenerate == false)
                    return;

                GameConfig gameConfig = LevelEditorAssets.FindGameConfig();
                LevelStats currentStats = CalculateStats(level, config, prefab, gameConfig);
                LevelStats newStats = CalculateStats(levelCopy, newGenerator, prefab, gameConfig);

                // Множитель здоровья в статистике не виден: она считает здоровье врагов по их конфигам
                List<float> currentHealths = EndlessEditorWaves.CalculateHealthWithMultiplier(currentStats, config);
                List<float> newHealths = EndlessEditorWaves.CalculateHealthWithMultiplier(newStats, configCopy);

                result.AnalysisChanges.AddRange(LevelStatsComparer.Compare(currentStats, newStats));
                result.AnalysisChanges.AddRange(LevelStatsComparer.CompareStageValues(HealthWithMultiplierLabel, currentHealths, newHealths));
            }
            finally
            {
                Object.DestroyImmediate(levelCopy);
                Object.DestroyImmediate(configCopy);
            }
        }

        // Текущий конфиг мог быть с ошибками — тогда волн до правки нет, и сравнение идёт с пустой статистикой
        private static LevelStats CalculateStats(LevelConfig level, EndlessModeConfig config, LevelPrefabInfo prefab, GameConfig gameConfig)
        {
            if (EndlessEditorWaves.TryCreateGenerator(config, new List<LevelIssue>(), out EndlessWavesGenerator generator) == false)
                return new LevelStats();

            return CalculateStats(level, generator, prefab, gameConfig);
        }

        private static LevelStats CalculateStats(
            LevelConfig level,
            EndlessWavesGenerator generator,
            LevelPrefabInfo prefab,
            GameConfig gameConfig)
        {
            List<EnemiesWavesStageConfig> waves = EndlessEditorWaves.Generate(generator, EndlessSliceSerializer.SliceWavesCount);

            return EndlessEditorWaves.CalculateStats(level, waves, prefab, gameConfig);
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

        private static LevelSlice CreateEconomySlice(string levelName, int startGold, int startPlayerHealth)
        {
            return new LevelSlice
            {
                Level = levelName,
                StartGold = startGold,
                StartPlayerHealth = startPlayerHealth,
            };
        }

        private static void Apply(
            LevelConfig level,
            EndlessModeConfig config,
            LevelSlice slice,
            EndlessSettingsSlice settings,
            IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            using (SerializedObject serializedLevel = new(level))
            {
                LevelSliceApplier.ApplyEconomy(serializedLevel, slice);
                serializedLevel.ApplyModifiedProperties();
            }

            using (SerializedObject serializedConfig = new(config))
            {
                EndlessSettingsApplier.Apply(serializedConfig, settings, enemies);
                serializedConfig.ApplyModifiedProperties();
            }

            Undo.SetCurrentGroupName(LevelSliceImporter.GetUndoName(level));
            Undo.CollapseUndoOperations(undoGroup);

            AssetDatabase.SaveAssetIfDirty(level);
            AssetDatabase.SaveAssetIfDirty(config);
        }
    }
}
