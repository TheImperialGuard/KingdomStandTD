using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Infrastructure.DI;

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
            return new BuildTowerAction(source);
        }
    }
}
