#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    // Плейтест из редактора уровней со стороны игры: что редакторская часть подставляет вместо обычного запуска
    // и о чём игра ей сообщает для лога. Как и PlaytestHooks, существует только в редакторе.
    public interface IPlaytestSession
    {
        // Аргументы для перехода в Gameplay сразу из точки входа; null, если их уже забрали.
        GameplayInputArgs TakeStartArgs();

        // Конфиг уровня для очередного прогона — вместо уровня кампании по номеру.
        LevelConfig CreateLevelConfig();

        // Регистрации сцены Gameplay, которые нужны плейтесту, — лог прогона.
        void ProcessRegistrations(DIContainer container);

        void OnGameEnded(LevelResults results);

        void OnEnemySpawned(Entity enemy, CharacterConfig enemyConfig, EnemiesWaveConfig wave);

        void OnTowerBought(TowerTypes towerType, int level, int cost);

        void OnTowerSold(TowerTypes towerType, int level, int gold);

        void OnAbilityBought(TowerTypes towerType, AbilityConfig ability, int cost);

        void OnSkipGoldEarned(int gold);
    }
}
#endif
