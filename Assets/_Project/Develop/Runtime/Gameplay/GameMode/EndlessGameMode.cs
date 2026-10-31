using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Conditions;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    // Бесконечный режим: победы нет, волны идут, пока у замка не кончится здоровье. Досрочный вызов волны закрыт, пока
    // на поле босс или толпа.
    public class EndlessGameMode : GameModeBase
    {
        private readonly ICondition _skipStageCondition;

        public EndlessGameMode(
            StagesCycle stagesCycle,
            WavesSpawner wavesSpawner,
            PlayerHealth playerHealth,
            GameplayPopupService popupService,
            ICondition skipStageCondition)
            : base(stagesCycle, wavesSpawner, playerHealth, popupService)
        {
            _skipStageCondition = skipStageCondition;
        }

        protected override bool CanSkipStage() => _skipStageCondition.Evaluate();
    }
}
