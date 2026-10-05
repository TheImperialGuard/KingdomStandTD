using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Массовая операция над срезом уровня — меняет на месте стейджи из области. Количество врагов: в волне с одной
    // группой меняется количество, с несколькими — повторы шаблона, чтобы сохранить пропорцию смеси; округление
    // до ближайшего целого. Интервал спавна — в процентах, старт волн, время стейджа и скип — сдвигом в секундах.
    // Значения не уходят ниже границ LevelConfigLimits; что упёрлось в границу, перечисляет результат.
    public static class LevelBulkEdit
    {
        // Процент делится в double: в float 5 × 90 % давало бы 4.4999…, и половинка округлялась бы вниз.
        private const double PERCENT_BASE = 100.0;

        public static List<string> Apply(LevelSlice slice, LevelBulkEditSettings settings)
        {
            LimitHits hits = new();
            int firstStageIndex = Math.Max(0, settings.FirstStageIndex);
            int lastStageIndex = Math.Min(settings.LastStageIndex, slice.Stages.Count - 1);

            for (int i = firstStageIndex; i <= lastStageIndex; i++)
                ApplyToStage(slice.Stages[i], settings, hits);

            return hits.Describe();
        }

        private static void ApplyToStage(StageSlice stage, LevelBulkEditSettings settings, LimitHits hits)
        {
            if (settings.StageTimeShift != 0f)
                stage.StageTime = Limit(stage.StageTime + settings.StageTimeShift, LevelConfigLimits.MIN_STAGE_TIME, ref hits.StageTimes);

            if (settings.SkipShift != 0f)
                stage.TimeToSkipStage = Limit(stage.TimeToSkipStage + settings.SkipShift, LevelConfigLimits.MIN_TIME_TO_SKIP_STAGE, ref hits.Skips);

            foreach (WaveSlice wave in stage.Waves)
                ApplyToWave(wave, settings, hits);
        }

        private static void ApplyToWave(WaveSlice wave, LevelBulkEditSettings settings, LimitHits hits)
        {
            if (settings.CountPercent != LevelBulkEditSettings.UNCHANGED_PERCENT)
                ScaleEnemies(wave, settings.CountPercent, hits);

            if (settings.DelayPercent != LevelBulkEditSettings.UNCHANGED_PERCENT)
            {
                double delay = wave.DelayBetweenSpawns * settings.DelayPercent / PERCENT_BASE;
                wave.DelayBetweenSpawns = Limit(delay, LevelConfigLimits.MIN_DELAY_BETWEEN_SPAWNS, ref hits.Delays);
            }

            if (settings.StartShift != 0f)
                wave.StartDelay = Limit(wave.StartDelay + settings.StartShift, LevelConfigLimits.MIN_START_DELAY, ref hits.Starts);
        }

        // Одна группа — меняется её количество, несколько — повторы шаблона. Пустой шаблон не меняется.
        private static void ScaleEnemies(WaveSlice wave, int percent, LimitHits hits)
        {
            if (wave.Groups.Count == 1)
            {
                EnemyGroupSlice group = wave.Groups[0];
                group.Count = Limit(Scale(group.Count, percent), LevelConfigLimits.MIN_GROUP_COUNT, ref hits.Counts);
                return;
            }

            if (wave.Groups.Count > 1)
                wave.PatternRepeats = Limit(Scale(wave.PatternRepeats, percent), LevelConfigLimits.MIN_PATTERN_REPEATS, ref hits.Repeats);
        }

        private static int Scale(int value, int percent) => (int)Math.Round(value * percent / PERCENT_BASE, MidpointRounding.AwayFromZero);

        private static int Limit(int value, int minValue, ref int hitsCount)
        {
            if (value >= minValue)
                return value;

            hitsCount++;
            return minValue;
        }

        // Секунды округляются до точности срезов.
        private static float Limit(double value, float minValue, ref int hitsCount)
        {
            float roundedValue = (float)LevelSliceNumbers.Round(value);

            if (roundedValue >= minValue)
                return roundedValue;

            hitsCount++;
            return minValue;
        }

        // Сколько значений каждого вида упёрлось в нижнюю границу.
        private class LimitHits
        {
            public int Counts;
            public int Repeats;
            public int Delays;
            public int Starts;
            public int StageTimes;
            public int Skips;

            public List<string> Describe()
            {
                string minDelay = LevelSliceNumbers.ToText(LevelConfigLimits.MIN_DELAY_BETWEEN_SPAWNS);
                string minStageTime = LevelSliceNumbers.ToText(LevelConfigLimits.MIN_STAGE_TIME);
                List<string> notes = new();

                AddNote(notes, Counts, $"Количество в группе не бывает меньше {LevelConfigLimits.MIN_GROUP_COUNT} — групп");
                AddNote(notes, Repeats, $"Повторы шаблона не бывают меньше {LevelConfigLimits.MIN_PATTERN_REPEATS} — волн");
                AddNote(notes, Delays, $"Интервал спавна не бывает меньше {minDelay} с — волн");
                AddNote(notes, Starts, "Волна не стартует раньше начала стейджа — волн");
                AddNote(notes, StageTimes, $"Время стейджа не бывает меньше {minStageTime} с — стейджей");
                AddNote(notes, Skips, "Скип не бывает раньше начала стейджа — стейджей");

                return notes;
            }

            private static void AddNote(List<string> notes, int hitsCount, string subject)
            {
                if (hitsCount > 0)
                    notes.Add($"{subject}: {hitsCount}");
            }
        }
    }
}
