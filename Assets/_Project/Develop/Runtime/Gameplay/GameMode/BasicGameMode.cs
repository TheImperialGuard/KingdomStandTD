using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.GameMode
{
    // Уровень кампании: победа — когда пройден последний стейдж, спавн окончен и врагов на поле не осталось.
    public class BasicGameMode : GameModeBase
    {
        private readonly StagesCycle _stagesCycle;
        private readonly WavesSpawner _wavesSpawner;
        private readonly PlayerHealth _playerHealth;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private CompositeCondition _winCondition = new();

        public BasicGameMode(
            StagesCycle stagesCycle,
            WavesSpawner wavesSpawner,
            PlayerHealth playerHealth,
            GameplayPopupService popupService,
            EntitiesLifeContext entitiesLifeContext)
            : base(stagesCycle, wavesSpawner, playerHealth, popupService)
        {
            _stagesCycle = stagesCycle;
            _wavesSpawner = wavesSpawner;
            _playerHealth = playerHealth;
            _entitiesLifeContext = entitiesLifeContext;

            CreateWinCondition();
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (_winCondition.Evaluate() == true)
                ProcessWin();
        }

        private void CreateWinCondition()
        {
            _winCondition
                .Add(new FuncCondition(() => _stagesCycle.InLastStage == true))
                .Add(new FuncCondition(() => _wavesSpawner.IsSpawnComplete == true))
                .Add(new FuncCondition(() => HasAliveEnemies() == false));
        }

        // Враги появляются не только из волн (например, призыв), поэтому считаются все сущности команды врагов.
        private bool HasAliveEnemies()
        {
            IReadOnlyList<Entity> entities = _entitiesLifeContext.Entities;

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].TryGetTeam(out ReactiveVariable<Teams> team) && team.Value == Teams.Enemies)
                    return true;
            }

            return false;
        }

        private void ProcessWin()
        {
            LevelResults levelResults = CalculateWinResults();

            ProcessEndGame(levelResults);
        }

        private LevelResults CalculateWinResults()
        {
            LevelResults results;

            int currentHealth = _playerHealth.Current.Value;
            int maxHealth = _playerHealth.Max.Value;

            float relation = (float)currentHealth / (float)maxHealth;

            if (relation == 1f)
                results = LevelResults.Perfect;
            else if (relation >= 0.5f)
                results = LevelResults.Average;
            else
                results = LevelResults.Bad;

            return results;
        }
    }
}
