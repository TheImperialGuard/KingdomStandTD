using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public List<int> LevelsCosts { get; private set; }

        public int MaxLevel => LevelsCosts.Count;

        //meta-data
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        public bool IsUpgradable() => MaxLevel > 1;

        public int GetCostBy(int level) => LevelsCosts[level - 1];
    }
}
