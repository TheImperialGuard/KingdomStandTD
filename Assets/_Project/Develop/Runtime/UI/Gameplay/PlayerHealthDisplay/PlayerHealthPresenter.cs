using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.PlayerHealthDisplay
{
    public class PlayerHealthPresenter : IPresenter
    {
        private readonly IconTextView _view;
        private readonly PlayerHealth _health;

        private IDisposable _currentHealthChangedDisposable;

        public PlayerHealthPresenter(IconTextView view, PlayerHealth health)
        {
            _view = view;
            _health = health;
        }

        public void Initialize()
        {
            _currentHealthChangedDisposable = _health.Current.Subscribe(OnCurrentHealthChanged);

            UpdateView();
        }

        public void Dispose()
        {
            _currentHealthChangedDisposable.Dispose();
        }

        private void OnCurrentHealthChanged(int arg1, int arg2) => UpdateView();

        private void UpdateView()
        {
            _view.SetText(_health.Current.Value.ToString());
        }
    }
}
