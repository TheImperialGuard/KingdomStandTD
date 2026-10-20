using Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet
{
    // Золото за досрочный вызов волны всплывает «+N» у числа золота.
    public class GoldGainPresenter : IPresenter
    {
        private readonly GoldGainView _view;
        private readonly EarnGoldOnSkipStageService _earnGoldOnSkipStageService;

        public GoldGainPresenter(GoldGainView view, EarnGoldOnSkipStageService earnGoldOnSkipStageService)
        {
            _view = view;
            _earnGoldOnSkipStageService = earnGoldOnSkipStageService;
        }

        public void Initialize()
        {
            _earnGoldOnSkipStageService.Earned += OnGoldEarned;
        }

        public void Dispose()
        {
            _earnGoldOnSkipStageService.Earned -= OnGoldEarned;
        }

        private void OnGoldEarned(int gold) => _view.Show(gold);
    }
}
