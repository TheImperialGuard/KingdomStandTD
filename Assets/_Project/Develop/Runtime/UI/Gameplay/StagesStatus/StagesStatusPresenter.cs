using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StagesStatus
{
    // «Волна  N/M» в HUD; у бесконечного режима числа волн нет — «Волна  N».
    public class StagesStatusPresenter : IPresenter
    {
        private const string StatusPrefix = "Волна  ";
        private const string CountSeparator = "/";

        private readonly IconTextView _view;
        private readonly StageProviderService _stagesProvider;

        private IDisposable _currentStageChangedDisposable;

        public StagesStatusPresenter(IconTextView view, StageProviderService stagesProviderService)
        {
            _view = view;
            _stagesProvider = stagesProviderService;
        }

        public void Initialize()
        {
            _currentStageChangedDisposable = _stagesProvider.CurrentStageNumber.Subscribe(OnCurrentStageChanged);

            UpdateView();
        }

        public void Dispose()
        {
            _currentStageChangedDisposable.Dispose();
        }

        private void OnCurrentStageChanged(int arg1, int arg2) => UpdateView();

        private void UpdateView()
        {
            string stagesStatus = StatusPrefix + _stagesProvider.CurrentStageNumber.Value.ToString();

            if (_stagesProvider.HasFiniteStages)
                stagesStatus += CountSeparator + _stagesProvider.StagesCount.ToString();

            _view.SetText(stagesStatus);
        }
    }
}
