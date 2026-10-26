using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers
{
    // Эффекты появления башни, общие для всех башен: постройка — на I уровне, улучшение — на II–IV.
    // На конфиг ссылается TowerAppearanceView каждого префаба башни
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Towers/NewTowerAppearanceEffectsConfig", fileName = "TowerAppearanceEffectsConfig")]
    public class TowerAppearanceEffectsConfig : ScriptableObject
    {
        [field: Tooltip("Эффект у основания построенной башни")]
        [field: SerializeField] public ParticleSystem BuildEffect { get; private set; }

        [field: Tooltip("Эффект у основания улучшенной башни")]
        [field: SerializeField] public ParticleSystem UpgradeEffect { get; private set; }

        [field: Tooltip("Вспышка на верху улучшенной башни")]
        [field: SerializeField] public ParticleSystem UpgradeTopEffect { get; private set; }

        [field: Header("Пружина: модель появляется сжатой и с отскоком выпрямляется")]
        [field: SerializeField] public bool IsPopEnabled { get; private set; } = true;

        [field: SerializeField, Min(0f)] public float PopDuration { get; private set; } = 0.35f;

        [field: Tooltip("Масштаб модели в начале пружины относительно обычного")]
        [field: SerializeField] public Vector3 PopStartScale { get; private set; } = new Vector3(1.15f, 0.6f, 1.15f);
    }
}
