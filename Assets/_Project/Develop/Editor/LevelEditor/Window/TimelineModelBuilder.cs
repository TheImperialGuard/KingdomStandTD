using System;
using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Модель таймлайна по данным уровня. Строки — сочетания «дорога × полоса», которые есть в уровне, поэтому
    // при переключении стейджей строки не прыгают. Цвет врага — по порядку его первого появления в уровне:
    // враг сохраняет цвет на всех стейджах, а врагам сверх палитры достаётся общий серый цвет «прочих».
    // В режиме уровня стейджи идут подряд без скипов — как в расчёте длительности уровня.
    public class TimelineModelBuilder
    {
        private const string LINE_BREAK = "\n";
        private const float MIN_DURATION = 10f;

        // Запас справа от последнего события, доля длительности: последняя засечка не прилипает к краю.
        private const float DURATION_MARGIN = 0.05f;

        private static readonly PathLane[] _lanesOrder = { PathLane.Left, PathLane.Center, PathLane.Right };

        private readonly LevelConfig _config;
        private readonly LevelStats _stats;
        private readonly LevelPrefabInfo _prefab;
        private readonly TimelineModel _model;
        private readonly Dictionary<CharacterConfig, int> _enemyIndices = new();
        private readonly Dictionary<(PathNumber, PathLane), int> _rowIndices = new();

        private TimelineModelBuilder(LevelConfig config, LevelStats stats, LevelPrefabInfo prefab, TimelineMode mode)
        {
            _config = config;
            _stats = stats;
            _prefab = prefab;
            _model = new TimelineModel
            {
                Mode = mode,
            };
        }

        public static TimelineModel Build(LevelConfig config, LevelStats stats, LevelPrefabInfo prefab, TimelineMode mode, int stageIndex)
        {
            TimelineModelBuilder builder = new(config, stats, prefab, mode);

            builder.AddEnemies();
            builder.AddRows();
            builder.AddStages(stageIndex);
            builder.PackSubRows();
            builder.SetDuration();

            return builder._model;
        }

        // Каждый враг получает свою строку легенды, даже без своего цвета: по ней его можно подсветить.
        private void AddEnemies()
        {
            foreach (CharacterConfig enemy in EnumerateEnemies())
            {
                int index = _model.Enemies.Count;
                int slot = index < TimelinePalette.EnemySlotsCount ? index : TimelinePalette.NO_SLOT;

                _enemyIndices.Add(enemy, index);
                _model.Enemies.Add(new TimelineEnemy
                {
                    Config = enemy,
                    Name = EnemyCatalog.GetDisplayName(enemy),
                    Tooltip = DescribeEnemy(enemy, slot),
                    Slot = slot,
                });
            }
        }

        // Враги уровня по порядку первого появления: стейдж за стейджем, волна за волной, группа за группой.
        private IEnumerable<CharacterConfig> EnumerateEnemies()
        {
            return _config.Stages
                .SelectMany(LevelStatsCalculator.GetWaves)
                .SelectMany(LevelStatsCalculator.GetGroups)
                .Select(group => group.EnemyConfig)
                .Where(enemy => enemy != null)
                .Distinct();
        }

        // Дороги — по номеру, полосы внутри дороги — слева направо по ходу врагов.
        private void AddRows()
        {
            HashSet<(PathNumber, PathLane)> usedRows = _config.Stages
                .SelectMany(LevelStatsCalculator.GetWaves)
                .Select(wave => (wave.PathNumber, wave.Lane))
                .ToHashSet();

            IEnumerable<PathNumber> pathNumbers = usedRows
                .Select(row => row.Item1)
                .Distinct()
                .OrderBy(pathNumber => pathNumber);

            foreach (PathNumber pathNumber in pathNumbers)
            {
                foreach (PathLane lane in _lanesOrder)
                {
                    if (usedRows.Contains((pathNumber, lane)))
                        AddRow(pathNumber, lane);
                }
            }
        }

        private void AddRow(PathNumber pathNumber, PathLane lane)
        {
            string laneLetter = LevelEditorTexts.GetLaneLetter(lane);
            string road = LevelEditorTexts.DescribeRoadChoice(pathNumber, _prefab);

            _rowIndices.Add((pathNumber, lane), _model.Rows.Count);
            _model.Rows.Add(new TimelineRow
            {
                PathNumber = pathNumber,
                Lane = lane,
                Label = $"{pathNumber} · {laneLetter}",
                Tooltip = $"Дорога {road}, полоса {laneLetter}{LINE_BREAK}{LevelEditorTexts.LANE_EXPLANATION}",
            });
        }

        private void AddStages(int shownStageIndex)
        {
            IReadOnlyList<EnemiesWavesStageConfig> stages = _config.Stages;
            bool isLevelMode = _model.Mode == TimelineMode.Level;
            float stageStart = 0f;

            for (int i = 0; i < stages.Count; i++)
            {
                if (isLevelMode || i == shownStageIndex)
                {
                    float offset = isLevelMode ? stageStart : 0f;
                    AddStage(i, offset);
                }

                stageStart += stages[i].StageTime;
            }
        }

        private void AddStage(int stageIndex, float offset)
        {
            EnemiesWavesStageConfig stage = _config.Stages[stageIndex];
            StageStats stageStats = _stats.Stages[stageIndex];
            bool isLastStage = stageIndex == _config.Stages.Count - 1;

            _model.Stages.Add(new TimelineStage
            {
                StageIndex = stageIndex,
                Start = offset,
                End = offset + stage.StageTime,
                Skip = isLastStage ? null : offset + stage.TimeToSkipStage,
                Tooltip = DescribeStage(stageIndex, stage, stageStats, offset, isLastStage),
            });

            IReadOnlyList<EnemiesWaveConfig> waves = LevelStatsCalculator.GetWaves(stage);

            for (int i = 0; i < waves.Count; i++)
                AddWave(stage, waves[i], stageStats.Waves[i], stageIndex, i, offset, isLastStage);
        }

        private void AddWave(
            EnemiesWavesStageConfig stage,
            EnemiesWaveConfig wave,
            WaveStats waveStats,
            int stageIndex,
            int waveIndex,
            float offset,
            bool isLastStage)
        {
            // Спавн после конца стейджа помечается так же, как в таблице волн: с точностью среза и не в последнем стейдже.
            bool isOverflow = isLastStage == false
                && LevelSliceNumbers.Round(waveStats.SpawnEnd) > LevelSliceNumbers.Round(stage.StageTime);

            TimelineWave timelineWave = new()
            {
                StageIndex = stageIndex,
                WaveIndex = waveIndex,
                RowIndex = _rowIndices[(wave.PathNumber, wave.Lane)],
                Start = offset + wave.StartDelay,
                End = offset + waveStats.SpawnEnd,
                OverflowStart = isOverflow ? offset + stage.StageTime : null,
                TrackSlot = FindTrackSlot(wave),
                Tooltip = DescribeWave(stage, wave, waveStats, stageIndex, waveIndex, offset, isOverflow),
            };

            foreach (WaveSpawn spawn in LevelStatsCalculator.EnumerateSpawns(wave))
                timelineWave.Spawns.Add(new TimelineSpawn(offset + spawn.Time, FindEnemyIndex(spawn.Enemy)));

            _model.Waves.Add(timelineWave);
        }

        // Волна с одним видом врагов получает его цвет на подложке, смешанная — нейтральную.
        private int FindTrackSlot(EnemiesWaveConfig wave)
        {
            List<CharacterConfig> enemies = LevelStatsCalculator.GetGroups(wave)
                .Select(group => group.EnemyConfig)
                .Distinct()
                .ToList();

            return enemies.Count == 1 ? FindEnemySlot(enemies[0]) : TimelinePalette.NO_SLOT;
        }

        private int FindEnemySlot(CharacterConfig enemy)
        {
            int index = FindEnemyIndex(enemy);

            return index != LevelAddress.NO_INDEX ? _model.Enemies[index].Slot : TimelinePalette.NO_SLOT;
        }

        private int FindEnemyIndex(CharacterConfig enemy)
        {
            if (enemy != null && _enemyIndices.TryGetValue(enemy, out int index))
                return index;

            return LevelAddress.NO_INDEX;
        }

        // Волны одной строки, которые пересекаются по времени, расходятся по подстрокам: жадно, в порядке начала.
        private void PackSubRows()
        {
            foreach (IGrouping<int, TimelineWave> rowWaves in _model.Waves.GroupBy(wave => wave.RowIndex))
            {
                List<float> subRowEnds = new();

                IEnumerable<TimelineWave> orderedWaves = rowWaves
                    .OrderBy(wave => wave.Start)
                    .ThenBy(wave => wave.StageIndex)
                    .ThenBy(wave => wave.WaveIndex);

                foreach (TimelineWave wave in orderedWaves)
                {
                    int subRow = subRowEnds.FindIndex(subRowEnd => subRowEnd < wave.Start);

                    if (subRow < 0)
                    {
                        subRow = subRowEnds.Count;
                        subRowEnds.Add(wave.End);
                    }
                    else
                    {
                        subRowEnds[subRow] = wave.End;
                    }

                    wave.SubRow = subRow;
                }

                _model.Rows[rowWaves.Key].SubRowsCount = Math.Max(1, subRowEnds.Count);
            }
        }

        private void SetDuration()
        {
            float stagesEnd = _model.Stages.Select(stage => stage.End).DefaultIfEmpty(0f).Max();
            float wavesEnd = _model.Waves.Select(wave => wave.End).DefaultIfEmpty(0f).Max();
            float duration = Math.Max(MIN_DURATION, Math.Max(stagesEnd, wavesEnd));

            _model.Duration = duration * (1f + DURATION_MARGIN);
        }

        private static string DescribeEnemy(CharacterConfig enemy, int slot)
        {
            string stats = EnemyCatalog.DescribeStats(enemy);

            if (slot != TimelinePalette.NO_SLOT)
                return stats;

            int colorsCount = TimelinePalette.EnemySlotsCount;

            return $"{stats}{LINE_BREAK}Показан серым: в палитре {colorsCount} цветов, а видов врагов в уровне больше";
        }

        private string DescribeStage(int stageIndex, EnemiesWavesStageConfig stage, StageStats stageStats, float offset, bool isLastStage)
        {
            string address = LevelAddress.Format(stageIndex);
            string stageTime = LevelEditorTexts.Seconds(stage.StageTime);
            string skipTime = LevelEditorTexts.Seconds(stage.TimeToSkipStage);
            string skipText = isLastStage ? "скипа нет" : $"скип через {skipTime}";
            string health = LevelEditorTexts.Number(stageStats.Health);

            List<string> lines = new()
            {
                $"{address}: время {stageTime}, {skipText}",
                $"Волн {stageStats.WavesCount}, врагов {stageStats.EnemiesCount}, HP {health}",
            };

            if (_model.Mode == TimelineMode.Level)
            {
                string start = LevelEditorTexts.Seconds(offset);
                lines.Add($"Начинается на {start} от начала уровня, если не скипать");
            }

            return string.Join(LINE_BREAK, lines);
        }

        private string DescribeWave(
            EnemiesWavesStageConfig stage,
            EnemiesWaveConfig wave,
            WaveStats waveStats,
            int stageIndex,
            int waveIndex,
            float offset,
            bool isOverflow)
        {
            string address = LevelAddress.Format(stageIndex, waveIndex);
            string composition = LevelEditorTexts.DescribeComposition(wave);
            string road = LevelEditorTexts.DescribeRoadChoice(wave.PathNumber, _prefab);
            string laneLetter = LevelEditorTexts.GetLaneLetter(wave.Lane);
            string startDelay = LevelEditorTexts.Seconds(wave.StartDelay);
            string delay = LevelEditorTexts.Seconds(wave.DelayBetweenSpawns);
            string spawnEnd = LevelEditorTexts.Seconds(waveStats.SpawnEnd);
            string health = LevelEditorTexts.Number(waveStats.Health);

            List<string> lines = new()
            {
                address,
                $"{composition} — врагов {waveStats.EnemiesCount}",
                $"Дорога {road}, полоса {laneLetter}",
                $"Старт {startDelay}, интервал {delay}, последний враг на {spawnEnd} от начала стейджа",
                $"HP {health}, золото {waveStats.KillGold}",
            };

            if (isOverflow)
            {
                string overflow = LevelEditorTexts.Seconds(waveStats.SpawnEnd - stage.StageTime);
                lines.Add($"Спавн идёт ещё {overflow} после конца стейджа");
            }

            if (_model.Mode == TimelineMode.Level)
            {
                string stageStart = LevelEditorTexts.Seconds(offset);
                lines.Add($"Стейдж начинается на {stageStart} от начала уровня, если не скипать");
            }

            return string.Join(LINE_BREAK, lines);
        }
    }
}
