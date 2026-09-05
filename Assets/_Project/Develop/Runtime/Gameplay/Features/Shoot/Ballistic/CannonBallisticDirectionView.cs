using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic
{
    public class CannonBallisticDirectionView : EntityView
    {
        [SerializeField] private Transform _rotatable;
        [SerializeField, Min(0.01f)] private float _speed = 900;

        private ReactiveVariable<TrajectoryResult> _trajectory;

        private bool _isInit;

        protected override void OnEntityStartedWork(Entity entity)
        {
            if (entity.TryGetComponent(out IsDemo isDemo) == true)
                return;

            _trajectory = entity.BallisticTrajectory;

            _isInit = true;
        }

        private void Update()
        {
            if (_isInit == false)
                return;

            if (_trajectory.Value.isValid)
            {
                Vector3 directionToTarget = _trajectory.Value.launchDirection;
                directionToTarget.x = 0;

                if (directionToTarget.sqrMagnitude < 0.001f)
                    return;

                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

                float step = _speed * Time.deltaTime;

                _rotatable.rotation = Quaternion.RotateTowards(_rotatable.rotation, lookRotation, step);
            }
        }
    }
}
