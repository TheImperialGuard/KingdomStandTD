using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.Abilities
{
    public class StunAbility : Ability, IDisposable
    {
        private readonly Entity _entity;
        private readonly StunAbilityConfig _config;

        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;
        private readonly StatusesFactory _statusesFactory;

        private IDisposable _currentLevelChangedDisposable;

        public StunAbility(
            Entity entity,
            StunAbilityConfig config,
            int currentLevel,
            AreaEntitiesDetectorService areaEntitiesDetectorService,
            StatusesFactory statusesFactory) : base(config.ID, currentLevel, config.MaxLevel)
        {
            _entity = entity;
            _config = config;
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
            _statusesFactory = statusesFactory;
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

        private void OnCurrentLevelChaged(int previousLevel, int newLevel) => SetStunDuration(newLevel);

        private void SetStunDuration(int newLevel)
        {
            _entity.StunDuration.Value = _config.GetStunTimeBy(newLevel);
        }

        private void AddComponents()
        {
            _entity
                .AddSecondAbilityCooldownTimer(new(_config.Cooldown), new(_config.Cooldown), new())
                .AddSecondAbilityDelayEndEvent()
                .AddSecondAbilityDelayModifiedTime(new(_config.Delay))
                .AddSecondAbilityDelayTime(new(_config.Delay))
                .AddSecondAbilityProcessTimer(new(_config.InitialTime), new(_config.InitialTime), new())
                .AddEndSecondAbilityEvent()
                .AddInSecondAbilityCooldown()
                .AddInSecondAbilityProcess()
                .AddStartSecondAbilityEvent()
                .AddStartSecondAbilityRequest()
                .AddStunDuration(new(_config.GetStunTimeBy(CurrentLevel.Value)))
                .AddAreaEffectRadius(new(_config.Radius));

            FuncCondition inFirstAbilityCooldownCondition = new FuncCondition(() =>
            {
                if (_entity.TryGetInFirstAbilityCooldown(out ReactiveVariable<bool> inFirstAbilityCooldown) == false)
                    return true;

                return inFirstAbilityCooldown.Value;
            });

            ICompositeCondition canStartAbility = new CompositeCondition()
                .Add(inFirstAbilityCooldownCondition)
                .Add(new FuncCondition(() => _entity.InSecondAbilityCooldown.Value == false))
                .Add(new FuncCondition(() => _entity.InSecondAbilityProcess.Value == false));

            _entity.AddCanStartSecondAbility(canStartAbility);

            _entity
                .AddSystem(new SecondAbilityCooldownByProcessTimeSyncSystem())
                .AddSystem(new SecondAbilityCooldownTimerSystem())
                .AddSystem(new RequestSecondAbilityOnAttackRequestSystem())
                .AddSystem(new StartSecondAbilitySystem())
                .AddSystem(new SecondAbilityProcessTimerSystem())
                .AddSystem(new SecondAbilityDelayEndTriggerSystem())
                .AddSystem(new StunEntitiesInAreaSecondAbilitySystem(_areaEntitiesDetectorService, _statusesFactory))
                .AddSystem(new EndSecondAbilitySystem());
        }
    }
}
