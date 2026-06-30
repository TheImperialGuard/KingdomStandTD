using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/NewLevelConfig", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath {  get; private set; }
        [field: SerializeField] public EnemiesWavesListConfig WavesConfig { get; private set; }

        [field: SerializeField, Range(1, 5)] public int ArrowsUpgradesLevelLimit { get; private set; } = 5;
        [field: SerializeField, Range(1, 5)] public int MagicUpgradesLevelLimit { get; private set; } = 5;
        [field: SerializeField, Range(1, 5)] public int CannonUpgradesLevelLimit { get; private set; } = 5;
        [field: SerializeField, Range(1, 5)] public int BarracksUpgradesLevelLimit { get; private set; } = 5;
    }
}
