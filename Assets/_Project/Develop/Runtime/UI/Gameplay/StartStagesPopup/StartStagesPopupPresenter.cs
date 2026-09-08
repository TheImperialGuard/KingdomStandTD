using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Raycast;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.Utilities.Audio;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup
{
    public class StartStagesPopupPresenter : PopupPresenterBase
    {
        private readonly StartStagesPopupView _view;

        private readonly StagesCycle _stagesCycle;
        private readonly StageProviderService _stageProviderService;
        private readonly MusicSwitcherService _musicSwitcherService;

        private int _clicks;

        public StartStagesPopupPresenter(
            StartStagesPopupView view,
            ICoroutinesPerformer coroutinesPerformer,
            StagesCycle stagesCycle,
            RayShooterService rayShooterService,
            StageProviderService stageProviderService,
            MusicSwitcherService musicSwitcherService)
            : base(coroutinesPerformer, rayShooterService)
        {
            _view = view;
            _stagesCycle = stagesCycle;
            _stageProviderService = stageProviderService;
            _musicSwitcherService = musicSwitcherService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.StartButtonClicked += OnStartButtonClicked;
            _view.SetWavesEnemiesText(GetWavesEnemiesText());

            _clicks = 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.StartButtonClicked -= OnStartButtonClicked;
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
                _musicSwitcherService.SwitchFor(MusicContexts.GameplayBattle);
                _stagesCycle.Launch();
                OnCloseRequest();
            }
            else
            {
                _view.ShowInfo();
            }
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
                string characterName = wave.EnemyConfig.CharacterName;
                int count = wave.EnemiesCount;

                if (allEnemiesInWaves.ContainsKey(characterName))
                {
                    allEnemiesInWaves[characterName] += count;
                }
                else
                {
                    allEnemiesInWaves.Add(characterName, count);
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
