using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Сколько врагов и боссов сейчас на поле. Враг — сущность, которую создал EnemiesFactory, призванные тоже; снаряды
    // команды врагов (снежки) в счёт не идут. Враг на поле от добавления сущности до её освобождения, то есть и во время
    // анимации смерти. Счёт ведётся по Added и Released, без обхода всех сущностей.
    public class EnemiesOnFieldCounter : IDisposable
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private ReactiveVariable<int> _enemiesCount = new();
        private ReactiveVariable<int> _bossesCount = new();

        public EnemiesOnFieldCounter(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;

            _entitiesLifeContext.Added += OnEntityAdded;
            _entitiesLifeContext.Released += OnEntityReleased;

            foreach (Entity entity in _entitiesLifeContext.Entities)
                OnEntityAdded(entity);
        }

        public IReadOnlyVariable<int> EnemiesCount => _enemiesCount;
        public IReadOnlyVariable<int> BossesCount => _bossesCount;

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
            _entitiesLifeContext.Released -= OnEntityReleased;
        }

        private void OnEntityAdded(Entity entity)
        {
            if (IsEnemy(entity) == false)
                return;

            _enemiesCount.Value++;

            if (IsBoss(entity))
                _bossesCount.Value++;
        }

        // Компоненты освобождённой сущности остаются на месте, и её узнают по тем же признакам, что при добавлении
        private void OnEntityReleased(Entity entity)
        {
            if (IsEnemy(entity) == false)
                return;

            _enemiesCount.Value--;

            if (IsBoss(entity))
                _bossesCount.Value--;
        }

        private static bool IsEnemy(Entity entity) => entity.HasComponent<EnemyKind>();

        private static bool IsBoss(Entity entity) => entity.HasComponent<IsBoss>();
    }
}
