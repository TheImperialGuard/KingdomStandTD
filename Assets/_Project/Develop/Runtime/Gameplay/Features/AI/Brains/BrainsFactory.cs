using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.TargetSelecting;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.Brains
{
    public class BrainsFactory
    {
        private readonly DIContainer _container;
        private readonly AIBrainsContext _brainsContext;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public BrainsFactory(DIContainer container)
        {
            _container = container;
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        public StateMachineBrain CreateMeleeBrain(Entity entity)
        {
            AIStateMachine stateMachine = CreateWaypointMovementStateMachine(entity);

            StateMachineBrain brain = new(stateMachine);

            _brainsContext.SetBrainsFor(entity, brain);

            return brain;
        }

        public StateMachineBrain CreateTowerBrain(Entity entity, ITargetSelector targetSelector)
        {
            FindTargetState findTargetState = new FindTargetState(targetSelector, _entitiesLifeContext, entity);
            AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

            ICompositeCondition fromFindTargetToAttackTriggerStateCondition = new CompositeCondition()
                .Add(entity.CanStartAttack)
                .Add(new FuncCondition(() => entity.InstantShotDirection.Value != null));

            ICondition fromAttackTriggerToFindTargetStateCondition = new FuncCondition(() => entity.CanStartAttack.Evaluate() == false);

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
            AIStateMachine stateMachine = CreateMoveRotateToTargetStateMachine(entity);

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
