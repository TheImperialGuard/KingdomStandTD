using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using UnityEditor;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Записывает данные среза в SerializedObject LevelConfig. Применяет изменения вызывающий код:
    // ApplyModifiedProperties — с Undo, ApplyModifiedPropertiesWithoutUndo — для копии в памяти.
    // Враги в срезе должны быть уже проверены (LevelSliceChecks): имя каждого есть в словаре enemies.
    // Отдельный стейдж или волну окно редактора записывает в только что вставленный элемент списка.
    public static class LevelSliceApplier
    {
        public static void Apply(SerializedObject config, LevelSlice slice, IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            SetInt(LevelConfigFields.FindRequired(config, LevelConfigFields.StartGold), slice.StartGold);
            SetInt(LevelConfigFields.FindRequired(config, LevelConfigFields.StartPlayerHealth), slice.StartPlayerHealth);

            SerializedProperty stages = LevelConfigFields.FindRequired(config, LevelConfigFields.STAGES);
            stages.arraySize = slice.Stages.Count;

            for (int i = 0; i < slice.Stages.Count; i++)
                ApplyStage(stages.GetArrayElementAtIndex(i), slice.Stages[i], enemies);
        }

        public static void ApplyStage(SerializedProperty stage, StageSlice stageSlice, IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            SetFloat(LevelConfigFields.FindRequired(stage, LevelConfigFields.StageTime), stageSlice.StageTime);
            SetFloat(LevelConfigFields.FindRequired(stage, LevelConfigFields.TimeToSkipStage), stageSlice.TimeToSkipStage);
            SetString(LevelConfigFields.FindRequired(stage, LevelConfigFields.NOTE), stageSlice.Note);

            SerializedProperty waves = LevelConfigFields.FindRequired(stage, LevelConfigFields.WAVES);
            waves.arraySize = stageSlice.Waves.Count;

            for (int i = 0; i < stageSlice.Waves.Count; i++)
                ApplyWave(waves.GetArrayElementAtIndex(i), stageSlice.Waves[i], enemies);
        }

        public static void ApplyWave(SerializedProperty wave, WaveSlice waveSlice, IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            SerializedProperty groups = LevelConfigFields.FindRequired(wave, LevelConfigFields.ENEMY_GROUPS);
            groups.arraySize = waveSlice.Groups.Count;

            for (int i = 0; i < waveSlice.Groups.Count; i++)
            {
                SerializedProperty group = groups.GetArrayElementAtIndex(i);
                EnemyGroupSlice groupSlice = waveSlice.Groups[i];

                SetObject(LevelConfigFields.FindRequired(group, LevelConfigFields.EnemyConfig), enemies[groupSlice.Enemy]);
                SetInt(LevelConfigFields.FindRequired(group, LevelConfigFields.Count), groupSlice.Count);
            }

            SetInt(LevelConfigFields.FindRequired(wave, LevelConfigFields.PatternRepeats), waveSlice.PatternRepeats);
            SetInt(LevelConfigFields.FindRequired(wave, LevelConfigFields.PathNumber), (int)waveSlice.PathNumber);
            SetInt(LevelConfigFields.FindRequired(wave, LevelConfigFields.Lane), (int)waveSlice.Lane);
            SetFloat(LevelConfigFields.FindRequired(wave, LevelConfigFields.StartDelay), waveSlice.StartDelay);
            SetFloat(LevelConfigFields.FindRequired(wave, LevelConfigFields.DelayBetweenSpawns), waveSlice.DelayBetweenSpawns);
            SetString(LevelConfigFields.FindRequired(wave, LevelConfigFields.NOTE), waveSlice.Note);
        }

        // Для enum intValue — числовое значение элемента, а не его индекс в списке имён.
        private static void SetInt(SerializedProperty property, int value)
        {
            if (property.intValue != value)
                property.intValue = value;
        }

        // Значение, совпадающее после округления до трёх знаков, не перезаписывается:
        // в срезе числа округлены, а в ассете могут быть точнее.
        private static void SetFloat(SerializedProperty property, float value)
        {
            if (LevelSliceNumbers.IsSame(property.floatValue, value) == false)
                property.floatValue = value;
        }

        private static void SetString(SerializedProperty property, string value)
        {
            string newValue = value ?? string.Empty;

            if (property.stringValue != newValue)
                property.stringValue = newValue;
        }

        private static void SetObject(SerializedProperty property, CharacterConfig value)
        {
            if (property.objectReferenceValue != value)
                property.objectReferenceValue = value;
        }
    }
}
