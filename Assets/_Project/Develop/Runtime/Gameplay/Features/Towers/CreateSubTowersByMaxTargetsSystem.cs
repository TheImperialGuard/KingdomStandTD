using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Towers
{
    public class CreateSubTowersByMaxTargetsSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly TowersFactory _towersFactory;

        private Entity _parent;

        private ReactiveVariable<int> _maxTargets;

        private List<Entity> _createdSubs = new();
        private IDisposable _onMaxTargetsChangedDisposable;

        public CreateSubTowersByMaxTargetsSystem(TowersFactory towersFactory)
        {
            _towersFactory = towersFactory;
        }

        public void OnInit(Entity entity)
        {
            _parent = entity;

            _maxTargets = _parent.MaxTargets;

            _onMaxTargetsChangedDisposable = _maxTargets.Subscribe(OnMaxTargetsChanged);
        }

        public void OnDispose()
        {
            _onMaxTargetsChangedDisposable.Dispose();
        }

        private void OnMaxTargetsChanged(int oldValue, int newMaxTargets)
        {
            if (newMaxTargets <= 0)
                throw new ArgumentOutOfRangeException($"Max targets for attack can not be < 1 (current: {newMaxTargets})");

            int additionalTargetsDiff = newMaxTargets - oldValue;

            if (additionalTargetsDiff > 0)
                CreateSubTowers(additionalTargetsDiff);
            else
                DeleteSubTowers(additionalTargetsDiff * -1);
        }

        private void CreateSubTowers(int count)
        {
            List<ReactiveVariable<Entity>> targetsForExclude = new(GetTargetsForExclude());

            for (int i = 0; i < count; i++)
            {
                Entity subTower = _towersFactory.CreateSubTowerFor(_parent, targetsForExclude);

                _createdSubs.Add(subTower);
                targetsForExclude.Add(subTower.CurrentTarget);
            }
        }

        private void DeleteSubTowers(int count)
        {
            if (_createdSubs.Count == 0 || _createdSubs.Count - count < 0)
                throw new InvalidOperationException();

            if (_createdSubs.Count - count == 0)
            {
                foreach (Entity subTower in _createdSubs)
                    subTower.SelfReleaseRequested.Value = true;

                _createdSubs.Clear();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Entity forDelete = _createdSubs[_createdSubs.Count - 1];

                for (int j = _createdSubs.Count - 2; j >= 0 ; j--)
                {
                    if (_createdSubs[j].TargetsForExclude.Contains(forDelete.CurrentTarget))
                        _createdSubs[j].TargetsForExclude.Remove(forDelete.CurrentTarget);
                }

                forDelete.SelfReleaseRequested.Value = true;
                _createdSubs.Remove(forDelete);
            }
        }

        private List<ReactiveVariable<Entity>> GetTargetsForExclude()
        {
            List<ReactiveVariable<Entity>> targetsForExclude = new();

            targetsForExclude.Add(_parent.CurrentTarget);

            for (int i = 0; i < _createdSubs.Count; i++)
            {
                targetsForExclude.Add(_createdSubs[i].CurrentTarget);
            }

            return targetsForExclude;
        }
    }
}
