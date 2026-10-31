using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    // Регистрации сцены Gameplay, которые нужны только бесконечному уровню: волны собирает генератор по правилам
    // EndlessModeConfig, здоровье врагов растёт по волнам, досрочный вызов закрыт при боссе и толпе, после босса
    // возвращается боевая музыка. GameplayContextRegistrations вызывает их вместо стейджей кампании.
    public static class EndlessContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            container.RegisterAsSingle(CreateEndlessWavesGenerator);

            container.RegisterAsSingle(CreateEndlessStagesSource);

            container.RegisterAsSingle(CreateStagesSource);

            container.RegisterAsSingle(CreateLevelStagesContent);

            container.RegisterAsSingle(CreateEnemiesOnFieldCounter);

            container.RegisterAsSingle(CreateEndlessSkipStageCondition);

            container.RegisterAsSingle(CreateEndlessHealthMultiplierService).NonLazy();

            container.RegisterAsSingle(CreateReturnBattleMusicAfterBossService).NonLazy();
        }

        private static EndlessWavesGenerator CreateEndlessWavesGenerator(DIContainer c)
            => new(GetConfig(c), c.Resolve<Level>().RoadPaths);

        private static EndlessStagesSource CreateEndlessStagesSource(DIContainer c)
            => new(c.Resolve<EndlessWavesGenerator>(), GetFirstWaveNumber());

        // StageProviderService получает тот же экземпляр, что зарегистрирован как EndlessStagesSource
        private static IStagesSource CreateStagesSource(DIContainer c)
            => c.Resolve<EndlessStagesSource>();

        // Прогрев — все враги тиров и боссы (призываемых и снежки EnemiesPrewarmer добавит сам), превью — все дороги уровня
        private static LevelStagesContent CreateLevelStagesContent(DIContainer c)
        {
            EndlessModeConfig config = GetConfig(c);
            List<CharacterConfig> enemies = new();

            foreach (EndlessTier tier in config.Tiers)
            {
                foreach (EndlessTierEnemy tierEnemy in tier.Enemies)
                    AddDistinct(enemies, tierEnemy.EnemyConfig);
            }

            foreach (MeleeConfig boss in config.Bosses)
                AddDistinct(enemies, boss);

            return new LevelStagesContent(enemies, c.Resolve<EndlessWavesGenerator>().PathNumbers);
        }

        private static EnemiesOnFieldCounter CreateEnemiesOnFieldCounter(DIContainer c)
            => new(c.Resolve<EntitiesLifeContext>());

        private static EndlessSkipStageCondition CreateEndlessSkipStageCondition(DIContainer c)
            => new(c.Resolve<EnemiesOnFieldCounter>(), GetConfig(c));

        private static EndlessHealthMultiplierService CreateEndlessHealthMultiplierService(DIContainer c)
            => new(c.Resolve<WavesSpawner>(), c.Resolve<EndlessStagesSource>(), GetConfig(c));

        private static ReturnBattleMusicAfterBossService CreateReturnBattleMusicAfterBossService(DIContainer c)
        {
            return new ReturnBattleMusicAfterBossService(
                c.Resolve<MusicSwitcherService>(),
                c.Resolve<EnemiesOnFieldCounter>(),
                c.Resolve<PlayerHealth>());
        }

        private static EndlessModeConfig GetConfig(DIContainer c)
            => c.Resolve<ConfigsProviderService>().GetConfig<EndlessModeConfig>();

        private static int GetFirstWaveNumber()
        {
            #if UNITY_EDITOR
                // Старт с волны N — настройка инспектора EndlessModeConfig, только для проверок в редакторе
                return EndlessModeEditorSettings.StartWaveNumber;
            #else
                return EndlessStagesSource.DefaultFirstWaveNumber;
            #endif
        }

        private static void AddDistinct(List<CharacterConfig> enemies, CharacterConfig enemy)
        {
            if (enemy != null && enemies.Contains(enemy) == false)
                enemies.Add(enemy);
        }
    }
}
