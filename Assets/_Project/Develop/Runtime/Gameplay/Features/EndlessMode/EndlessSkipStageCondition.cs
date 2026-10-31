using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Utilities.Conditions;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Можно ли сейчас вызвать волну бесконечного режима досрочно: нельзя, пока на поле босс или врагов больше порога из
    // конфига. Тогда следующая волна придёт сама, по таймеру текущей.
    public class EndlessSkipStageCondition : ICondition
    {
        private readonly EnemiesOnFieldCounter _enemiesOnFieldCounter;
        private readonly int _maxEnemiesToSkipWave;

        public EndlessSkipStageCondition(EnemiesOnFieldCounter enemiesOnFieldCounter, EndlessModeConfig config)
        {
            _enemiesOnFieldCounter = enemiesOnFieldCounter;
            _maxEnemiesToSkipWave = config.MaxEnemiesToSkipWave;
        }

        public bool Evaluate()
        {
            return _enemiesOnFieldCounter.BossesCount.Value == 0
                && _enemiesOnFieldCounter.EnemiesCount.Value <= _maxEnemiesToSkipWave;
        }
    }
}
