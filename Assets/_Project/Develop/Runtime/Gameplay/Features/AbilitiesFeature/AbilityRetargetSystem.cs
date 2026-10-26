using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature
{
    // Пока способность готовится к срабатыванию, следит за целью: пропавшую заменяет новой по правилу башни,
    // а если выбрать некого — отменяет способность без перезарядки, и башня применит её заново при следующей атаке.
    // В фабрике ставится перед таймером процесса: замена цели должна случиться в том же кадре раньше срабатывания
    public abstract class AbilityRetargetSystem : IInitializableSystem, IUpdatableSystem
    {
        protected ReactiveVariable<Entity> CurrentTarget;
        protected ITargetSelector TargetSelector;

        protected ReactiveVariable<bool> InProcess;
        protected ReactiveVariable<float> ProcessCurrentTime;
        protected ReactiveVariable<float> Delay;

        protected ReactiveEvent CanceledEvent;

        private readonly TargetCandidatesService _targetCandidatesService;

        protected AbilityRetargetSystem(TargetCandidatesService targetCandidatesService)
        {
            _targetCandidatesService = targetCandidatesService;
        }

        public abstract void OnInit(Entity entity);

        public void OnUpdate(float deltaTime)
        {
            // После срабатывания идёт только хвост процесса: отмена в нём сняла бы уже заработанную перезарядку
            if (InProcess.Value == false || ProcessCurrentTime.Value >= Delay.Value)
                return;

            if (IsAlive(CurrentTarget.Value))
                return;

            Entity newTarget = TargetSelector.SelectTargetFrom(_targetCandidatesService.Candidates);

            if (newTarget != null)
            {
                CurrentTarget.Value = newTarget;
                return;
            }

            InProcess.Value = false;
            CanceledEvent.Invoke();
        }

        // Пропажу проверяем по самой сущности: объект освобождённой сущности из пула может уже принадлежать другой
        private bool IsAlive(Entity target)
        {
            if (target == null || target.IsInit == false)
                return false;

            if (target.TryGetIsDead(out ReactiveVariable<bool> isDead) == false)
                return true;

            return isDead.Value == false;
        }
    }
}
