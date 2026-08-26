using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class OneShotAttackAbility : Ability, IDisposable
    {
        private readonly Entity _entity;
        private readonly OneShotAttackAbilityConfig _config;
        private readonly ProjectilesFactory _projectilesFactory;

        private IDisposable _currentLevelChangedDisposable;

        public OneShotAttackAbility(
            Entity entity,
            OneShotAttackAbilityConfig config,
            int currentLevel,
            ProjectilesFactory projectilesFactory) : base(config.ID, currentLevel, config.MaxLevel)
        {
            _entity = entity;
            _config = config;
            _projectilesFactory = projectilesFactory;
        }

        public override void Activate()
        {
            AddComponents();

            _currentLevelChangedDisposable = CurrentLevel.Subscribe(OnCurrentLevelChaged);
        }

        public void Dispose()
        {
            _currentLevelChangedDisposable.Dispose();
        }

        private void OnCurrentLevelChaged(int previousLevel, int newLevel) => SetAbilityCooldown(newLevel);

        private void SetAbilityCooldown(int level)
        {
            _entity.FirstAbilityCooldownTimerC.InitialTime.Value = _config.GetCooldownBy(level);
        }

        private void AddComponents()
        {
            _entity
                .AddFirstAbilityProjectileType(new(_config.ProjectileType))
                .AddFirstAbilityCooldownTimer(new(_config.GetCooldownBy(CurrentLevel.Value)), new(_config.GetCooldownBy(CurrentLevel.Value)), new())
                .AddFirstAbilityDelayEndEvent()
                .AddFirstAbilityDelayModifiedTime(new(_config.Delay))
                .AddFirstAbilityDelayTime(new(_config.Delay))
                .AddFirstAbilityProcessTimer(new(_config.InitialTime), new(_config.InitialTime), new())
                .AddEndFirstAbilityEvent()
                .AddInFirstAbilityCooldown()
                .AddInFirstAbilityProcess()
                .AddStartFirstAbilityEvent()
                .AddStartFirstAbilityRequest();

            ICompositeCondition canStartAbility = new CompositeCondition(LogicOperations.And)
                .Add(new FuncCondition(() => _entity.InAttackProcess.Value == false))
                .Add(new FuncCondition(() => _entity.InFirstAbilityCooldown.Value == false))
                .Add(new FuncCondition(() => _entity.InFirstAbilityProcess.Value == false));

            _entity.AddCanStartFirstAbility(canStartAbility);

            _entity
                .AddSystem(new FirstAbilityCooldownByProcessTimeSyncSystem())
                .AddSystem(new FirstAbilityCooldownTimerSystem())
                .AddSystem(new RequestFirstAbilityOnAttackRequestSystem())
                .AddSystem(new StartFirstAbilitySystem())
                .AddSystem(new FirstAbilityProcessTimerSystem())
                .AddSystem(new FirstAbilityDelayEndTriggerSystem())
                .AddSystem(new FirstAbilityInstantShootSystem(_projectilesFactory))
                .AddSystem(new EndFirstAbilitySystem());

            _entity.CanStartAttack
                .Add(new FuncCondition(() => _entity.InFirstAbilityCooldown.Value == true))
                .Add(new FuncCondition(() => _entity.InFirstAbilityProcess.Value == false));
        }
    }
}
