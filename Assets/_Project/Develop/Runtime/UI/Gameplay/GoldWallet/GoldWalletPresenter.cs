using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core.Presenters;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.GoldWallet
{
    public class GoldWalletPresenter : IPresenter
    {
        private readonly IconTextView _view;
        private readonly WalletService _wallet;

        private IDisposable _currentGoldChangedDisposable;

        public GoldWalletPresenter(IconTextView view, WalletService wallet)
        {
            _view = view;
            _wallet = wallet;
        }

        public void Initialize()
        {
            _currentGoldChangedDisposable = _wallet.GetCurrency(CurrencyTypes.Gold).Subscribe(OnCurrentGoldChanged);

            UpdateView();
        }

        public void Dispose()
        {
            _currentGoldChangedDisposable.Dispose();
        }

        private void OnCurrentGoldChanged(int arg1, int arg2) => UpdateView();

        private void UpdateView()
        {
            _view.SetText(_wallet.GetCurrency(CurrencyTypes.Gold).Value.ToString());
        }
    }
}
