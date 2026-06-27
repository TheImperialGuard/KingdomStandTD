using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class InstantShotDirectionArgs
    {
        private int _angle;
        private int _projectileCounts;

        public InstantShotDirectionArgs(int angle, int projectileCounts)
        {
            _angle = angle;
            _projectileCounts = projectileCounts;
        }

        public int Angle => _angle;
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
