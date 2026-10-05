using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Правка одного LevelConfig из окна. Все изменения идут через SerializedObject, поэтому каждое — отдельный шаг Undo.
    // После любого изменения — своего, через Undo или со стороны (инспектор, импорт среза) — пересчитываются
    // статистика и замечания, а окно получает событие Changed. Изменения других ассетов (враги, префаб уровня)
    // окно передаёт через Reload.
    public class LevelEditingSession : IDisposable
    {
        public event Action Changed;

        private readonly SerializedObject _serializedConfig;
        private readonly GameConfig _gameConfig;

        private int _knownDirtyCount;

        public LevelEditingSession(LevelConfig config)
        {
            Config = config;
            _serializedConfig = new SerializedObject(config);
            _gameConfig = LevelEditorAssets.FindGameConfig();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            Recalculate();
        }

        public LevelConfig Config { get; }
        public LevelPrefabInfo Prefab { get; private set; }
        public LevelStats Stats { get; private set; }
        public IReadOnlyList<LevelIssue> Issues { get; private set; }

        public bool IsAlive => Config != null;
        public bool IsDirty => IsAlive && EditorUtility.IsDirty(Config);
        public int StagesCount => Config.Stages.Count;

        public int GetWavesCount(int stageIndex) => LevelStatsCalculator.GetWaves(Config.Stages[stageIndex]).Count;

        // Правку со стороны видно по счётчику пометок ассета изменённым: окно сверяет его несколько раз в секунду.
        public void ReloadIfChangedOutside()
        {
            if (IsAlive && EditorUtility.GetDirtyCount(Config) != _knownDirtyCount)
                Reload();
        }

        public void Reload()
        {
            if (IsAlive == false)
                return;

            _serializedConfig.Update();
            Recalculate();
            Changed?.Invoke();
        }

        public void Save()
        {
            if (IsDirty)
                AssetDatabase.SaveAssetIfDirty(Config);

            Changed?.Invoke();
        }

        // Возвращает уровень к сохранённому на диске состоянию: файл читается заново и копируется в открытый конфиг.
        // Это один шаг Undo, так что Ctrl+Z вернёт сброшенные правки.
        public void RevertToSaved()
        {
            if (IsDirty == false)
                return;

            string assetPath = AssetDatabase.GetAssetPath(Config);
            Object[] savedObjects = InternalEditorUtility.LoadSerializedFileAndForget(assetPath);
            LevelConfig savedConfig = savedObjects.OfType<LevelConfig>().FirstOrDefault();

            try
            {
                if (savedConfig == null)
                {
                    Debug.LogError($"Не удалось прочитать сохранённый уровень: {assetPath}");
                    return;
                }

                Undo.RecordObject(Config, "Сбросить изменения уровня");
                EditorUtility.CopySerialized(savedConfig, Config);
                Undo.FlushUndoRecordObjects();
                EditorUtility.ClearDirty(Config);
            }
            finally
            {
                foreach (Object savedObject in savedObjects)
                    Object.DestroyImmediate(savedObject);
            }

            _serializedConfig.Update();
            Recalculate();
            Changed?.Invoke();
        }

        public void SetStartGold(int value)
        {
            int gold = Math.Max(LevelConfigLimits.MIN_START_GOLD, value);
            Modify("Стартовое золото", () => FindLevelField(LevelConfigFields.StartGold).intValue = gold);
        }

        public void SetStartPlayerHealth(int value)
        {
            int health = Math.Max(LevelConfigLimits.MIN_PLAYER_HEALTH, value);
            Modify("Здоровье игрока", () => FindLevelField(LevelConfigFields.StartPlayerHealth).intValue = health);
        }

        public void SetStageTime(int stageIndex, float value)
        {
            float stageTime = Mathf.Max(LevelConfigLimits.MIN_STAGE_TIME, value);
            Modify("Время стейджа", () => FindField(FindStage(stageIndex), LevelConfigFields.StageTime).floatValue = stageTime);
        }

        public void SetTimeToSkipStage(int stageIndex, float value)
        {
            float skipTime = Mathf.Max(LevelConfigLimits.MIN_TIME_TO_SKIP_STAGE, value);
            Modify("Время до скипа", () => FindField(FindStage(stageIndex), LevelConfigFields.TimeToSkipStage).floatValue = skipTime);
        }

        public void SetStageNote(int stageIndex, string value)
            => Modify("Заметка стейджа", () => FindField(FindStage(stageIndex), LevelConfigFields.NOTE).stringValue = value ?? string.Empty);

        public void SetWavePathNumber(int stageIndex, int waveIndex, PathNumber value)
            => Modify("Дорога волны", () => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.PathNumber).intValue = (int)value);

        public void SetWaveLane(int stageIndex, int waveIndex, PathLane value)
            => Modify("Полоса волны", () => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.Lane).intValue = (int)value);

        public void SetWaveStartDelay(int stageIndex, int waveIndex, float value)
        {
            float startDelay = Mathf.Max(LevelConfigLimits.MIN_START_DELAY, value);
            Modify("Старт волны", () => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.StartDelay).floatValue = startDelay);
        }

        public void SetWaveDelayBetweenSpawns(int stageIndex, int waveIndex, float value)
        {
            float delay = Mathf.Max(LevelConfigLimits.MIN_DELAY_BETWEEN_SPAWNS, value);
            Modify("Интервал спавна", () => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.DelayBetweenSpawns).floatValue = delay);
        }

        public void SetWavePatternRepeats(int stageIndex, int waveIndex, int value)
        {
            int repeats = Math.Max(LevelConfigLimits.MIN_PATTERN_REPEATS, value);
            Modify("Повторы шаблона", () => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.PatternRepeats).intValue = repeats);
        }

        public void SetWaveNote(int stageIndex, int waveIndex, string value)
            => Modify("Заметка волны", () => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.NOTE).stringValue = value ?? string.Empty);

        public void SetGroupEnemy(int stageIndex, int waveIndex, int groupIndex, CharacterConfig enemy)
        {
            Modify("Враг группы", () =>
            {
                SerializedProperty group = FindGroups(stageIndex, waveIndex).GetArrayElementAtIndex(groupIndex);
                FindField(group, LevelConfigFields.EnemyConfig).objectReferenceValue = enemy;
            });
        }

        public void SetGroupCount(int stageIndex, int waveIndex, int groupIndex, int value)
        {
            int count = Math.Max(LevelConfigLimits.MIN_GROUP_COUNT, value);

            Modify("Количество в группе", () =>
            {
                SerializedProperty group = FindGroups(stageIndex, waveIndex).GetArrayElementAtIndex(groupIndex);
                FindField(group, LevelConfigFields.Count).intValue = count;
            });
        }

        // Новая группа копирует врага последней группы, количество — минимальное.
        public void AddGroup(int stageIndex, int waveIndex)
        {
            Modify("Добавить группу врагов", () =>
            {
                SerializedProperty groups = FindGroups(stageIndex, waveIndex);
                int newIndex = groups.arraySize;

                groups.InsertArrayElementAtIndex(newIndex);

                SerializedProperty group = groups.GetArrayElementAtIndex(newIndex);
                FindField(group, LevelConfigFields.Count).intValue = LevelConfigLimits.MIN_GROUP_COUNT;
            });
        }

        public void RemoveGroup(int stageIndex, int waveIndex, int groupIndex)
            => Modify("Удалить группу врагов", () => FindGroups(stageIndex, waveIndex).DeleteArrayElementAtIndex(groupIndex));

        public void MoveGroup(int stageIndex, int waveIndex, int groupIndex, int targetIndex)
            => Modify("Переставить группу врагов", () => FindGroups(stageIndex, waveIndex).MoveArrayElement(groupIndex, targetIndex));

        // Новый стейдж — со значениями по умолчанию из EnemiesWavesStageConfig и без волн.
        public void InsertStage(int stageIndex)
            => Modify("Добавить стейдж", () => InsertDefault(FindStages(), stageIndex, new EnemiesWavesStageConfig()));

        // Копия встаёт сразу после исходного стейджа.
        public void DuplicateStage(int stageIndex)
            => Modify("Дублировать стейдж", () => FindStages().InsertArrayElementAtIndex(stageIndex));

        public void RemoveStage(int stageIndex)
            => Modify("Удалить стейдж", () => FindStages().DeleteArrayElementAtIndex(stageIndex));

        public void MoveStage(int stageIndex, int targetIndex)
            => Modify("Переставить стейдж", () => FindStages().MoveArrayElement(stageIndex, targetIndex));

        // Стейджи из буфера обмена встают подряд, начиная с stageIndex. Враги в них должны быть уже проверены.
        public void InsertStages(int stageIndex, IReadOnlyList<StageSlice> stages, IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            string undoName = stages.Count == 1 ? "Вставить стейдж из буфера" : "Вставить стейджи из буфера";

            Modify(undoName, () =>
            {
                SerializedProperty stagesProperty = FindStages();

                for (int i = 0; i < stages.Count; i++)
                {
                    SerializedProperty stage = InsertDefault(stagesProperty, stageIndex + i, new EnemiesWavesStageConfig());
                    LevelSliceApplier.ApplyStage(stage, stages[i], enemies);
                }
            });
        }

        // Новая волна — со значениями по умолчанию из EnemiesWaveConfig и пустым шаблоном.
        public void InsertWave(int stageIndex, int waveIndex)
            => Modify("Добавить волну", () => InsertDefault(FindWaves(stageIndex), waveIndex, new EnemiesWaveConfig()));

        // Копия встаёт сразу после исходной волны.
        public void DuplicateWave(int stageIndex, int waveIndex)
            => Modify("Дублировать волну", () => FindWaves(stageIndex).InsertArrayElementAtIndex(waveIndex));

        public void RemoveWave(int stageIndex, int waveIndex)
            => Modify("Удалить волну", () => FindWaves(stageIndex).DeleteArrayElementAtIndex(waveIndex));

        public void MoveWave(int stageIndex, int waveIndex, int targetIndex)
            => Modify("Переставить волну", () => FindWaves(stageIndex).MoveArrayElement(waveIndex, targetIndex));

        // Волна переносится в конец другого стейджа целиком: шаблон, дорога, полоса, тайминги и заметка.
        public void MoveWaveToStage(int stageIndex, int waveIndex, int targetStageIndex)
        {
            int targetStageNumber = targetStageIndex + 1;

            Modify($"Перенести волну в стейдж {targetStageNumber}", () =>
            {
                object wave = FindWave(stageIndex, waveIndex).boxedValue;
                SerializedProperty targetWaves = FindWaves(targetStageIndex);
                int targetIndex = targetWaves.arraySize;

                targetWaves.InsertArrayElementAtIndex(targetIndex);
                targetWaves.GetArrayElementAtIndex(targetIndex).boxedValue = wave;

                FindWaves(stageIndex).DeleteArrayElementAtIndex(waveIndex);
            });
        }

        // Волны из буфера обмена встают подряд, начиная с waveIndex. Враги в них должны быть уже проверены.
        public void InsertWaves(int stageIndex, int waveIndex, IReadOnlyList<WaveSlice> waves, IReadOnlyDictionary<string, CharacterConfig> enemies)
        {
            string undoName = waves.Count == 1 ? "Вставить волну из буфера" : "Вставить волны из буфера";

            Modify(undoName, () =>
            {
                SerializedProperty wavesProperty = FindWaves(stageIndex);

                for (int i = 0; i < waves.Count; i++)
                {
                    SerializedProperty wave = InsertDefault(wavesProperty, waveIndex + i, new EnemiesWaveConfig());
                    LevelSliceApplier.ApplyWave(wave, waves[i], enemies);
                }
            });
        }

        // Весь уровень из среза — одним шагом Undo и без сохранения: вставка из буфера обмена и массовые операции.
        // Срез должен быть уже проверен.
        public void ApplyLevelSlice(LevelSlice slice, IReadOnlyDictionary<string, CharacterConfig> enemies, string undoName)
            => Modify(undoName, () => LevelSliceApplier.Apply(_serializedConfig, slice, enemies));

        public void Dispose()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            _serializedConfig.Dispose();
        }

        private void Modify(string undoName, Action change)
        {
            if (IsAlive == false)
                return;

            _serializedConfig.Update();
            change();

            if (_serializedConfig.ApplyModifiedProperties())
                Undo.SetCurrentGroupName(undoName);

            Recalculate();
            Changed?.Invoke();
        }

        private void Recalculate()
        {
            Prefab = LevelPrefabReader.Read(Config);
            Stats = LevelStatsCalculator.Calculate(Config, Prefab, _gameConfig);
            Issues = LevelValidator.Validate(Config, Prefab);

            _knownDirtyCount = EditorUtility.GetDirtyCount(Config);
        }

        // Элемент вставляется копией соседа, поэтому сразу получает значения по умолчанию целиком.
        private static SerializedProperty InsertDefault(SerializedProperty array, int index, object defaultValue)
        {
            array.InsertArrayElementAtIndex(index);

            SerializedProperty element = array.GetArrayElementAtIndex(index);
            element.boxedValue = defaultValue;

            return element;
        }

        private SerializedProperty FindLevelField(string fieldName) => LevelConfigFields.FindRequired(_serializedConfig, fieldName);

        private SerializedProperty FindField(SerializedProperty owner, string fieldName) => LevelConfigFields.FindRequired(owner, fieldName);

        private SerializedProperty FindStages() => FindLevelField(LevelConfigFields.STAGES);

        private SerializedProperty FindStage(int stageIndex) => FindStages().GetArrayElementAtIndex(stageIndex);

        private SerializedProperty FindWaves(int stageIndex) => FindField(FindStage(stageIndex), LevelConfigFields.WAVES);

        private SerializedProperty FindWave(int stageIndex, int waveIndex) => FindWaves(stageIndex).GetArrayElementAtIndex(waveIndex);

        private SerializedProperty FindGroups(int stageIndex, int waveIndex)
            => FindField(FindWave(stageIndex, waveIndex), LevelConfigFields.ENEMY_GROUPS);

        private void OnUndoRedoPerformed() => Reload();
    }
}
