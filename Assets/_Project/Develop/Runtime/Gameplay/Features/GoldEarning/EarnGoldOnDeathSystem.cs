using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Wallet;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GoldEarning
{
    public class EarnGoldOnDeathSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly WalletService _wallet;

        public EarnGoldOnDeathSystem(WalletService walletService)
        {
            _wallet = walletService;
        }

        private Entity _entity;

        private ReactiveVariable<int> _goldOnDeath;

        private IDisposable _isDeadDisposable;

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _goldOnDeath = entity.GoldOnDeath;

            _isDeadDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        public void OnDispose()
        {
            _isDeadDisposable.Dispose();
        }

        private void OnIsDeadChanged(bool oldIsDead, bool newIsDead)
        {
            if (newIsDead == false)
                return;

            _wallet.Add(CurrencyTypes.Gold, _goldOnDeath.Value);
        }
    }
}
