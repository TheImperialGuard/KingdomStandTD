using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class InstantShotDirectionArgs
    {
        private Vector3 _direction;
        private int _projectileCounts;

        public InstantShotDirectionArgs(Vector3 direction, int projectileCounts)
        {
            _direction = direction;
            _projectileCounts = projectileCounts;
        }

        public Vector3 Direction => _direction;
        public int ProjectileCounts
        {
            get => _projectileCounts;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _projectileCounts = value;
            }
        }
    }
}
