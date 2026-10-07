using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup
{
    public class SkipStagePopupPresenter : PopupPresenterBase
    {
        private readonly SkipStagePopupView _view;

        private readonly StagesCycle _stagesCycle;
        private readonly StageProviderService _stageProviderService;

        private int _clicks;

        private float _lifetime;

        private IDisposable _canBeSkipedDisposable;
        private IDisposable _remainingTimeDisposable;

        public SkipStagePopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            RayShooterService rayShooterService,
            SkipStagePopupView view,
            StagesCycle stagesCycle,
            StageProviderService stageProviderService) :
            base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _stagesCycle = stagesCycle;
            _stageProviderService = stageProviderService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.StartButtonClicked += OnStartButtonClicked;

            _canBeSkipedDisposable = _stagesCycle.StageCanBeSkiped.Subscribe(OnStageCanBeSkipedChange);
            _view.SetWavesEnemiesText(GetWavesEnemiesText());

            IReadOnlyVariable<float> remainingTime = _stageProviderService.CurrentStageRemainingTime;

            _lifetime = remainingTime.Value;
            _view.SetLifetimeProgress(0f);
            _remainingTimeDisposable = remainingTime.Subscribe(OnRemainingTimeChanged);

            _clicks = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.StartButtonClicked -= OnStartButtonClicked;
            _canBeSkipedDisposable.Dispose();
            _remainingTimeDisposable.Dispose();
        }

        protected override void OnClickedOutside(RaycastHit oldHit, RaycastHit newHit)
        {
            base.OnClickedOutside(oldHit, newHit);

            _view.HideInfo();
            _clicks = 0;
        }

        private void OnStartButtonClicked()
        {
            if (_clicks++ > 0)
            {
                _stagesCycle.SkipStage();
                OnCloseRequest();
            }
            else
            {
                _view.ShowInfo();
            }
        }

        private void OnStageCanBeSkipedChange(bool oldValue, bool newValue)
        {
            if (newValue == false)
                OnCloseRequest();
        }

        private void OnRemainingTimeChanged(float oldValue, float newValue)
        {
            float progress = Mathf.InverseLerp(_lifetime, 0f, newValue);

            _view.SetLifetimeProgress(progress);
        }

        private string GetWavesEnemiesText()
        {
            StageConfig config = _stageProviderService.NextStageConfig;

            if (config is EnemiesWavesStageConfig enemiesWavesStageConfig == false)
                throw new ArgumentException($"{typeof(StageConfig)}");

            IReadOnlyList<EnemiesWaveConfig> waves = enemiesWavesStageConfig.EnemiesWaveConfigs;

            Dictionary<string, int> allEnemiesInWaves = new();

            foreach (EnemiesWaveConfig wave in waves)
            {
                foreach (WaveEnemyGroup enemyGroup in wave.EnemyGroups)
                {
                    string characterName = enemyGroup.EnemyConfig.CharacterName;
                    int count = enemyGroup.Count * wave.PatternRepeats;

                    if (allEnemiesInWaves.ContainsKey(characterName))
                    {
                        allEnemiesInWaves[characterName] += count;
                    }
                    else
                    {
                        allEnemiesInWaves.Add(characterName, count);
                    }
                }
            }

            string text = "";

            StringBuilder stringBuilder = new StringBuilder(text);

            foreach (KeyValuePair<string, int> kvp in allEnemiesInWaves)
            {
                stringBuilder
                    .Append(kvp.Key)
                    .Append(" x")
                    .Append(kvp.Value.ToString())
                    .Append(", ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);

            text = stringBuilder.ToString();

            return text;
        }
    }
}
