using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.Shoot;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using static UnityEngine.Rendering.STP;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.EntitiesFactory
{
    public partial class EntitiesFactory
    {
        public Entity CreateSubArrowsEntity(Entity parent)
        {
            Entity entity = CreateEmpty();

            entity
                .AddShootPoint(parent.ShootPoint)
                .AddTransform(parent.Transform);

            entity
                .AddAttackProcessInitialTime(parent.AttackProcessInitialTime)
                .AddAttackProcessModifiedTime(parent.AttackProcessModifiedTime)
                .AddAttackProcessCurrentTime()
                .AddInAttackProcess()
                .AddStartAttackRequest()
                .AddStartAttackEvent()
                .AddEndAttackEvent()
                .AddAttackDelayTime(parent.AttackDelayTime)
                .AddAttackDelayModifiedTime(parent.AttackDelayModifiedTime)
                .AddAttackDelayEndEvent()
                .AddIsSubTower()
                .AddSubTowerParent(parent)
                .AddBaseStats(parent.BaseStats)
                .AddModifiedStats(parent.ModifiedStats)
                .AddCurrentTarget()
                .AddInstantAttackDamage(parent.InstantAttackDamage)
                .AddInstantAttackDamageType(parent.InstantAttackDamageType)
                .AddInstantShootRange(parent.InstantShootRange)
                .AddInstantShotDirection()
                .AddAttackCooldownCurrentTime()
                .AddAttackCooldownInitialTime(parent.AttackCooldownInitialTime)
                .AddAttackCooldownModifiedTime(parent.AttackCooldownModifiedTime)
                .AddProjectileType(parent.ProjectileType)
                .AddProjectileSpeed(parent.ProjectileSpeed)
                .AddInAttackCooldown()
                .AddAttacksPerSecond(parent.AttacksPerSecond)
                .AddLastingDamage(parent.LastingDamage)
                .AddLastingDamageInitialTime(parent.LastingDamageInitialTime)
                .AddLastingDamageInterval(parent.LastingDamageInterval);

            ICompositeCondition canStartAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.InAttackCooldown.Value == false))
                .Add(new FuncCondition(() => entity.InAttackProcess.Value == false));

            entity
                .AddCanStartAttack(canStartAttack);

            entity
                .AddSystem(new AttackCooldownTimerSystem())
                .AddSystem(new ShootDirectionCalculateSystem())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new InstantShootSystem(_container.Resolve<ProjectilesFactory>()))
                .AddSystem(new EndAttackSystem());

            return entity;
        }
    }
}
