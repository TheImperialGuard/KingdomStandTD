using System;
using Assets._Project.Develop.Runtime.Gameplay.Features.Player;
using Assets._Project.Develop.Runtime.Utilities.Audio;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
    // Когда с поля ушёл последний босс, снова играет боевая музыка. Его включают регистрации бесконечного режима: в
    // кампании после босса музыка не меняется. Бесконечный кончается только поражением — если босс, дойдя до замка,
    // закончил игру, музыку уже поставил итог боя, и её здесь не трогают.
    public class ReturnBattleMusicAfterBossService : IDisposable
    {
        private readonly MusicSwitcherService _musicSwitcherService;
        private readonly PlayerHealth _playerHealth;

        private IDisposable _bossesCountDisposable;

        public ReturnBattleMusicAfterBossService(
            MusicSwitcherService musicSwitcherService,
            EnemiesOnFieldCounter enemiesOnFieldCounter,
            PlayerHealth playerHealth)
        {
            _musicSwitcherService = musicSwitcherService;
            _playerHealth = playerHealth;

            _bossesCountDisposable = enemiesOnFieldCounter.BossesCount.Subscribe(OnBossesCountChanged);
        }

        public void Dispose()
        {
            _bossesCountDisposable.Dispose();
        }

        private void OnBossesCountChanged(int oldCount, int newCount)
        {
            if (newCount > 0 || IsCastleFallen())
                return;

            _musicSwitcherService.SwitchFor(MusicContexts.GameplayBattle);
        }

        private bool IsCastleFallen() => _playerHealth.Current.Value <= 0;
    }
}
