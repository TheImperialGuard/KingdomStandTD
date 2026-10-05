using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Человекочитаемый дифф данных уровня: одна строка — одно изменённое значение.
    // Стейджи и волны сравниваются по позициям; лишние в конце — добавленные или удалённые.
    public static class LevelSliceComparer
    {
        private const string ARROW = " → ";
        private const string GROUPS_SEPARATOR = " + ";
        private const string MISSING_ENEMY = "?";
        private const string EMPTY_PATTERN = "пусто";
        private const string NOTE_LINE_SEPARATOR = " / ";

        public static List<string> Compare(LevelSlice before, LevelSlice after)
        {
            List<string> changes = new();

            AddIfChanged(changes, "Стартовое золото:", before.StartGold, after.StartGold);
            AddIfChanged(changes, "Здоровье игрока:", before.StartPlayerHealth, after.StartPlayerHealth);

            int commonCount = Math.Min(before.Stages.Count, after.Stages.Count);

            for (int i = 0; i < commonCount; i++)
                CompareStage(changes, i, before.Stages[i], after.Stages[i]);

            for (int i = commonCount; i < after.Stages.Count; i++)
                AddStage(changes, i, after.Stages[i]);

            for (int i = commonCount; i < before.Stages.Count; i++)
            {
                string address = LevelAddress.Format(i);
                string stageText = DescribeStage(before.Stages[i]);
                changes.Add($"{address}: удалён — {stageText}");
            }

            return changes;
        }

        private static void CompareStage(List<string> changes, int stageIndex, StageSlice before, StageSlice after)
        {
            string address = LevelAddress.Format(stageIndex);

            AddIfChanged(changes, $"{address}: время", before.StageTime, after.StageTime);
            AddIfChanged(changes, $"{address}: время до скипа", before.TimeToSkipStage, after.TimeToSkipStage);
            AddIfNoteChanged(changes, address, before.Note, after.Note);

            int commonCount = Math.Min(before.Waves.Count, after.Waves.Count);

            for (int i = 0; i < commonCount; i++)
                CompareWave(changes, stageIndex, i, before.Waves[i], after.Waves[i]);

            for (int i = commonCount; i < after.Waves.Count; i++)
                AddWave(changes, stageIndex, i, after.Waves[i]);

            for (int i = commonCount; i < before.Waves.Count; i++)
            {
                string waveAddress = LevelAddress.Format(stageIndex, i);
                string waveText = DescribeWave(before.Waves[i]);
                changes.Add($"{waveAddress}: удалена — {waveText}");
            }
        }

        private static void CompareWave(List<string> changes, int stageIndex, int waveIndex, WaveSlice before, WaveSlice after)
        {
            string address = LevelAddress.Format(stageIndex, waveIndex);

            AddIfChanged(changes, $"{address}: состав", DescribeGroups(before.Groups), DescribeGroups(after.Groups));
            AddIfChanged(changes, $"{address}: повторы", before.PatternRepeats, after.PatternRepeats);
            AddIfChanged(changes, $"{address}: дорога", before.PathNumber.ToString(), after.PathNumber.ToString());
            AddIfChanged(changes, $"{address}: полоса", before.Lane.ToString(), after.Lane.ToString());
            AddIfChanged(changes, $"{address}: старт", before.StartDelay, after.StartDelay);
            AddIfChanged(changes, $"{address}: интервал", before.DelayBetweenSpawns, after.DelayBetweenSpawns);
            AddIfNoteChanged(changes, address, before.Note, after.Note);
        }

        private static void AddStage(List<string> changes, int stageIndex, StageSlice stage)
        {
            string address = LevelAddress.Format(stageIndex);
            string stageText = DescribeStage(stage);

            changes.Add($"{address}: добавлен — {stageText}");

            for (int i = 0; i < stage.Waves.Count; i++)
                AddWave(changes, stageIndex, i, stage.Waves[i]);
        }

        private static void AddWave(List<string> changes, int stageIndex, int waveIndex, WaveSlice wave)
        {
            string address = LevelAddress.Format(stageIndex, waveIndex);
            string waveText = DescribeWave(wave);

            changes.Add($"{address}: добавлена — {waveText}");
        }

        private static void AddIfChanged(List<string> changes, string subject, int before, int after)
        {
            if (before != after)
                changes.Add($"{subject} {before}{ARROW}{after}");
        }

        private static void AddIfChanged(List<string> changes, string subject, float before, float after)
        {
            if (LevelSliceNumbers.IsSame(before, after))
                return;

            string beforeText = LevelSliceNumbers.ToText(before);
            string afterText = LevelSliceNumbers.ToText(after);
            changes.Add($"{subject} {beforeText}{ARROW}{afterText}");
        }

        private static void AddIfChanged(List<string> changes, string subject, string before, string after)
        {
            if (string.Equals(before, after, StringComparison.Ordinal) == false)
                changes.Add($"{subject} {before}{ARROW}{after}");
        }

        private static void AddIfNoteChanged(List<string> changes, string address, string before, string after)
        {
            string beforeText = FlattenNote(before);
            string afterText = FlattenNote(after);

            if (string.Equals(beforeText, afterText, StringComparison.Ordinal) == false)
                changes.Add($"{address}: заметка «{beforeText}»{ARROW}«{afterText}»");
        }

        private static string DescribeStage(StageSlice stage)
        {
            string stageTimeText = LevelSliceNumbers.ToText(stage.StageTime);
            string skipTimeText = LevelSliceNumbers.ToText(stage.TimeToSkipStage);

            return $"время {stageTimeText}, время до скипа {skipTimeText}, волн {stage.Waves.Count}";
        }

        private static string DescribeWave(WaveSlice wave)
        {
            string groupsText = DescribeGroups(wave.Groups);
            string startDelayText = LevelSliceNumbers.ToText(wave.StartDelay);
            string delayText = LevelSliceNumbers.ToText(wave.DelayBetweenSpawns);

            return $"{groupsText} · ×{wave.PatternRepeats}, {wave.PathNumber} · {wave.Lane}, старт {startDelayText}, интервал {delayText}";
        }

        private static string DescribeGroups(IReadOnlyList<EnemyGroupSlice> groups)
        {
            if (groups == null || groups.Count == 0)
                return EMPTY_PATTERN;

            IEnumerable<string> groupsTexts = groups.Select(DescribeGroup);

            return string.Join(GROUPS_SEPARATOR, groupsTexts);
        }

        private static string DescribeGroup(EnemyGroupSlice group)
        {
            string enemy = string.IsNullOrEmpty(group.Enemy) ? MISSING_ENEMY : group.Enemy;

            return $"{enemy}×{group.Count}";
        }

        // Заметка выводится одной строкой: переводы строк заменяются разделителем.
        private static string FlattenNote(string note)
        {
            if (string.IsNullOrEmpty(note))
                return string.Empty;

            string[] lines = note.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            return string.Join(NOTE_LINE_SEPARATOR, lines);
        }
    }
}
