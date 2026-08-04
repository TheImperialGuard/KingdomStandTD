using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/NewGameConfig", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float GoldIndexForRemainingStageSeconds = 1f;

        [field: SerializeField, Min(0)] public int MaxGoldForSkipStage = 20;
    }
}
