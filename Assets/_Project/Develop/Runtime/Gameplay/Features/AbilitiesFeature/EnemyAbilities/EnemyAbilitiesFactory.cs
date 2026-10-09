using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Abilities.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AOE;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.WarCryFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature.EnemyAbilities
{
    // Ставит врагу способность в слот FirstAbility: общий цикл каста из систем башен и механику конкретной способности.
    public class EnemyAbilitiesFactory
    {
        private readonly DIContainer _container;

        public EnemyAbilitiesFactory(DIContainer container)
        {
            _container = container;
        }

        public void AddAbilityTo(Entity entity, EnemyAbilityConfig config)
        {
            AddCastCycleTo(entity, config);

            switch (config)
            {
                case WarCryAbilityConfig warCryConfig:
                    AddWarCryTo(entity, warCryConfig);
                    break;

                default:
                    throw new ArgumentException($"Not supported {config.GetType()} enemy ability config");
            }
        }

        // Перезарядка идёт после каста и стартует уже при появлении врага, поэтому первый каст — через полную перезарядку.
        private void AddCastCycleTo(Entity entity, EnemyAbilityConfig config)
        {
            entity
                .AddFirstAbilityCooldownTimer(new(config.Cooldown), new(config.Cooldown), new(config.Cooldown))
                .AddInFirstAbilityCooldown(new(true))
                .AddFirstAbilityProcessTimer(new(config.Duration), new(config.Duration), new())
                .AddInFirstAbilityProcess()
                .AddStartFirstAbilityRequest()
                .AddStartFirstAbilityEvent()
                .AddEndFirstAbilityEvent();

            ICompositeCondition canStartAbility = new CompositeCondition()
                .Add(new FuncCondition(() => entity.IsDead.Value == false))
                .Add(new FuncCondition(() => entity.InFirstAbilityCooldown.Value == false))
                .Add(new FuncCondition(() => entity.InFirstAbilityProcess.Value == false));

            entity
                .AddCanStartFirstAbility(canStartAbility);

            entity.CanMove.Add(new FuncCondition(() => entity.InFirstAbilityProcess.Value == false));

            entity
                .AddSystem(new RequestFirstAbilityWhenReadySystem())
                .AddSystem(new StartFirstAbilitySystem())
                .AddSystem(new FirstAbilityProcessTimerSystem())
                .AddSystem(new EndFirstAbilitySystem())
                .AddSystem(new FirstAbilityCooldownTimerSystem());
        }

        private void AddWarCryTo(Entity entity, WarCryAbilityConfig config)
        {
            entity
                .AddAreaEffectRadius(new(config.Radius))
                .AddSpeedBoostIndex(new(config.SpeedBoostIndex))
                .AddSpeedBoostDuration(new(config.SpeedBoostDuration))
                .AddWarCryTargetKinds(new(config.TargetKinds));

            entity
                .AddSystem(new WarCryZoneSystem(
                    _container.Resolve<AreaEntitiesDetectorService>(),
                    _container.Resolve<StatusesFactory>()));
        }
    }
}
