using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private List<EnemiesWavesStageConfig> _enemiesWavesStageConfigs;
        [SerializeField] private List<Transform> _towersPositions;
        [SerializeField] private GameObject _staticGroup;

        [Header("Объекты вне игрового поля: тайл спавна врагов, концы дороги")]
        [SerializeField] private List<GameObject> _boundsExcludedObjects;

        private Bounds _fieldBounds;
        private bool _hasFieldBounds;

        public IReadOnlyList<EnemiesWavesStageConfig> EnemiesWavesStageConfigs => _enemiesWavesStageConfigs;
        public IReadOnlyList<Transform> TowersPositions => _towersPositions;

        public Bounds FieldBounds => _fieldBounds;
        public bool HasFieldBounds => _hasFieldBounds;

        public void CombineStaticGroup() => StaticBatchingUtility.Combine(_staticGroup);

        // Границы считаются до CombineStaticGroup: после склейки геометрия
        // перестраивается, и опираться на неё для замеров не стоит.
        private void Awake()
        {
            _hasFieldBounds = TryCalculateFieldBounds(out _fieldBounds);

            if (_hasFieldBounds == false)
                Debug.LogWarning($"Не удалось вычислить границы поля для уровня {name}.", this);
        }

        private bool TryCalculateFieldBounds(out Bounds bounds)
        {
            bounds = new Bounds();

            if (_staticGroup == null)
                return false;

            MeshRenderer[] renderers = _staticGroup.GetComponentsInChildren<MeshRenderer>();
            bool isInitialized = false;

            foreach (MeshRenderer renderer in renderers)
            {
                if (IsExcluded(renderer.transform))
                    continue;

                if (isInitialized == false)
                {
                    bounds = renderer.bounds;
                    isInitialized = true;
                    continue;
                }

                bounds.Encapsulate(renderer.bounds);
            }

            return isInitialized;
        }

        private bool IsExcluded(Transform target)
        {
            if (_boundsExcludedObjects == null)
                return false;

            foreach (GameObject excluded in _boundsExcludedObjects)
            {
                if (excluded == null)
                    continue;

                if (target.IsChildOf(excluded.transform))
                    return true;
            }

            return false;
        }
    }
}
