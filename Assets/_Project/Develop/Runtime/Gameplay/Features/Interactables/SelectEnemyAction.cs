using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Gameplay;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class SelectEnemyAction : IInteractAction
    {
        private readonly Entity _source;

        private readonly GameplayPopupService _popupService;

        public SelectEnemyAction(Entity source, GameplayPopupService popupService)
        {
            _source = source;
            _popupService = popupService;
        }

        public void Do()
        {
            _popupService.OpenEnemyInfoPopup(_source);
        }
    }
}
