using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Weather;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Endless;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;
using UnityEngine;
using Random = System.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    // Регистрации сцены Gameplay, которые нужны только бесконечному уровню: волны собирает генератор по правилам
    // EndlessModeConfig, здоровье врагов растёт по волнам, досрочный вызов закрыт при боссе и толпе, после босса
    // возвращается боевая музыка, забег засчитывается в рекорд, погода меняется по волнам. GameplayContextRegistrations
    // вызывает их вместо стейджей кампании.
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

            container.RegisterAsSingle(CreateEndlessRunRecorder);

            // Погода — только если она задана в конфиге режима
            if (GetConfig(container).Weather != null)
                RegisterWeather(container);
        }

        private static void RegisterWeather(DIContainer container)
        {
            container.RegisterAsSingle(CreateWeatherSequence);

            container.RegisterAsSingle(CreateRainView);

            // Контейнер инициализирует в порядке регистраций: свет погод снимается раньше, чем сервис погоды поставит
            // стартовую погоду
            container.RegisterAsSingle(CreateWeatherLighting).NonLazy();

            container.RegisterAsSingle(CreateLightningStrikesService);

            container.RegisterAsSingle(CreateWeatherService).NonLazy();
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

        private static EndlessRunRecorder CreateEndlessRunRecorder(DIContainer c)
        {
            return new EndlessRunRecorder(
                c.Resolve<StageProviderService>(),
                c.Resolve<EndlessStagesSource>(),
                c.Resolve<EndlessRecordService>(),
                c.Resolve<PlayerDataProvider>(),
                c.Resolve<ICoroutinesPerformer>());
        }

        // Погода не зависит от зерна волн: в каждом забеге своя
        private static WeatherSequence CreateWeatherSequence(DIContainer c)
        {
            WeatherConfig config = GetWeatherConfig(c);

            return new WeatherSequence(config, new Random(), GetForcedWeather() ?? config.StartWeather);
        }

        // Дождь — над серединой поля на высоте уровня; капли и всплески разнесены по высоте в самом префабе
        private static RainView CreateRainView(DIContainer c)
        {
            Level level = c.Resolve<Level>();
            Vector3 fieldCenter = level.FieldBounds.center;
            Vector3 position = new(fieldCenter.x, level.transform.position.y, fieldCenter.z);

            return GameObject.Instantiate(GetWeatherConfig(c).RainPrefab, position, Quaternion.identity);
        }

        private static WeatherLighting CreateWeatherLighting(DIContainer c)
            => new(c.Resolve<Level>().Lighting, RenderSettings.sun, GetWeatherConfig(c));

        // Урон молнии растёт по волнам вместе со здоровьем врагов — по той же таблице множителя
        private static LightningStrikesService CreateLightningStrikesService(DIContainer c)
        {
            return new LightningStrikesService(
                GetWeatherConfig(c).Lightning,
                c.Resolve<LevelPointsSampler>(),
                c.Resolve<AreaDamageService>(),
                c.Resolve<ParticleEffectsPool>(),
                c.Resolve<WeatherLighting>(),
                c.Resolve<StageProviderService>(),
                GetConfig(c).HealthMultiplier);
        }

        private static WeatherService CreateWeatherService(DIContainer c)
        {
            return new WeatherService(
                GetWeatherConfig(c),
                c.Resolve<WeatherSequence>(),
                c.Resolve<WeatherLighting>(),
                c.Resolve<RainView>(),
                c.Resolve<LightningStrikesService>(),
                c.Resolve<StageProviderService>(),
                GetForcedWeather().HasValue);
        }

        private static EndlessModeConfig GetConfig(DIContainer c)
            => c.Resolve<ConfigsProviderService>().GetConfig<EndlessModeConfig>();

        private static WeatherConfig GetWeatherConfig(DIContainer c) => GetConfig(c).Weather;

        private static int GetFirstWaveNumber()
        {
            #if UNITY_EDITOR
                // Старт с волны N — настройка инспектора EndlessModeConfig, только для проверок в редакторе
                return EndlessModeEditorSettings.StartWaveNumber;
            #else
                return EndlessStagesSource.DefaultFirstWaveNumber;
            #endif
        }

        private static WeatherKinds? GetForcedWeather()
        {
            #if UNITY_EDITOR
                // Принудительная погода — настройка инспектора WeatherConfig, только для проверок в редакторе
                return EndlessModeEditorSettings.ForcedWeather;
            #else
                return null;
            #endif
        }

        private static void AddDistinct(List<CharacterConfig> enemies, CharacterConfig enemy)
        {
            if (enemy != null && enemies.Contains(enemy) == false)
                enemies.Add(enemy);
        }
    }
}
