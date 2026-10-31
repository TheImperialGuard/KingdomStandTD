using UnityEngine;
using UnityEngine.Rendering;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    // Снимок света сцены: солнце и подсветка окружения. Окружение хранится сферическими гармониками — в них Unity
    // переводит любой режим подсветки (небо, градиент, цвет), поэтому два снимка можно плавно смешать, даже если
    // у одного подсветка от неба, а у другого — градиентом.
    public readonly struct SceneLighting
    {
        public readonly Color SunColor;
        public readonly float SunIntensity;
        public readonly Quaternion SunRotation;
        public readonly float SunShadowStrength;
        public readonly SphericalHarmonicsL2 Ambient;

        public SceneLighting(
            Color sunColor,
            float sunIntensity,
            Quaternion sunRotation,
            float sunShadowStrength,
            SphericalHarmonicsL2 ambient)
        {
            SunColor = sunColor;
            SunIntensity = sunIntensity;
            SunRotation = sunRotation;
            SunShadowStrength = sunShadowStrength;
            Ambient = ambient;
        }

        public static SceneLighting Capture(Light sun)
        {
            return new SceneLighting(
                sun.color,
                sun.intensity,
                sun.transform.rotation,
                sun.shadowStrength,
                RenderSettings.ambientProbe);
        }

        public static SceneLighting Lerp(SceneLighting from, SceneLighting to, float progress)
        {
            return new SceneLighting(
                Color.Lerp(from.SunColor, to.SunColor, progress),
                Mathf.Lerp(from.SunIntensity, to.SunIntensity, progress),
                Quaternion.Slerp(from.SunRotation, to.SunRotation, progress),
                Mathf.Lerp(from.SunShadowStrength, to.SunShadowStrength, progress),
                from.Ambient * (1f - progress) + to.Ambient * progress);
        }

        public SceneLighting WithSunIntensity(float sunIntensity)
            => new(SunColor, sunIntensity, SunRotation, SunShadowStrength, Ambient);

        // Гармоники, заданные вручную, Unity оставляет только в режиме подсветки от неба: в режимах градиента и цвета
        // он тут же пересчитывает их из цветов (проверено в Unity 6000.3)
        public void ApplyTo(Light sun)
        {
            sun.color = SunColor;
            sun.intensity = SunIntensity;
            sun.transform.rotation = SunRotation;
            sun.shadowStrength = SunShadowStrength;

            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientProbe = Ambient;
        }
    }
}
