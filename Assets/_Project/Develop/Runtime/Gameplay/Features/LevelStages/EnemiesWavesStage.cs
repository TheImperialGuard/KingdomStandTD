using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    public class EnemiesWavesStage : IStage
    {
        private readonly TimerServiceFactory _timerServiceFactory;
        private readonly WavesSpawner _wavesSpawner;

        private readonly EnemiesWavesStageConfig _config;

        private readonly float _stageTime;
        private readonly float _timeToSkipStage;

        private ReactiveEvent _completed = new();
        private ReactiveEvent _canBeSkiped = new();

        private ReactiveVariable<float> _remainingTime;

        private bool _inProcess;

        private TimerService _timer;

        private List<IDisposable> _disposables = new();

        public EnemiesWavesStage(
            EnemiesWavesStageConfig config,
            TimerServiceFactory timerServiceFactory,
            WavesSpawner wavesSpawner)
        {
            _config = config;

            _stageTime = _config.StageTime;
            _timeToSkipStage = _config.TimeToSkipStage;

            _remainingTime = new(_stageTime);

            _timerServiceFactory = timerServiceFactory;
            _wavesSpawner = wavesSpawner;
        }

        public IReadOnlyEvent Completed => _completed;

        public IReadOnlyEvent CanBeSkiped => _canBeSkiped;

        public IReadOnlyVariable<float> RemainingTime => _remainingTime;

        public void Start()
        {
            if (_inProcess)
                throw new InvalidOperationException("Game mode already started");

            CreateStageTimer();
            StartWavesSpawnProcess();

            _timer.Restart();

            _inProcess = true;
        }

        public void Update(float deltaTime)
        {
            if (_inProcess == false)
                return;

            if (StageSkipTimerIsOut())
                _canBeSkiped.Invoke();
        }

        public void Cleanup()
        {
            _inProcess = false;

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
        }

        public void Dispose()
        {
            _inProcess = false;

            _wavesSpawner.Dispose();

            foreach (IDisposable disposable  in _disposables)
                disposable.Dispose();
        }

        private void OnStageTimeOut() => ProcessEnd();

        private void OnTimerCurrentTimeChanged(float oldTime, float newTime) => _remainingTime.Value = newTime;

        private void ProcessEnd()
        {
            _inProcess = false;
            _completed.Invoke();
        }

        private void CreateStageTimer()
        {
            _timer = _timerServiceFactory.Create(_stageTime);

            _disposables.Add(_timer);
            _disposables.Add(_timer.CooldownEnded.Subscribe(OnStageTimeOut));
            _disposables.Add(_timer.CurrentTime.Subscribe(OnTimerCurrentTimeChanged));
        }

        private void StartWavesSpawnProcess()
        {
            foreach (EnemiesWaveConfig waveConfig in _config.EnemiesWaveConfigs)
                _wavesSpawner.SpawnWave(waveConfig);
        }

        private bool StageSkipTimerIsOut() => _timer.CurrentTime.Value <= (_stageTime - _timeToSkipStage);
    }
}
