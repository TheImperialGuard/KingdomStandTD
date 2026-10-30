using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Кладёт в пул объекты врагов уровня до начала боя: по одному на вид, а призываемым — весь строй, который призыв
    // создаёт за один кадр. Бросающим снежки — ещё и снежок: первый бросок не грузит префаб посреди боя.
    // Обычные враги выходят по одному, поэтому дальше пул дорастёт в бою сам. Виды врагов уровня даёт его регистрация:
    // кампания собирает их по стейджам, бесконечный режим — по тирам.
    // Перед этим склеивает части модели каждого вида в один меш: один вызов отрисовки на врага вместо вызова на часть.
    // У снежка кожи нет, склейка для него холостая.
    public class EnemiesPrewarmer
    {
        private const int InstancesPerKind = 1;
        private const int SnowballsPerThrower = 1;

        private readonly IReadOnlyList<CharacterConfig> _enemies;
        private readonly MonoEntitiesFactory _monoEntitiesFactory;

        public EnemiesPrewarmer(IReadOnlyList<CharacterConfig> enemies, MonoEntitiesFactory monoEntitiesFactory)
        {
            _enemies = enemies;
            _monoEntitiesFactory = monoEntitiesFactory;
        }

        public void Prewarm()
        {
            Dictionary<string, int> pathToCount = new();

            foreach (CharacterConfig enemy in _enemies)
                AddCount(enemy, InstancesPerKind, pathToCount);

            foreach (KeyValuePair<string, int> prefabCount in pathToCount)
            {
                _monoEntitiesFactory.CombineSkinnedParts(prefabCount.Key);
                _monoEntitiesFactory.Prewarm(prefabCount.Key, prefabCount.Value);
            }
        }

        private void AddCount(CharacterConfig config, int count, Dictionary<string, int> pathToCount)
        {
            if (TryRaiseCount(config.PrefabPath, count, pathToCount) == false)
                return;

            if (config is MeleeConfig meleeConfig == false)
                return;

            if (meleeConfig.Ability is SummonAbilityConfig summonConfig)
                AddCount(summonConfig.SummonedConfig, summonConfig.Count, pathToCount);

            if (meleeConfig.Ability is SnowballThrowAbilityConfig snowballThrowConfig)
                TryRaiseCount(snowballThrowConfig.SnowballPrefabPath, SnowballsPerThrower, pathToCount);
        }

        // Вид может прийти и в волне, и призывом — берётся наибольшее число
        private bool TryRaiseCount(string prefabPath, int count, Dictionary<string, int> pathToCount)
        {
            if (pathToCount.TryGetValue(prefabPath, out int currentCount) && currentCount >= count)
                return false;

            pathToCount[prefabPath] = count;

            return true;
        }
    }
}
