using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    // Бесконечный режим: победы нет, волны идут, пока у замка не кончится здоровье.
    public class EndlessGameMode : GameModeBase
    {
        public EndlessGameMode(
            StagesCycle stagesCycle,
            WavesSpawner wavesSpawner,
            PlayerHealth playerHealth,
            GameplayPopupService popupService)
            : base(stagesCycle, wavesSpawner, playerHealth, popupService)
        {
        }
    }
}
