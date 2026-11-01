using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Weather;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Newtonsoft.Json;

namespace Assets._Project.Develop.Editor.EndlessMode
{
    // Блок endless среза бесконечного уровня — настройки EndlessModeConfig, по которым генератор собирает волны.
    // Импортируется целиком. Враги — имена ассетов MeleeConfig; null — пустой элемент списка.
    public class EndlessSettingsSlice
    {
        [JsonProperty(Required = Required.Always)]
        public int Seed { get; set; }

        [JsonProperty(Required = Required.Always)]
        public WaveValueTableSlice EnemiesCount { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<EndlessTierSlice> Tiers { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public WaveValueTableSlice DelayBetweenSpawns { get; set; }

        [JsonProperty(Required = Required.Always)]
        public WaveValueTableSlice RestTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int BossWavesStep { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<string> Bosses { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public float BossWaveEnemiesShare { get; set; }

        [JsonProperty(Required = Required.Always)]
        public WaveValueTableSlice HealthMultiplier { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int MaxEnemiesToSkipWave { get; set; }
    }

    public class WaveValueTableSlice
    {
        [JsonProperty(Required = Required.Always)]
        public List<WaveValueKeySlice> Keys { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public WaveValueTailModes TailMode { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float TailStep { get; set; }
    }

    public class WaveValueKeySlice
    {
        [JsonProperty(Required = Required.Always)]
        public int Wave { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float Value { get; set; }
    }

    public class EndlessTierSlice
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int KindsPerWave { get; set; }

        [JsonProperty(Required = Required.Always)]
        public WaveValueTableSlice Share { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<EndlessTierEnemySlice> Enemies { get; set; } = new();
    }

    public class EndlessTierEnemySlice
    {
        [JsonProperty(Required = Required.AllowNull)]
        public string Enemy { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int FirstWave { get; set; }
    }

    // Блок weather — справочно: правила погоды из WeatherConfig. Погода не зависит от зерна, поэтому в волны среза не
    // входит; правится она в инспекторе конфига погоды. transitions — какие погоды могут прийти на смену каждой.
    public class EndlessWeatherSlice
    {
        public WeatherKinds StartWeather { get; set; }
        public List<WeatherKindSlice> Weathers { get; set; } = new();
        public float ChangeChance { get; set; }
        public float ChangeChanceGrowth { get; set; }
        public SortedDictionary<WeatherKinds, List<WeatherKinds>> Transitions { get; set; } = new();
        public float TransitionDuration { get; set; }
        public LightningSlice Lightning { get; set; }
    }

    public class WeatherKindSlice
    {
        public WeatherKinds Kind { get; set; }
        public int WavesCount { get; set; }
        public bool HasRain { get; set; }
        public bool HasLightning { get; set; }
    }

    // Урон удара растёт по волнам по таблице множителя здоровья врагов.
    public class LightningSlice
    {
        public float MinInterval { get; set; }
        public float MaxInterval { get; set; }
        public float RoadStrikesShare { get; set; }
        public float Radius { get; set; }
        public float BaseDamage { get; set; }
        public DamageTypes DamageType { get; set; }
    }

    // Добавки бесконечного режима к analysis волны (стейджа среза): здоровье — с множителем её волны, босс — отдельно
    // от счёта тиров.
    public class EndlessStageAnalysis
    {
        public float HealthMultiplier { get; set; }
        public float HealthWithMultiplier { get; set; }
        public float EffectiveHealthPhysicWithMultiplier { get; set; }
        public float EffectiveHealthMagicWithMultiplier { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Boss { get; set; }

        // Ключ — имя тира, порядок — порядок тиров в конфиге
        public Dictionary<string, int> EnemiesByTier { get; set; } = new();
    }

    // Добавка к analysis потока (волны среза).
    public class EndlessWaveAnalysis
    {
        public float HealthWithMultiplier { get; set; }
    }

    // Добавки к analysis всего среза — суммы по выгруженным волнам.
    public class EndlessLevelAnalysis
    {
        public float HealthWithMultiplier { get; set; }
        public float EffectiveHealthPhysicWithMultiplier { get; set; }
        public float EffectiveHealthMagicWithMultiplier { get; set; }
    }
}
