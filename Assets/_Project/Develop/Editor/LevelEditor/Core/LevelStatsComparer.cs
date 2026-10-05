using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Сравнение статистики уровня до и после правки — что изменится, если применить срез.
    // Одна строка — одна изменившаяся метрика стейджа или уровня; состав врагов — по каждому типу отдельно.
    public static class LevelStatsComparer
    {
        private const string ARROW = " → ";
        private const string MISSING_VALUE = "—";
        private const string LEVEL_ADDRESS = "Уровень";
        private const string PERCENT_FORMAT = "+0.#;-0.#";
        private const float PERCENT_SCALE = 100f;

        private static readonly (string Label, Func<StageStats, float?> Read)[] _stageMetrics =
        {
            ("врагов", stats => stats.EnemiesCount),
            ("HP", stats => stats.Health),
            ("EHP против физики", stats => stats.EffectiveHealthPhysic),
            ("EHP против магии", stats => stats.EffectiveHealthMagic),
            ("HP в секунду", stats => stats.HealthPerSecond),
            ("золото за убийства", stats => stats.KillGold),
            ("максимум золота за скип", stats => stats.MaxSkipGold),
            ("золото к началу без скипов", stats => stats.GoldAtStartWithoutSkips),
            ("золото к началу со скипами", stats => stats.GoldAtStartWithMaxSkips),
            ("конец спавна", stats => stats.SpawnEnd),
            ("спавн после конца стейджа", stats => stats.SpawnOverflow),
            ("последний приход на базу", stats => stats.LastArrival),
            ("пик врагов на поле", stats => stats.PeakEnemies),
            ("угроза прорыва", stats => stats.BreakthroughThreat),
        };

        private static readonly (string Label, Func<LevelStats, float?> Read)[] _levelMetrics =
        {
            ("врагов", stats => stats.EnemiesCount),
            ("HP", stats => stats.Health),
            ("EHP против физики", stats => stats.EffectiveHealthPhysic),
            ("EHP против магии", stats => stats.EffectiveHealthMagic),
            ("золото за убийства", stats => stats.KillGold),
            ("максимум золота за скипы", stats => stats.MaxSkipGold),
            ("всего золота без скипов", stats => stats.TotalGoldWithoutSkips),
            ("всего золота со скипами", stats => stats.TotalGoldWithMaxSkips),
            ("длительность без скипов", stats => stats.DurationWithoutSkips),
            ("длительность со скипами", stats => stats.DurationWithMaxSkips),
            ("пик врагов на поле", stats => stats.PeakEnemies),
            ("угроза прорыва", stats => stats.BreakthroughThreat),
        };

        public static List<string> Compare(LevelStats before, LevelStats after)
        {
            List<string> changes = new();
            int stagesCount = Math.Max(before.Stages.Count, after.Stages.Count);

            for (int i = 0; i < stagesCount; i++)
            {
                StageStats beforeStage = i < before.Stages.Count ? before.Stages[i] : null;
                StageStats afterStage = i < after.Stages.Count ? after.Stages[i] : null;
                string address = LevelAddress.Format(i);

                AddCompositionChanges(changes, address, beforeStage?.EnemiesByType, afterStage?.EnemiesByType);
                AddMetricChanges(changes, address, _stageMetrics, beforeStage, afterStage);
            }

            AddCompositionChanges(changes, LEVEL_ADDRESS, before.EnemiesByType, after.EnemiesByType);
            AddMetricChanges(changes, LEVEL_ADDRESS, _levelMetrics, before, after);

            return changes;
        }

        // Тип врага, которого нет с одной из сторон, считается с нулевым количеством.
        private static void AddCompositionChanges(
            List<string> changes,
            string address,
            IReadOnlyDictionary<string, int> before,
            IReadOnlyDictionary<string, int> after)
        {
            IEnumerable<string> enemyNames = (before?.Keys ?? Enumerable.Empty<string>())
                .Union(after?.Keys ?? Enumerable.Empty<string>())
                .OrderBy(enemyName => enemyName, StringComparer.Ordinal);

            foreach (string enemyName in enemyNames)
            {
                int beforeCount = GetCount(before, enemyName);
                int afterCount = GetCount(after, enemyName);

                if (beforeCount != afterCount)
                    changes.Add($"{address}: {enemyName} {beforeCount}{ARROW}{afterCount}");
            }
        }

        private static int GetCount(IReadOnlyDictionary<string, int> counts, string enemyName)
        {
            if (counts == null || counts.TryGetValue(enemyName, out int count) == false)
                return 0;

            return count;
        }

        private static void AddMetricChanges<TStats>(
            List<string> changes,
            string address,
            IEnumerable<(string Label, Func<TStats, float?> Read)> metrics,
            TStats before,
            TStats after)
            where TStats : class
        {
            foreach ((string label, Func<TStats, float?> read) in metrics)
            {
                float? beforeValue = before != null ? read(before) : null;
                float? afterValue = after != null ? read(after) : null;

                if (IsSame(beforeValue, afterValue))
                    continue;

                string beforeText = ToText(beforeValue);
                string afterText = ToText(afterValue);
                string percentText = DescribePercent(beforeValue, afterValue);
                changes.Add($"{address}: {label} {beforeText}{ARROW}{afterText}{percentText}");
            }
        }

        private static bool IsSame(float? before, float? after)
        {
            if (before.HasValue == false || after.HasValue == false)
                return before.HasValue == after.HasValue;

            return LevelSliceNumbers.IsSame(before.Value, after.Value);
        }

        private static string ToText(float? value) => value.HasValue ? LevelSliceNumbers.ToText(value.Value) : MISSING_VALUE;

        private static string DescribePercent(float? before, float? after)
        {
            if (before.HasValue == false || after.HasValue == false || before.Value == 0f)
                return string.Empty;

            float percent = (after.Value - before.Value) / Math.Abs(before.Value) * PERCENT_SCALE;
            string percentText = percent.ToString(PERCENT_FORMAT, CultureInfo.InvariantCulture);

            return $" ({percentText}%)";
        }
    }
}
