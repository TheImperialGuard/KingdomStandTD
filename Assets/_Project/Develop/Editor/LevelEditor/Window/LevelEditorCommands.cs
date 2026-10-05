using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Структурные правки уровня из окна: новые, дублированные, удалённые и переставленные стейджи и волны,
    // перенос волны в другой стейдж, копирование и вставка через буфер обмена. Каждая правка — шаг Undo.
    // Перед правкой команда просит окно выбрать элемент, который получится в результате, — так выбор и данные
    // обновляются вместе, одним пересчётом окна.
    public class LevelEditorCommands
    {
        private const string PASTE_DIALOG_TITLE = "Вставка из буфера";
        private const string DIALOG_OK_BUTTON = "OK";
        private const string LINE_BREAK = "\n";
        private const int MAX_SHOWN_ERRORS = 15;

        public event Action<int, int> SelectionRequested;

        private readonly EditorWindow _window;

        private LevelEditingSession _session;

        public LevelEditorCommands(EditorWindow window)
        {
            _window = window;
        }

        private bool IsLevelOpen => _session != null && _session.IsAlive;

        public void Bind(LevelEditingSession session) => _session = session;

        public void InsertStage(int stageIndex)
        {
            RequestSelection(stageIndex, 0);
            _session.InsertStage(stageIndex);
        }

        public void DuplicateStage(int stageIndex)
        {
            RequestSelection(stageIndex + 1, 0);
            _session.DuplicateStage(stageIndex);
        }

        // Выбор остаётся на месте — на стейдже, который займёт место удалённого.
        public void RemoveStage(int stageIndex)
        {
            RequestSelection(stageIndex, 0);
            _session.RemoveStage(stageIndex);
        }

        public void MoveStage(int stageIndex, int targetIndex)
        {
            RequestSelection(targetIndex, 0);
            _session.MoveStage(stageIndex, targetIndex);
        }

        public void CopyStage(int stageIndex)
        {
            EnemiesWavesStageConfig stage = _session.Config.Stages[stageIndex];
            StageStats stats = _session.Stats.Stages[stageIndex];

            LevelClipboard.Write(LevelSliceSerializer.CreateStageSlice(stage, stats));

            string address = LevelAddress.Format(stageIndex);
            Notify($"{address} скопирован");
        }

        public void InsertWave(int stageIndex, int waveIndex)
        {
            RequestSelection(stageIndex, waveIndex);
            _session.InsertWave(stageIndex, waveIndex);
        }

        public void DuplicateWave(int stageIndex, int waveIndex)
        {
            RequestSelection(stageIndex, waveIndex + 1);
            _session.DuplicateWave(stageIndex, waveIndex);
        }

        public void RemoveWave(int stageIndex, int waveIndex)
        {
            RequestSelection(stageIndex, waveIndex);
            _session.RemoveWave(stageIndex, waveIndex);
        }

        public void MoveWave(int stageIndex, int waveIndex, int targetIndex)
        {
            RequestSelection(stageIndex, targetIndex);
            _session.MoveWave(stageIndex, waveIndex, targetIndex);
        }

        // Волна уходит в конец другого стейджа, выбор — следом за ней.
        public void MoveWaveToStage(int stageIndex, int waveIndex, int targetStageIndex)
        {
            int targetWaveIndex = _session.GetWavesCount(targetStageIndex);

            RequestSelection(targetStageIndex, targetWaveIndex);
            _session.MoveWaveToStage(stageIndex, waveIndex, targetStageIndex);
        }

        public void CopyWave(int stageIndex, int waveIndex)
        {
            EnemiesWaveConfig wave = LevelStatsCalculator.GetWaves(_session.Config.Stages[stageIndex])[waveIndex];
            WaveStats stats = _session.Stats.Stages[stageIndex].Waves[waveIndex];

            LevelClipboard.Write(LevelSliceSerializer.CreateWaveSlice(wave, stats));

            string address = LevelAddress.Format(stageIndex, waveIndex);
            Notify($"{address} скопирована");
        }

        // Срез уровня с анализом — как в Docs/Balance/Levels, но вместе с несохранёнными правками.
        public void CopyLevel()
        {
            if (IsLevelOpen == false)
                return;

            LevelConfig level = _session.Config;
            string json = LevelSliceExporter.CreateSliceJson(level, LevelEditorAssets.FindGameConfig(), LevelEditorAssets.FindLevelsList());

            LevelClipboard.WriteJson(json);
            Notify($"Уровень {level.name} скопирован");
        }

        // Что и куда вставить, решает содержимое буфера. Стейджи встают после стейджа stageIndex, а без выбранного
        // стейджа — в конец уровня. Волны встают в стейдж stageIndex после волны waveIndex, а без выбранной волны —
        // в конец стейджа. Уровень заменяет данные открытого после предпросмотра изменений.
        public void Paste(int stageIndex, int waveIndex)
        {
            if (IsLevelOpen == false)
                return;

            LevelClipboardContent content = LevelClipboard.Read();

            switch (content.Kind)
            {
                case LevelClipboardContentKind.Level:
                    PasteLevel(content.Level);
                    break;

                case LevelClipboardContentKind.Stages:
                    PasteStages(content.Stages, stageIndex);
                    break;

                case LevelClipboardContentKind.Waves:
                    PasteWaves(content.Waves, stageIndex, waveIndex);
                    break;

                default:
                    ShowPasteError(content.Error);
                    break;
            }
        }

        private void PasteLevel(LevelSlice slice)
        {
            LevelSliceEnemies enemies = new(LevelEditorAssets.FindEnemyConfigs());
            LevelSliceImportResult preview = LevelSliceImporter.Preview(_session.Config, slice, enemies);

            if (preview.HasErrors == false && preview.HasChanges == false)
            {
                Notify("Уровень в буфере совпадает с открытым — изменений нет");
                return;
            }

            if (LevelPasteDialog.Confirm(preview, slice.Level) == false)
                return;

            _session.ApplyLevelSlice(slice, enemies.ByName);
            Notify("Уровень из буфера вставлен, Ctrl+Z отменит вставку");
        }

        private void PasteStages(IReadOnlyList<StageSlice> stages, int stageIndex)
        {
            LevelSliceEnemies enemies = new(LevelEditorAssets.FindEnemyConfigs());
            List<string> errors = LevelSliceChecks.CheckStages(stages, enemies);

            if (errors.Count > 0)
            {
                ShowPasteErrors(errors);
                return;
            }

            int insertIndex = stageIndex == LevelAddress.NO_INDEX ? _session.StagesCount : stageIndex + 1;

            RequestSelection(insertIndex, 0);
            _session.InsertStages(insertIndex, stages, enemies.ByName);
        }

        private void PasteWaves(IReadOnlyList<WaveSlice> waves, int stageIndex, int waveIndex)
        {
            if (stageIndex == LevelAddress.NO_INDEX)
            {
                ShowPasteError("Волны вставляются в выбранный стейдж, а в уровне нет стейджей. Сначала добавьте стейдж.");
                return;
            }

            LevelSliceEnemies enemies = new(LevelEditorAssets.FindEnemyConfigs());
            List<string> errors = LevelSliceChecks.CheckWaves(waves, enemies);

            if (errors.Count > 0)
            {
                ShowPasteErrors(errors);
                return;
            }

            int insertIndex = waveIndex == LevelAddress.NO_INDEX ? _session.GetWavesCount(stageIndex) : waveIndex + 1;

            RequestSelection(stageIndex, insertIndex);
            _session.InsertWaves(stageIndex, insertIndex, waves, enemies.ByName);
        }

        private static void ShowPasteErrors(IReadOnlyList<string> errors)
        {
            string errorsText = string.Join(LINE_BREAK, errors.Take(MAX_SHOWN_ERRORS));
            int hiddenCount = errors.Count - MAX_SHOWN_ERRORS;

            if (hiddenCount > 0)
                errorsText += $"{LINE_BREAK}…и ещё {hiddenCount}";

            ShowPasteError($"Ничего не вставлено: в буфере обмена есть ошибки.{LINE_BREAK}{errorsText}");
        }

        private static void ShowPasteError(string message) => EditorUtility.DisplayDialog(PASTE_DIALOG_TITLE, message, DIALOG_OK_BUTTON);

        private void RequestSelection(int stageIndex, int waveIndex) => SelectionRequested?.Invoke(stageIndex, waveIndex);

        private void Notify(string message) => _window.ShowNotification(new GUIContent(message));
    }
}
