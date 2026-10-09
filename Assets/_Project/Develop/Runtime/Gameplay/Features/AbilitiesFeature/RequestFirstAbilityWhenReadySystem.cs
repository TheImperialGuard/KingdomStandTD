using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    // Автокаст: просит способность, пока она не в перезарядке и не в процессе. Остальные ограничения — в CanStartFirstAbility.
    public class RequestFirstAbilityWhenReadySystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _inCooldown;
        private ReactiveVariable<bool> _inProcess;

        private ReactiveEvent _startAbilityRequest;

        public void OnInit(Entity entity)
        {
            _inCooldown = entity.InFirstAbilityCooldown;
            _inProcess = entity.InFirstAbilityProcess;

            _startAbilityRequest = entity.StartFirstAbilityRequest;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inCooldown.Value || _inProcess.Value)
                return;

            _startAbilityRequest.Invoke();
        }
    }
}
