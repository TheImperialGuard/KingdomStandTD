using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Правила бесконечного режима: уровень, на котором он идёт, и всё, из чего генератор собирает волны. Числа по волнам —
    // таблицы ключей, состав — тиры врагов в порядке выхода на дорогу. Ещё — боссы, рост здоровья врагов и порог толпы,
    // выше которого нельзя вызвать волну досрочно.
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/NewEndlessModeConfig", fileName = "EndlessModeConfig")]
    public class EndlessModeConfig : ScriptableObject
    {
        [Tooltip("Уровень режима: карта, стартовое золото, здоровье замка, лимиты улучшений. Его стейджи не используются")]
        [SerializeField] private LevelConfig _level;

        [Tooltip("Описание режима для окна в главном меню")]
        [SerializeField, TextArea(3, 12)] private string _description;

        [Tooltip("Зерно случайности: вместе с номером волны задаёт её состав, так что волна N всегда одна и та же")]
        [SerializeField] private int _seed;

        [Tooltip("Число врагов волны, округляется до целого")]
        [SerializeField] private WaveValueTable _enemiesCount = new();

        [Tooltip("Тиры врагов. Порядок тиров — порядок выхода на дорогу внутри волны")]
        [SerializeField] private List<EndlessTier> _tiers = new();

        [Tooltip("Интервал между врагами одной дороги, с")]
        [SerializeField] private WaveValueTable _delayBetweenSpawns = new();

        [Tooltip("Передышка после выхода последнего врага волны до начала следующей, с")]
        [SerializeField] private WaveValueTable _restTime = new();

        [Tooltip("Боссовая — каждая волна с номером, кратным шагу")]
        [SerializeField, Min(1)] private int _bossWavesStep = 10;

        [Tooltip("Боссы по кругу: на первой боссовой волне — первый, на второй — второй, после последнего — снова первый")]
        [SerializeField] private List<MeleeConfig> _bosses = new();

        [Tooltip("Сколько обычных врагов остаётся в боссовой волне — доля от волны того же номера")]
        [SerializeField, Range(0f, 1f)] private float _bossWaveEnemiesShare = 0.5f;

        [Tooltip("Множитель здоровья врагов из волн, в том числе боссов. Призванных врагов не касается")]
        [SerializeField] private WaveValueTable _healthMultiplier = new();

        [Tooltip("Досрочный вызов волны закрыт, пока врагов на поле больше этого числа (призванные — в счёт)")]
        [SerializeField, Min(0)] private int _maxEnemiesToSkipWave = 40;

        public LevelConfig Level => _level;
        public string Description => _description;
        public int Seed => _seed;
        public WaveValueTable EnemiesCount => _enemiesCount;
        public IReadOnlyList<EndlessTier> Tiers => _tiers;
        public WaveValueTable DelayBetweenSpawns => _delayBetweenSpawns;
        public WaveValueTable RestTime => _restTime;
        public int BossWavesStep => _bossWavesStep;
        public IReadOnlyList<MeleeConfig> Bosses => _bosses;
        public float BossWaveEnemiesShare => _bossWaveEnemiesShare;
        public WaveValueTable HealthMultiplier => _healthMultiplier;
        public int MaxEnemiesToSkipWave => _maxEnemiesToSkipWave;
    }
}
