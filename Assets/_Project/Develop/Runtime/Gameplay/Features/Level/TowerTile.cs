using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Level
{
    public class TowerTile : MonoBehaviour
    {
        [SerializeField] private GameObject _towerPlaceholder;

        [SerializeField] private Transform _towerPosition;

        public Vector3 TowerPosition => _towerPosition.position;

        public void ShowTowerPlaceholder() => _towerPlaceholder.SetActive(true);

        public void HideTowerPlaceholder() => _towerPlaceholder.SetActive(false);
    }
}
