using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Срез уровня — JSON в Docs/Balance/Levels. Импортируются только startGold, startPlayerHealth и stages
    // (данные стейджей и волн); campaignNumber, prefabPath, gameMode, towerSlotsCount, roads, analysis, errors
    // и warnings — справочные блоки, при импорте их содержимое пропускается.
    public class LevelSlice
    {
        [JsonProperty(Required = Required.Always)]
        public string Level { get; set; }

        public int? CampaignNumber { get; set; }
        public string PrefabPath { get; set; }
        public string GameMode { get; set; }
        public int? TowerSlotsCount { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int StartGold { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int StartPlayerHealth { get; set; }

        public JToken Roads { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<StageSlice> Stages { get; set; } = new();

        public JToken Analysis { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    public class StageSlice
    {
        [JsonProperty(Required = Required.Always)]
        public float StageTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float TimeToSkipStage { get; set; }

        public string Note { get; set; } = string.Empty;

        [JsonProperty(Required = Required.Always)]
        public List<WaveSlice> Waves { get; set; } = new();

        public JToken Analysis { get; set; }
    }

    public class WaveSlice
    {
        [JsonProperty(Required = Required.Always)]
        public List<EnemyGroupSlice> Groups { get; set; } = new();

        [JsonProperty(Required = Required.Always)]
        public int PatternRepeats { get; set; }

        [JsonProperty(Required = Required.Always)]
        public PathNumber PathNumber { get; set; }

        [JsonProperty(Required = Required.Always)]
        public PathLane Lane { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float StartDelay { get; set; }

        [JsonProperty(Required = Required.Always)]
        public float DelayBetweenSpawns { get; set; }

        public string Note { get; set; } = string.Empty;
        public JToken Analysis { get; set; }
    }

    // Враг — имя ассета CharacterConfig; null, если в конфиге враг не выбран.
    public class EnemyGroupSlice
    {
        [JsonProperty(Required = Required.AllowNull)]
        public string Enemy { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Count { get; set; }
    }
}
