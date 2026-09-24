using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class AIParallelState : ParallelState<IUpdatableState>, IUpdatableState
    {
        public AIParallelState(params IUpdatableState[] states) : base(states)
        {
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < States.Count; i++)
            {
                States[i].Update(deltaTime);
            }
        }
    }
}
