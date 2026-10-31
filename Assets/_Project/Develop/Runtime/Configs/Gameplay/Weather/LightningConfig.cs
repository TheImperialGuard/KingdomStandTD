using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Weather
{
    // Молнии в грозу: как часто и куда бьют, какой урон наносят и как выглядит удар.
    [Serializable]
    public class LightningConfig
    {
        [Tooltip("Пауза между ударами, с: каждый раз случайная от меньшей до большей")]
        [SerializeField, Min(0f)] private float _minInterval = 6f;

        [SerializeField, Min(0f)] private float _maxInterval = 12f;

        [Tooltip("Доля ударов в дорогу; остальные — в поле вне дороги")]
        [SerializeField, Range(0f, 1f)] private float _roadStrikesShare = 0.4f;

        [Tooltip("Радиус удара: урон получают наземные враги в нём")]
        [SerializeField, Min(0f)] private float _radius = 1.5f;

        [Tooltip("Урон удара до умножения: дальше растёт по волнам вместе со здоровьем врагов")]
        [SerializeField, Min(0f)] private float _baseDamage = 100f;

        [SerializeField] private DamageTypes _damageType = DamageTypes.Magic;

        [Tooltip("Эффект удара: молния, искры и гром")]
        [SerializeField] private ParticleSystem _strikeEffect;

        [Tooltip("Вспышка: на сколько ярче становится солнце в момент удара")]
        [SerializeField, Min(0f)] private float _flashIntensity = 1.5f;

        [Tooltip("За сколько секунд гаснет вспышка")]
        [SerializeField, Min(0f)] private float _flashDuration = 0.3f;

        public float MinInterval => _minInterval;
        public float MaxInterval => _maxInterval;
        public float RoadStrikesShare => _roadStrikesShare;
        public float Radius => _radius;
        public float BaseDamage => _baseDamage;
        public DamageTypes DamageType => _damageType;
        public ParticleSystem StrikeEffect => _strikeEffect;
        public float FlashIntensity => _flashIntensity;
        public float FlashDuration => _flashDuration;
    }
}
