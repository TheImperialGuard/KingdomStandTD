using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies;
using Assets._Project.Develop.Runtime.Utilities.Text;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Строка цифр способности под её описанием (Р-19, Р-67): числа берутся из полей конфига, в описаниях их нет. У башен —
    // значения заданного уровня способности: в книге первого, в окне способностей боя — покупаемого (Р-20 боя). Части
    // строки разделены «·» и переносятся только по нему: «раз в 10 с» не разрывается.
    public static class EncyclopediaAbilityNumbers
    {
        private const int FirstLevel = 1;
        private const char Space = ' ';
        private const string PartsSeparator = "·";

        public static string Format(AbilityConfig ability) => Format(ability, FirstLevel);

        public static string Format(AbilityConfig ability, int level) => KeepPartsTogether(FormatTowerAbility(ability, level));

        public static string Format(EnemyAbilityConfig ability) => KeepPartsTogether(FormatEnemyAbility(ability));

        private static string FormatTowerAbility(AbilityConfig ability, int level)
        {
            switch (ability)
            {
                case AdditionalShootingTargetsAbilityConfig targets:
                    return FormatAdditionalTargets(targets.GetAdditionsTargetsBy(level));

                case PoisonedAttackAbilityConfig poison:
                    return FormatPoison(
                        DecimalText.Format(poison.GetDamageByLevel(level)),
                        DecimalText.Format(poison.GetEffectIntervalByLevel(level)),
                        DecimalText.Format(poison.GetEffectTimeByLevel(level)));

                case ProjectileFragmentsAbilityConfig fragments:
                    return FormatFragments((int)fragments.FragmentsCount, fragments.GetFragmentsDamageBy(level));

                case AttackStunAbilityConfig attackStun:
                    return FormatStun(DecimalText.Format(attackStun.GetStunTimeBy(level)));

                case OneShotAttackAbilityConfig oneShot:
                    return FormatCooldown(DecimalText.Format(oneShot.GetCooldownBy(level)));

                case StunAbilityConfig stun:
                    return FormatFreeze(
                        DecimalText.Format(stun.GetStunTimeBy(level)),
                        DecimalText.Format(stun.Cooldown));

                default:
                    return string.Empty;
            }
        }

        private static string FormatEnemyAbility(EnemyAbilityConfig ability)
        {
            string cooldown = DecimalText.Format(ability.Cooldown);

            switch (ability)
            {
                case SummonAbilityConfig summon:
                    return $"Отряд {summon.Count} · раз в {cooldown} с";

                case WarCryAbilityConfig warCry:
                    return FormatWarCry(
                        DecimalText.Format(warCry.Radius),
                        DecimalText.Format(warCry.SpeedBoostIndex),
                        DecimalText.Format(warCry.SpeedBoostDuration),
                        cooldown);

                default:
                    return FormatCooldown(cooldown);
            }
        }

        // Все пробелы — неразрывные, кроме пробела после разделителя частей.
        private static string KeepPartsTogether(string numbers)
        {
            string nonBreakingSpace = NonBreakingSpace.Character.ToString();
            string glued = numbers.Replace(Space, NonBreakingSpace.Character);

            return glued.Replace(PartsSeparator + nonBreakingSpace, PartsSeparator + Space);
        }

        private static string FormatCooldown(string cooldown) => $"Раз в {cooldown} с";

        private static string FormatStun(string duration) => $"Оглушение на {duration} с";

        private static string FormatAdditionalTargets(int targets)
        {
            string noun = RussianPlural.Select(targets, "цель", "цели", "целей");

            return $"+{targets} {noun} за уровень";
        }

        private static string FormatPoison(string damage, string interval, string duration)
            => $"Яд: {damage} урона раз в {interval} с, {duration} с";

        private static string FormatFragments(int count, float damage)
        {
            string noun = RussianPlural.Select(count, "осколок", "осколка", "осколков");
            string damageText = DecimalText.Format(damage);

            return $"{count} {noun} по {damageText} урона";
        }

        private static string FormatFreeze(string duration, string cooldown)
            => $"Заморозка {duration} с · раз в {cooldown} с";

        private static string FormatWarCry(string radius, string speedBoost, string boostDuration, string cooldown)
            => $"Радиус {radius} · скорость ×{speedBoost} на {boostDuration} с · раз в {cooldown} с";
    }
}
