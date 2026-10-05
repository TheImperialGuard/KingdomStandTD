using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using UnityEditor;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Имена сериализованных полей LevelConfig, стейджа, волны и группы врагов —
    // для правки через SerializedObject, который даёт Undo и помечает ассет изменённым.
    public static class LevelConfigFields
    {
        public const string STAGES = "_stages";
        public const string WAVES = "_enemiesWaveConfigs";
        public const string ENEMY_GROUPS = "_enemyGroups";
        public const string NOTE = "_note";

        public static readonly string StartGold = ToBackingField(nameof(LevelConfig.StartGold));
        public static readonly string StartPlayerHealth = ToBackingField(nameof(LevelConfig.StartPlayerHealth));

        public static readonly string StageTime = ToBackingField(nameof(EnemiesWavesStageConfig.StageTime));
        public static readonly string TimeToSkipStage = ToBackingField(nameof(EnemiesWavesStageConfig.TimeToSkipStage));

        public static readonly string PatternRepeats = ToBackingField(nameof(EnemiesWaveConfig.PatternRepeats));
        public static readonly string PathNumber = ToBackingField(nameof(EnemiesWaveConfig.PathNumber));
        public static readonly string Lane = ToBackingField(nameof(EnemiesWaveConfig.Lane));
        public static readonly string StartDelay = ToBackingField(nameof(EnemiesWaveConfig.StartDelay));
        public static readonly string DelayBetweenSpawns = ToBackingField(nameof(EnemiesWaveConfig.DelayBetweenSpawns));

        public static readonly string EnemyConfig = ToBackingField(nameof(WaveEnemyGroup.EnemyConfig));
        public static readonly string Count = ToBackingField(nameof(WaveEnemyGroup.Count));

        public static SerializedProperty FindRequired(SerializedObject owner, string fieldName)
        {
            SerializedProperty property = owner.FindProperty(fieldName);

            if (property == null)
            {
                string ownerName = owner.targetObject.name;
                throw new InvalidOperationException($"У {ownerName} нет сериализованного поля {fieldName}");
            }

            return property;
        }

        public static SerializedProperty FindRequired(SerializedProperty owner, string fieldName)
        {
            SerializedProperty property = owner.FindPropertyRelative(fieldName);

            if (property == null)
            {
                string ownerPath = owner.propertyPath;
                throw new InvalidOperationException($"У {ownerPath} нет сериализованного поля {fieldName}");
            }

            return property;
        }

        private static string ToBackingField(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}
