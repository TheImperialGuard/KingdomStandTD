using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature
{
    // Имена методов совпадают с событиями клипа Throw: без них Animator пишет в консоль «has no receiver».
    public class SnowballThrowAnimationEventReceiver : MonoBehaviour
    {
        public event Action SnowballSpawned;
        public event Action SnowballReleased;

        [SerializeField] private Transform _releasePoint;

        public Transform ReleasePoint => _releasePoint;

        public void SpawnSnowball() => SnowballSpawned?.Invoke();

        public void ReleaseSnowball() => SnowballReleased?.Invoke();
    }
}
