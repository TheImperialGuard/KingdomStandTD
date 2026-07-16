using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class BuildTowerAction : IInteractAction
    {
        private readonly Entity _source;

        private readonly GameplayPopupService _popupService;

        public BuildTowerAction(
            Entity source, 
            GameplayPopupService popupService)
        {
            _source = source;
            _popupService = popupService;
        }

        public void Do()
        {
            _popupService.OpenBuildTowerPopup(_source);
        }
    }
}
