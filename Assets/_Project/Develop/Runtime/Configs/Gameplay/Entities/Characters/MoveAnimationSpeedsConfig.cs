using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters
{
    // Базовая скорость клипа ходьбы — скорость движения модели масштаба 1, при которой клип с темпом ×1 выглядит естественно.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Entities/Characters/NewMoveAnimationSpeedsConfig", fileName = "MoveAnimationSpeeds")]
    public class MoveAnimationSpeedsConfig : ScriptableObject
    {
#if UNITY_EDITOR
        public event Action Changed;
#endif

        [SerializeField] private List<MoveAnimationSpeed> _speeds = new();

        public bool TryGetBaseSpeed(AnimationClip clip, out float baseSpeed)
        {
            foreach (MoveAnimationSpeed speed in _speeds)
            {
                if (speed.Clip == clip)
                {
                    baseSpeed = speed.BaseSpeed;
                    return true;
                }
            }

            baseSpeed = 0;
            return false;
        }

#if UNITY_EDITOR
        // Правки таблицы в Play Mode сразу применяются к идущим врагам
        private void OnValidate() => Changed?.Invoke();
#endif

        [Serializable]
        private class MoveAnimationSpeed
        {
            [SerializeField] private AnimationClip _clip;
            [SerializeField, Min(0.01f)] private float _baseSpeed = 1;

            public AnimationClip Clip => _clip;

            public float BaseSpeed => _baseSpeed;
        }
    }
}
