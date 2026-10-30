using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Правила бесконечного режима: уровень, на котором он идёт, и всё, из чего генератор собирает волны. Числа по волнам —
    // таблицы ключей, состав — тиры врагов в порядке выхода на дорогу.
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

        public LevelConfig Level => _level;
        public string Description => _description;
        public int Seed => _seed;
        public WaveValueTable EnemiesCount => _enemiesCount;
        public IReadOnlyList<EndlessTier> Tiers => _tiers;
        public WaveValueTable DelayBetweenSpawns => _delayBetweenSpawns;
        public WaveValueTable RestTime => _restTime;
    }
}
