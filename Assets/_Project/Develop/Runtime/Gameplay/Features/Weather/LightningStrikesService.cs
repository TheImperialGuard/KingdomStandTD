using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Utilities.ParticleEffects;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Weather
{
    // Молнии, пока их включил сервис погоды: удар через случайную паузу, в дорогу или в поле вне дороги. Удар бьёт
    // наземных врагов в радиусе, урон растёт по волнам по таблице множителя. Вид — эффект частиц с громом и вспышка солнца.
    public class LightningStrikesService
    {
        private readonly LightningConfig _config;
        private readonly LevelPointsSampler _levelPointsSampler;
        private readonly AreaDamageService _areaDamageService;
        private readonly ParticleEffectsPool _particleEffectsPool;
        private readonly WeatherLighting _weatherLighting;
        private readonly StageProviderService _stageProviderService;
        private readonly WaveValueTable _damageMultiplier;

        private bool _isActive;
        private float _timeToStrike;

        // damageMultiplier — множитель урона по номеру волны
        public LightningStrikesService(
            LightningConfig config,
            LevelPointsSampler levelPointsSampler,
            AreaDamageService areaDamageService,
            ParticleEffectsPool particleEffectsPool,
            WeatherLighting weatherLighting,
            StageProviderService stageProviderService,
            WaveValueTable damageMultiplier)
        {
            _config = config;
            _levelPointsSampler = levelPointsSampler;
            _areaDamageService = areaDamageService;
            _particleEffectsPool = particleEffectsPool;
            _weatherLighting = weatherLighting;
            _stageProviderService = stageProviderService;
            _damageMultiplier = damageMultiplier;
        }

        // Включённые молнии первый раз бьют не сразу, а через обычную паузу
        public void SetActive(bool isActive)
        {
            if (isActive && _isActive == false)
                _timeToStrike = GetRandomInterval();

            _isActive = isActive;
        }

        public void Update(float deltaTime)
        {
            if (_isActive == false)
                return;

            _timeToStrike -= deltaTime;

            if (_timeToStrike > 0f)
                return;

            Strike();

            _timeToStrike = GetRandomInterval();
        }

        private void Strike()
        {
            bool isRoadStrike = Random.value < _config.RoadStrikesShare;
            Vector3 point = isRoadStrike ? _levelPointsSampler.GetRandomRoadPoint() : _levelPointsSampler.GetRandomOffroadPoint();

            int waveNumber = _stageProviderService.CurrentStageNumber.Value;
            float damage = _config.BaseDamage * _damageMultiplier.GetValue(waveNumber);

            int hitEnemiesCount = _areaDamageService.DealToGroundEnemies(point, _config.Radius, damage, _config.DamageType);

            _particleEffectsPool.Spawn(_config.StrikeEffect, point);
            _weatherLighting.Flash(_config.FlashIntensity, _config.FlashDuration);

            string place = isRoadStrike ? "дорогу" : "поле";

            Debug.Log($"Молния в {place} {point}: урон {damage}, задето врагов {hitEnemiesCount}");
        }

        private float GetRandomInterval() => Random.Range(_config.MinInterval, _config.MaxInterval);
    }
}
