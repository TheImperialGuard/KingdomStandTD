using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Тексты окна редактора уровней: строки списков, составы волн, итоги. Числа — с точкой и не больше одного знака.
    public static class LevelEditorTexts
    {
        public const string ERROR_MARKER = "✖ ";
        public const string WARNING_MARKER = "⚠ ";
        public const string LANE_EXPLANATION =
            "Полоса по ходу врагов: Л — слева, П — справа от направления движения.\n"
            + "Там, где дорога идёт вниз по экрану, левая полоса на экране справа.";

        private const string NUMBER_FORMAT = "0.#";
        private const string GROUPS_SEPARATOR = " + ";
        private const string EMPTY_PATTERN = "пусто";
        private const string MISSING_ENEMY = "?";
        private const string MISSING_ROAD = "нет на префабе";

        public static string Number(float value)
        {
            if (float.IsInfinity(value))
                return "∞";

            return value.ToString(NUMBER_FORMAT, CultureInfo.InvariantCulture);
        }

        public static string Seconds(float value)
        {
            string number = Number(value);
            return $"{number} с";
        }

        // «Скелет×3 + Орк×1 · ×4»: шаблон групп и число его повторов, если оно больше одного.
        public static string DescribeComposition(EnemiesWaveConfig wave)
        {
            IReadOnlyList<WaveEnemyGroup> groups = LevelStatsCalculator.GetGroups(wave);

            if (groups.Count == 0)
                return EMPTY_PATTERN;

            IEnumerable<string> groupsTexts = groups.Select(DescribeGroup);
            string pattern = string.Join(GROUPS_SEPARATOR, groupsTexts);

            if (wave.PatternRepeats <= LevelConfigLimits.MIN_PATTERN_REPEATS)
                return pattern;

            return $"{pattern} · ×{wave.PatternRepeats}";
        }

        public static string DescribeStageRow(int stageIndex, EnemiesWavesStageConfig stage, StageStats stats, IReadOnlyList<LevelIssue> issues)
        {
            int stageNumber = stageIndex + 1;
            string marker = GetIssuesMarker(issues, stageIndex);
            string stageTime = Seconds(stage.StageTime);
            string skipTime = Seconds(stage.TimeToSkipStage);
            string health = Number(stats.Health);

            return $"{marker}{stageNumber} · {stageTime} · скип {skipTime} · волн {stats.WavesCount} · врагов {stats.EnemiesCount} · HP {health}";
        }

        public static string DescribeLevelTotals(LevelConfig config, LevelStats stats)
        {
            string health = Number(stats.Health);
            string physicHealth = Number(stats.EffectiveHealthPhysic);
            string magicHealth = Number(stats.EffectiveHealthMagic);
            string duration = Seconds(stats.DurationWithoutSkips);
            string durationWithSkips = Seconds(stats.DurationWithMaxSkips);

            return $"Стейджей {stats.StagesCount} · волн {stats.WavesCount} · врагов {stats.EnemiesCount} · HP {health}"
                + $" · EHP физ {physicHealth} / маг {magicHealth}"
                + $" · золото {config.StartGold} → {stats.TotalGoldWithoutSkips} (со скипами {stats.TotalGoldWithMaxSkips})"
                + $" · спавн до {duration} (со скипами {durationWithSkips})";
        }

        // Итог проверки уровня: заголовок панели замечаний и сводка в инспекторе LevelConfig.
        public static string DescribeIssuesCount(IReadOnlyList<LevelIssue> issues)
        {
            if (issues.Count == 0)
                return "Замечаний нет";

            int errorsCount = issues.Count(issue => issue.IsError);
            int warningsCount = issues.Count - errorsCount;

            return $"Замечания: ошибок {errorsCount}, предупреждений {warningsCount}";
        }

        // «✖ Стейдж 2, волна 1: …» — замечание со значком своей серьёзности.
        public static string FormatIssue(LevelIssue issue)
        {
            string marker = issue.IsError ? ERROR_MARKER : WARNING_MARKER;
            string issueText = issue.ToString();

            return $"{marker}{issueText}";
        }

        // Короткое имя показателя для переключателя и заголовка кривой сложности.
        public static string GetMeasureName(DifficultyMeasure measure)
        {
            switch (measure)
            {
                case DifficultyMeasure.Health:
                    return "HP";

                case DifficultyMeasure.EffectiveHealthPhysic:
                    return "EHP физ";

                case DifficultyMeasure.EffectiveHealthMagic:
                    return "EHP маг";

                case DifficultyMeasure.HealthPerSecond:
                    return "HP в секунду";

                default:
                    return measure.ToString();
            }
        }

        public static string DescribeMeasure(DifficultyMeasure measure)
        {
            switch (measure)
            {
                case DifficultyMeasure.Health:
                    return "Сумма HP врагов стейджа";

                case DifficultyMeasure.EffectiveHealthPhysic:
                    return "Сколько физического урона нужно нанести с учётом сопротивлений";

                case DifficultyMeasure.EffectiveHealthMagic:
                    return "Сколько магического урона нужно нанести с учётом сопротивлений";

                case DifficultyMeasure.HealthPerSecond:
                    return "HP стейджа, делённое на его время";

                default:
                    return measure.ToString();
            }
        }

        public static string DescribeRoadChoice(PathNumber number, LevelPrefabInfo prefab)
        {
            LevelRoad road = prefab.Roads.FirstOrDefault(candidate => candidate.Number == number);
            string roadName = road != null ? road.Name : MISSING_ROAD;

            return $"{number} · {roadName}";
        }

        // Буква полосы по ходу врагов: «Л», «Ц», «П».
        public static string GetLaneLetter(PathLane lane)
        {
            switch (lane)
            {
                case PathLane.Left:
                    return "Л";

                case PathLane.Center:
                    return "Ц";

                case PathLane.Right:
                    return "П";

                default:
                    return lane.ToString();
            }
        }

        public static string DescribeDamageType(DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Physic:
                    return "физическому";

                case DamageTypes.Magic:
                    return "магическому";

                default:
                    return damageType.ToString();
            }
        }

        // «Защита от физики / магии».
        public static string DescribeDamageSource(DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Physic:
                    return "физики";

                case DamageTypes.Magic:
                    return "магии";

                default:
                    return damageType.ToString();
            }
        }

        // Маркер самого серьёзного замечания стейджа: ошибка важнее предупреждения.
        public static string GetIssuesMarker(IReadOnlyList<LevelIssue> issues, int stageIndex)
        {
            bool hasWarning = false;

            foreach (LevelIssue issue in issues)
            {
                if (issue.StageIndex != stageIndex)
                    continue;

                if (issue.IsError)
                    return ERROR_MARKER;

                hasWarning = true;
            }

            return hasWarning ? WARNING_MARKER : string.Empty;
        }

        private static string DescribeGroup(WaveEnemyGroup group)
        {
            string enemyName = group.EnemyConfig != null ? EnemyCatalog.GetDisplayName(group.EnemyConfig) : MISSING_ENEMY;

            return $"{enemyName}×{group.Count}";
        }
    }
}
