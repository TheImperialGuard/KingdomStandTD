using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/NewLevelConfig", fileName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [field: SerializeField] public string PrefabPath {  get; private set; }

        [field: SerializeField] public EnemiesWavesListConfig WavesConfig { get; private set; }
    }
}
