using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatusFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using NUnit.Framework;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesHelper
    {
        public static bool TryTakeDamageFrom(Entity source, Entity target, float damage)
        {
            if (target.TryGetTakeDamageRequest(out ReactiveEvent<float> takeDamageRequest) == false)
                return false;

            if (IsSameTeam(source, target))
                return false;

            if (source.TryGetInstantAttackDamageType(out ReactiveVariable<DamageTypes> damageType) == false)
                return false;

            if (target.TryGetDamageResistanceType(out ReactiveVariable<DamageTypes> damageResistanceType) == false)
                return false;

            if (target.TryGetDamageResistanceIndex(out ReactiveVariable<float> damageResistanceIndex) == false)
                return false;

            float finalDamage = CalculateFinalDamage(damage, damageType.Value, damageResistanceType.Value, damageResistanceIndex.Value);

            if (target.TryGetComponent(out IsBoss isBoss))
                finalDamage = GetDamageForBoss(finalDamage, target);

            takeDamageRequest.Invoke(finalDamage);

            return true;
        }

        public static bool IsSameTeam(Entity firstEntity, Entity secondEntity)
        {
            if (firstEntity.TryGetTeam(out ReactiveVariable<Teams> firstTeam)
                && secondEntity.TryGetTeam(out ReactiveVariable<Teams> secondTeam))
            {
                return firstTeam.Value == secondTeam.Value;
            }

            return false;
        }

        public static bool TryInjectStatusTo(Entity target, Status status)
        {
            if (target.TryGetStatuses(out StatusesList statuses) == false)
                return false;

            statuses.AddElement(status);

            return true;
        }

        private static float CalculateFinalDamage(
            float damage, 
            DamageTypes damageType, 
            DamageTypes damageResistanceType, 
            float damageResistanceIndex)
        {
            if (damageType == DamageTypes.None)
                throw new ArgumentException($"Dealing damage type can not be {nameof(DamageTypes.None)}");

            if (damageType != damageResistanceType || damageResistanceType == DamageTypes.None)
                return damage;

            float finalDamage = damage - damage * damageResistanceIndex;

            return finalDamage;
        }

        private static float GetDamageForBoss(float damage, Entity target)
        {
            float maxDamageForApply = target.MaxHealth.Value / 3;

            if (damage > maxDamageForApply)
                return maxDamageForApply;

            return damage;
        }
    }
}
