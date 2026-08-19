using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class SelfReleaseEffectView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _effectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;

        private void OnDestroy()
        {
            Instantiate(_effectPrefab, _effectSpawnPoint.position, Quaternion.identity);
        }
    }
}
