using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Towers;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.UI.Gameplay;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class InteractiveActionsFactory
    {
        private readonly DIContainer _container;

        public InteractiveActionsFactory(DIContainer container)
        {
            _container = container;
        }

        public IInteractAction CreateBuildTowerAction(Entity source)
        {
            return new BuildTowerAction(source, _container.Resolve<GameplayPopupService>());
        }

        public IInteractAction CreateSelectTowerAction(Entity source)
        {
            return new SelectTowerAction(source, _container.Resolve<GameplayPopupService>());
        }
    }
}
