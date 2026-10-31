using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AOE
{
    // Удар по площади в точке без сущности-источника, например молния: урон наземным врагам в радиусе. Летающих не
    // задевает, башни и всё, что не враг, — тоже. Боссы и призванные — враги, их задевает.
    public class AreaDamageService
    {
        private readonly AreaEntitiesDetectorService _areaEntitiesDetectorService;

        public AreaDamageService(AreaEntitiesDetectorService areaEntitiesDetectorService)
        {
            _areaEntitiesDetectorService = areaEntitiesDetectorService;
        }

        // Возвращает, скольких врагов задел удар
        public int DealToGroundEnemies(Vector3 center, float radius, float damage, DamageTypes damageType)
        {
            int hitEnemiesCount = 0;

            foreach (Entity entity in _areaEntitiesDetectorService.GetEntitiesInArea(center, radius))
            {
                if (entity.HasComponent<EnemyKind>() == false || entity.HasComponent<IsFlying>())
                    continue;

                if (EntitiesHelper.TryTakeDamage(entity, damage, damageType))
                    hitEnemiesCount++;
            }

            return hitEnemiesCount;
        }
    }
}
