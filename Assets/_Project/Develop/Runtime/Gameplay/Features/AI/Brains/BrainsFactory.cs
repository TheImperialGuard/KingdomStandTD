using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.SnowballThrowFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains
{
    public class BrainsFactory
    {
        private readonly DIContainer _container;
        private readonly AIBrainsContext _brainsContext;
        private readonly TargetCandidatesService _targetCandidatesService;

        public BrainsFactory(DIContainer container)
        {
            _container = container;
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _targetCandidatesService = _container.Resolve<TargetCandidatesService>();
        }

        public StateMachineBrain CreateMeleeBrain(Entity entity)
        {
            AIStateMachine stateMachine = entity.HasComponent<SnowballThrowTarget>()
                ? CreateSnowballThrowerStateMachine(entity)
                : CreateWaypointMovementStateMachine(entity);

            StateMachineBrain brain = new(stateMachine);

            _brainsContext.SetBrainsFor(entity, brain);

            return brain;
        }

        public StateMachineBrain CreateTowerBrain(Entity entity, ITargetSelector targetSelector)
        {
            FindTargetState findTargetState = new FindTargetState(targetSelector, _targetCandidatesService, entity);
            AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

            // Замороженная башня не стреляет: это единственный вход и атак, и способностей башни
            // (AttackTriggerState → StartAttackRequest). Выстрел, начатый до заморозки, долетает.
            ICompositeCondition fromFindTargetToAttackTriggerStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentTarget.Value != null))
                .Add(new FuncCondition(() => entity.IsFrozen.Value == false));

            ICondition fromAttackTriggerToFindTargetStateCondition = new FuncCondition(() => entity.InAttackProcess.Value == false);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(findTargetState);
            stateMachine.AddState(attackTriggerState);

            stateMachine.AddTransition(findTargetState, attackTriggerState, fromFindTargetToAttackTriggerStateCondition);
            stateMachine.AddTransition(attackTriggerState, findTargetState, fromAttackTriggerToFindTargetStateCondition);

            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetBrainsFor(entity, brain);

            return brain;
        }

        public StateMachineBrain CreateMagicProjectileBrain(Entity entity)
        {
            AIStateMachine movementState = CreateMoveRotateToTargetStateMachine(entity);

            SelfReleaseTriggerState selfReleaseTriggerState = new SelfReleaseTriggerState(entity);

            ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

            // Одним условием: CompositeCondition вычисляет все части, и при пустой цели упал бы на IsDead
            ICondition fromMovementToSelfReleaseStateCondition = new FuncCondition(
                () => currentTarget.Value == null || currentTarget.Value.IsDead.Value == true);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(movementState);
            stateMachine.AddState(selfReleaseTriggerState);

            stateMachine.AddTransition(movementState, selfReleaseTriggerState, fromMovementToSelfReleaseStateCondition);

            StateMachineBrain brain = new(stateMachine);

            _brainsContext.SetBrainsFor(entity, brain);

            return brain;
        }

        private AIStateMachine CreateWaypointMovementStateMachine(Entity entity)
        {
            WaypointsMovementState waypointsMovementState = new(entity);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(waypointsMovementState);

            return stateMachine;
        }

        // Мозг пишет направление поворота каждый кадр и тикает после сущностей, поэтому развернуть бросающего к башне на
        // время каста может только он сам — вторым состоянием. Первое добавленное состояние — начальное.
        private AIStateMachine CreateSnowballThrowerStateMachine(Entity entity)
        {
            WaypointsMovementState waypointsMovementState = new(entity);
            RotateToTargetOnGroundState rotateToThrowTargetState = new(entity, entity.SnowballThrowTarget);

            ICondition fromMovementToRotateStateCondition = new FuncCondition(() => entity.InFirstAbilityProcess.Value);
            ICondition fromRotateToMovementStateCondition = new FuncCondition(() => entity.InFirstAbilityProcess.Value == false);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(waypointsMovementState);
            stateMachine.AddState(rotateToThrowTargetState);

            stateMachine.AddTransition(waypointsMovementState, rotateToThrowTargetState, fromMovementToRotateStateCondition);
            stateMachine.AddTransition(rotateToThrowTargetState, waypointsMovementState, fromRotateToMovementStateCondition);

            return stateMachine;
        }

        private AIStateMachine CreateMoveRotateToTargetStateMachine(Entity entity)
        {
            MoveToTargetState moveToTargetState = new(entity);
            RotateToTargetState rotateToTargetState = new(entity);

            AIParallelState moveRotateToTargetState = new AIParallelState(moveToTargetState, rotateToTargetState);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(moveRotateToTargetState);

            return stateMachine;
        }
    }
}
