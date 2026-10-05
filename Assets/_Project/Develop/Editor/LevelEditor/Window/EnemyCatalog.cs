using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Враги для выпадающих списков окна: имена, подсказки со статами и превью префабов.
    public class EnemyCatalog
    {
        private const string MISSING_ENEMY_NAME = "— не выбран —";
        private const string STATS_SEPARATOR = " · ";
        private const float PERCENT_SCALE = 100f;

        private readonly List<CharacterConfig> _choices = new();
        private readonly Dictionary<string, CharacterConfig> _enemiesByAssetName = new(StringComparer.Ordinal);

        public EnemyCatalog()
        {
            Reload();
        }

        // Первый элемент — null, «враг не выбран». Список общий для всех выпадающих полей окна.
        public List<CharacterConfig> Choices => _choices;

        public void Reload()
        {
            List<CharacterConfig> enemies = LevelEditorAssets.FindEnemyConfigs()
                .OrderBy(GetDisplayName, StringComparer.CurrentCulture)
                .ToList();

            _choices.Clear();
            _choices.Add(null);
            _choices.AddRange(enemies);

            _enemiesByAssetName.Clear();

            foreach (CharacterConfig enemy in enemies)
                _enemiesByAssetName.TryAdd(enemy.name, enemy);
        }

        // Выпадающее поле требует, чтобы значение было среди вариантов: враг мог появиться после загрузки каталога.
        public void EnsureChoice(CharacterConfig enemy)
        {
            if (enemy != null && _choices.Contains(enemy) == false)
                _choices.Add(enemy);
        }

        public string GetDisplayNameByAsset(string assetName)
        {
            if (_enemiesByAssetName.TryGetValue(assetName, out CharacterConfig enemy))
                return GetDisplayName(enemy);

            return assetName;
        }

        public static string GetDisplayName(CharacterConfig enemy)
        {
            if (enemy == null)
                return MISSING_ENEMY_NAME;

            return string.IsNullOrWhiteSpace(enemy.CharacterName) ? enemy.name : enemy.CharacterName;
        }

        public static string GetChoiceName(CharacterConfig enemy)
        {
            if (enemy == null)
                return MISSING_ENEMY_NAME;

            string displayName = GetDisplayName(enemy);

            if (displayName == enemy.name)
                return displayName;

            return $"{displayName} ({enemy.name})";
        }

        public static string DescribeStats(CharacterConfig enemy)
        {
            if (enemy == null)
                return "Враг не выбран";

            string choiceName = GetChoiceName(enemy);

            if ((enemy is MeleeConfig meleeEnemy) == false)
            {
                string configType = enemy.GetType().Name;
                return $"{choiceName}\nКонфиг {configType} игра не создаёт: EnemiesFactory поддерживает только MeleeConfig";
            }

            StringBuilder text = new();
            string health = LevelEditorTexts.Number(meleeEnemy.MaxHealth);
            string moveSpeed = LevelEditorTexts.Number(meleeEnemy.MoveSpeed);

            text.AppendLine(choiceName);
            text.AppendLine($"HP {health} · скорость {moveSpeed}");
            text.AppendLine(DescribeResistance(meleeEnemy));
            text.Append($"Золото за убийство {meleeEnemy.GoldOnDeath} · урон по базе {meleeEnemy.DamageOnFinishPath}");

            if (meleeEnemy.IsBoss)
                text.Append("\nБосс: за один удар теряет не больше трети HP");

            return text.ToString();
        }

        // Коротко для карточки группы: «HP 100 · скорость 0.6 · защита от магии 85% · золото 20 · урон по базе 1».
        public static string DescribeShortStats(CharacterConfig enemy)
        {
            if (enemy == null)
                return "Выберите врага";

            if ((enemy is MeleeConfig meleeEnemy) == false)
                return "Этот тип врага игра не создаёт";

            List<string> parts = new();
            string health = LevelEditorTexts.Number(meleeEnemy.MaxHealth);
            string moveSpeed = LevelEditorTexts.Number(meleeEnemy.MoveSpeed);

            parts.Add($"HP {health}");
            parts.Add($"скорость {moveSpeed}");

            if (HasResistance(meleeEnemy))
            {
                string damageSource = LevelEditorTexts.DescribeDamageSource(meleeEnemy.DamageResistanceType);
                string percent = LevelEditorTexts.Number(meleeEnemy.DamageResistanceIndex * PERCENT_SCALE);
                parts.Add($"защита от {damageSource} {percent}%");
            }

            parts.Add($"золото {meleeEnemy.GoldOnDeath}");
            parts.Add($"урон по базе {meleeEnemy.DamageOnFinishPath}");

            if (meleeEnemy.IsBoss)
                parts.Add("босс");

            return string.Join(STATS_SEPARATOR, parts);
        }

        public static Texture2D GetPreview(CharacterConfig enemy)
        {
            if (enemy == null || string.IsNullOrWhiteSpace(enemy.PrefabPath))
                return null;

            GameObject prefab = Resources.Load<GameObject>(enemy.PrefabPath);

            return prefab != null ? AssetPreview.GetAssetPreview(prefab) : null;
        }

        private static bool HasResistance(MeleeConfig enemy)
            => enemy.DamageResistanceType != DamageTypes.None && enemy.DamageResistanceIndex > 0f;

        private static string DescribeResistance(MeleeConfig enemy)
        {
            if (HasResistance(enemy) == false)
                return "Сопротивления нет";

            string damageType = LevelEditorTexts.DescribeDamageType(enemy.DamageResistanceType);
            string percent = LevelEditorTexts.Number(enemy.DamageResistanceIndex * PERCENT_SCALE);
            string effectiveHealth = LevelEditorTexts.Number(LevelStatsCalculator.CalculateEffectiveHealth(enemy, enemy.DamageResistanceType));

            return $"Сопротивление {damageType} урону {percent}% (EHP {effectiveHealth})";
        }
    }
}
