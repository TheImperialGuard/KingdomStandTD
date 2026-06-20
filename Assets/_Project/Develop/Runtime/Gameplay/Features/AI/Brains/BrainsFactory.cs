using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        private AIStateMachine CreateWaypointMovementStateMachine(Entity entity)
        {
            WaypointsMovementState waypointsMovementState = new(entity);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(waypointsMovementState);

            return stateMachine;
        }
    }
}
