using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Interactables
{
    public class BuildTowerAction : IInteractAction
    {
        private readonly Entity _source;

        public BuildTowerAction(Entity source)
        {
            _source = source;
        }

        public void Do()
        {
            Debug.Log("Открыт попап постройки башни");
        }
    }
}
