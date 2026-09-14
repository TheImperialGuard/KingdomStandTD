using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Gameplay.BottomInfoPopup;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class SelectTowerAction : IInteractAction
    {
        private readonly Entity _source;

        private readonly GameplayPopupService _popupService;

        public SelectTowerAction(Entity source, GameplayPopupService popupService)
        {
            _source = source;
            _popupService = popupService;
        }

        public void Do()
        {
            _source.ShootingRangeZone.Show();

            _popupService.OpenTowerInfoPopup(_source);

            if (_source.TowerLevel.Value < 4)
                _popupService.OpenUpgradeTowerPopup(_source, _source.ShootingRangeZone.Hide);
            else
                _popupService.OpenTowerAbilitiesPopup(_source, _source.ShootingRangeZone.Hide);
        }
    }
}
