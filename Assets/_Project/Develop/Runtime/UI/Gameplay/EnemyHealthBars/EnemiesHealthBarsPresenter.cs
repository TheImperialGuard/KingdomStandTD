using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.EnemyHealthBars
{
    // Полоски здоровья всех врагов в одном экранном слое интерфейса, вместо своего Canvas у каждого врага: части полосок
    // на одной текстуре атласа идут общим вызовом отрисовки на весь экран. Элементы берутся из своего пула, а их место
    // пересчитывается каждый кадр — из GameplayBootstrap.Update, после камеры и движения
    public class EnemiesHealthBarsPresenter : IPresenter
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly ViewsFactory _viewsFactory;
        private readonly GameplayPresentersFactory _presentersFactory;
        private readonly Transform _layer;

        private readonly Dictionary<Entity, EnemyHealthBar> _bars = new();
        private readonly Stack<EnemyHealthBarView> _freeViews = new();

        public EnemiesHealthBarsPresenter(
            EntitiesLifeContext entitiesLifeContext,
            ViewsFactory viewsFactory,
            GameplayPresentersFactory presentersFactory,
            Transform layer)
        {
            _entitiesLifeContext = entitiesLifeContext;
            _viewsFactory = viewsFactory;
            _presentersFactory = presentersFactory;
            _layer = layer;
        }

        public void Initialize()
        {
            _entitiesLifeContext.Added += OnEntityAdded;
            _entitiesLifeContext.Released += OnEntityReleased;

            foreach (Entity entity in _entitiesLifeContext.Entities)
                OnEntityAdded(entity);
        }

        // Сами элементы — дети слоя в GameplayUIRoot и уничтожаются вместе со сценой
        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
            _entitiesLifeContext.Released -= OnEntityReleased;

            foreach (EnemyHealthBar bar in _bars.Values)
                bar.Presenter.Dispose();

            _bars.Clear();
            _freeViews.Clear();
        }

        public void Update()
        {
            foreach (EnemyHealthBar bar in _bars.Values)
                bar.UpdatePosition();
        }

        private void OnEntityAdded(Entity entity)
        {
            if (entity.TryGetHealthBarPoint(out Transform point) == false)
                return;

            EnemyHealthBarView view = TakeView();

            // Позже появившийся враг рисуется поверх: порядок в слое — порядок появления
            view.transform.SetAsLastSibling();

            EnemyHealthBarPresenter presenter = _presentersFactory.CreateEnemyHealthBarPresenter(view, entity);
            presenter.Initialize();

            EnemyHealthBar bar = new(view, presenter, point);

            // Сразу на место: призванный посреди кадра враг иначе показал бы полоску там, где её оставил прошлый
            bar.UpdatePosition();

            _bars.Add(entity, bar);
        }

        private void OnEntityReleased(Entity entity)
        {
            if (_bars.Remove(entity, out EnemyHealthBar bar) == false)
                return;

            bar.Presenter.Dispose();
            bar.View.Hide();

            _freeViews.Push(bar.View);
        }

        private EnemyHealthBarView TakeView()
        {
            if (_freeViews.TryPop(out EnemyHealthBarView view))
                return view;

            return _viewsFactory.Create<EnemyHealthBarView>(ViewIDs.EnemyHealthBar, _layer);
        }

        private class EnemyHealthBar
        {
            private readonly Transform _point;

            public EnemyHealthBar(EnemyHealthBarView view, EnemyHealthBarPresenter presenter, Transform point)
            {
                View = view;
                Presenter = presenter;
                _point = point;
            }

            public EnemyHealthBarView View { get; }

            public EnemyHealthBarPresenter Presenter { get; }

            public void UpdatePosition() => View.UpdateWorldPosition(_point.position);
        }
    }
}
