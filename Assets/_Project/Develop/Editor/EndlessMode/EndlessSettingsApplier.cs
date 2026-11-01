using System.Collections.Generic;
using Assets._Project.Develop.Editor.LevelEditor;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using UnityEditor;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Записывает блок endless среза в SerializedObject EndlessModeConfig. Применяет изменения вызывающий код:
    // ApplyModifiedProperties — с Undo, ApplyModifiedPropertiesWithoutUndo — для копии в памяти. Враги должны быть уже
    // проверены: имя каждого есть в словаре enemies и это MeleeConfig. Описание, уровень и погода режима не трогаются.
    public static class EndlessSettingsApplier
    {
        private const string Seed = "_seed";
        private const string EnemiesCount = "_enemiesCount";
        private const string Tiers = "_tiers";
        private const string DelayBetweenSpawns = "_delayBetweenSpawns";
        private const string RestTime = "_restTime";
        private const string BossWavesStep = "_bossWavesStep";
        private const string Bosses = "_bosses";
        private const string BossWaveEnemiesShare = "_bossWaveEnemiesShare";
        private const string HealthMultiplier = "_healthMultiplier";
        private const string MaxEnemiesToSkipWave = "_maxEnemiesToSkipWave";

        private const string TableKeys = "_keys";
        private const string TierName = "_name";
        private const string TierEnemies = "_enemies";

        private static readonly string _tableTailMode = ToBackingField(nameof(WaveValueTable.TailMode));
        private static readonly string _tableTailStep = ToBackingField(nameof(WaveValueTable.TailStep));
        private static readonly string _keyWave = ToBackingField(nameof(WaveValueKey.Wave));
        private static readonly string _keyValue = ToBackingField(nameof(WaveValueKey.Value));
        private static readonly string _tierShare = ToBackingField(nameof(EndlessTier.Share));
        private static readonly string _tierKindsPerWave = ToBackingField(nameof(EndlessTier.KindsPerWave));
        private static readonly string _tierEnemyConfig = ToBackingField(nameof(EndlessTierEnemy.EnemyConfig));
        private static readonly string _tierEnemyFirstWave = ToBackingField(nameof(EndlessTierEnemy.FirstWave));

        public static void Apply(
            SerializedObject config,
            EndlessSettingsSlice settings,
            IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            LevelSliceApplier.SetInt(Find(config, Seed), settings.Seed);
            ApplyTable(Find(config, EnemiesCount), settings.EnemiesCount);
            ApplyTiers(Find(config, Tiers), settings.Tiers, enemies);
            ApplyTable(Find(config, DelayBetweenSpawns), settings.DelayBetweenSpawns);
            ApplyTable(Find(config, RestTime), settings.RestTime);
            LevelSliceApplier.SetInt(Find(config, BossWavesStep), settings.BossWavesStep);
            ApplyEnemies(Find(config, Bosses), settings.Bosses, enemies);
            LevelSliceApplier.SetFloat(Find(config, BossWaveEnemiesShare), settings.BossWaveEnemiesShare);
            ApplyTable(Find(config, HealthMultiplier), settings.HealthMultiplier);
            LevelSliceApplier.SetInt(Find(config, MaxEnemiesToSkipWave), settings.MaxEnemiesToSkipWave);
        }

        private static void ApplyTable(SerializedProperty table, WaveValueTableSlice tableSlice)
        {
            SerializedProperty keys = Find(table, TableKeys);
            keys.arraySize = tableSlice.Keys.Count;

            for (int i = 0; i < tableSlice.Keys.Count; i++)
            {
                SerializedProperty key = keys.GetArrayElementAtIndex(i);

                LevelSliceApplier.SetInt(Find(key, _keyWave), tableSlice.Keys[i].Wave);
                LevelSliceApplier.SetFloat(Find(key, _keyValue), tableSlice.Keys[i].Value);
            }

            LevelSliceApplier.SetInt(Find(table, _tableTailMode), (int)tableSlice.TailMode);
            LevelSliceApplier.SetFloat(Find(table, _tableTailStep), tableSlice.TailStep);
        }

        private static void ApplyTiers(
            SerializedProperty tiers,
            IReadOnlyList<EndlessTierSlice> tierSlices,
            IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            tiers.arraySize = tierSlices.Count;

            for (int i = 0; i < tierSlices.Count; i++)
            {
                SerializedProperty tier = tiers.GetArrayElementAtIndex(i);
                EndlessTierSlice tierSlice = tierSlices[i];

                LevelSliceApplier.SetString(Find(tier, TierName), tierSlice.Name);
                LevelSliceApplier.SetInt(Find(tier, _tierKindsPerWave), tierSlice.KindsPerWave);
                ApplyTable(Find(tier, _tierShare), tierSlice.Share);
                ApplyTierEnemies(Find(tier, TierEnemies), tierSlice.Enemies, enemies);
            }
        }

        private static void ApplyTierEnemies(
            SerializedProperty tierEnemies,
            IReadOnlyList<EndlessTierEnemySlice> enemySlices,
            IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            tierEnemies.arraySize = enemySlices.Count;

            for (int i = 0; i < enemySlices.Count; i++)
            {
                SerializedProperty tierEnemy = tierEnemies.GetArrayElementAtIndex(i);

                LevelSliceApplier.SetObject(Find(tierEnemy, _tierEnemyConfig), FindEnemy(enemies, enemySlices[i].Enemy));
                LevelSliceApplier.SetInt(Find(tierEnemy, _tierEnemyFirstWave), enemySlices[i].FirstWave);
            }
        }

        private static void ApplyEnemies(
            SerializedProperty list,
            IReadOnlyList<string> enemyNames,
            IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            list.arraySize = enemyNames.Count;

            for (int i = 0; i < enemyNames.Count; i++)
                LevelSliceApplier.SetObject(list.GetArrayElementAtIndex(i), FindEnemy(enemies, enemyNames[i]));
        }

        // null в срезе — пустой элемент списка
        private static CharacterConfig FindEnemy(IReadOnlyDictionary<string, CharacterConfig> enemies, string enemyName)
            => enemyName != null ? enemies[enemyName] : null;

        private static SerializedProperty Find(SerializedObject owner, string fieldName)
            => LevelConfigFields.FindRequired(owner, fieldName);

        private static SerializedProperty Find(SerializedProperty owner, string fieldName)
            => LevelConfigFields.FindRequired(owner, fieldName);

        private static string ToBackingField(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}
