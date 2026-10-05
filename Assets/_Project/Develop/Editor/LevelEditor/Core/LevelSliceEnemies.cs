using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Враги в срезах — имена ассетов CharacterConfig. Имя ищется среди всех CharacterConfig проекта,
    // поэтому должно быть уникальным: врага с повторяющимся именем срез применить не может.
    public class LevelSliceEnemies
    {
        private readonly Dictionary<string, CharacterConfig> _enemiesByName = new(StringComparer.Ordinal);
        private readonly HashSet<string> _duplicatedNames = new(StringComparer.Ordinal);

        public LevelSliceEnemies(IEnumerable<CharacterConfig> enemies)
        {
            foreach (CharacterConfig enemy in enemies)
            {
                if (_enemiesByName.TryAdd(enemy.name, enemy) == false)
                    _duplicatedNames.Add(enemy.name);
            }
        }

        public IReadOnlyDictionary<string, CharacterConfig> ByName => _enemiesByName;

        // Почему врага нельзя найти по имени; null — враг найден однозначно.
        public string FindProblem(string enemyName)
        {
            if (_duplicatedNames.Contains(enemyName))
                return $"несколько ассетов врага с именем {enemyName}";

            if (_enemiesByName.ContainsKey(enemyName) == false)
                return $"враг {enemyName} не найден среди CharacterConfig";

            return null;
        }
    }
}
