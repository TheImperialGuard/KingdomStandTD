using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    // Ставит сцене свет уровня, если он задан: солнце и градиентную подсветку окружения. Прежний свет возвращать
    // не нужно: сцена геймплея загружается заново при каждом входе.
    public class LevelLightingService : IInitializable
    {
        private readonly Level _level;
        private readonly Light _sun;

        public LevelLightingService(Level level, Light sun)
        {
            _level = level;
            _sun = sun;
        }

        public void Initialize()
        {
            LevelLightingConfig lighting = _level.Lighting;

            if (lighting == null)
                return;

            ApplySun(lighting);
            ApplyAmbient(lighting);
        }

        private void ApplySun(LevelLightingConfig lighting)
        {
            if (_sun == null)
                return;

            _sun.color = lighting.SunColor;
            _sun.intensity = lighting.SunIntensity;
            _sun.transform.rotation = Quaternion.Euler(lighting.SunRotation);
            _sun.shadowStrength = lighting.SunShadowStrength;
        }

        private void ApplyAmbient(LevelLightingConfig lighting)
        {
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = lighting.AmbientSkyColor;
            RenderSettings.ambientEquatorColor = lighting.AmbientEquatorColor;
            RenderSettings.ambientGroundColor = lighting.AmbientGroundColor;
        }
    }
}
