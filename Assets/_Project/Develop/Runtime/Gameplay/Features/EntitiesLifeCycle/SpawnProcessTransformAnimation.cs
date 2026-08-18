using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EntitiesLifeCycle
{
    public class SpawnProcessTransformAnimation : EntityView
    {
        [SerializeField] private Vector3 _fromPos;
        [SerializeField] private Quaternion _fromRotation;
        [SerializeField] private Vector3 _fromScale;
        [SerializeField] private Vector3 _toPos;
        [SerializeField] private Quaternion _toRotation;
        [SerializeField] private Vector3 _toScale;

        private ReactiveVariable<bool> _inSpawnProcess;
        private ReactiveVariable<float> _spawnProcessCurrentTime;
        private ReactiveVariable<float> _spawnProcessInitialTime;

        private float _progress;

        private bool _isRunning = false;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _inSpawnProcess = entity.InSpawnProcess;
            _spawnProcessCurrentTime = entity.SpawnProcessTimerC.CurrentTime;
            _spawnProcessInitialTime = entity.SpawnProcessTimerC.InitialTime;

            _fromRotation = _fromRotation.normalized;
            _toRotation = _toRotation.normalized;

            _isRunning = true;
        }

        private void Update()
        {
            if (_isRunning == false || _inSpawnProcess.Value == false)
                return;

            _progress = _spawnProcessCurrentTime.Value / _spawnProcessInitialTime.Value;

            if (_fromPos != _toPos)
                transform.position = Vector3.Lerp(_fromPos, _toPos, _progress);

            if (_fromRotation != _toRotation)
                transform.rotation = Quaternion.Lerp(_fromRotation, _toRotation, _progress);

            if ( _fromScale != _toScale)
                transform.localScale = Vector3.Lerp(_fromScale, _toScale, _progress);
        }
    }
}
