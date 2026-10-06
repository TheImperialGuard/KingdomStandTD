using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Newtonsoft.Json.Linq;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Лог прогона плейтеста — JSON в Logs/Playtests. Стейджи — с настоящими номерами уровня, время — игровые секунды
    // от запуска прогона (пауза их не считает). Враги (вышло, убито, прорвалось, урон, золото за убийства) числятся
    // за стейджем своей волны; золото, здоровье и башни — за временем стейджа: от его начала до начала следующего,
    // у первого стейджа прогона — вместе с подготовкой. levelSnapshot — срез уровня на момент запуска, как в
    // Docs/Balance, с несохранёнными правками.
    public class PlaytestLog
    {
        public string Level { get; set; }
        public int? CampaignNumber { get; set; }
        public string StartedAt { get; set; }
        public int StartStage { get; set; }
        public int LevelStagesCount { get; set; }
        public int StartGold { get; set; }
        public int StartPlayerHealth { get; set; }
        public bool HasUnsavedChanges { get; set; }
        public PlaytestResult Result { get; set; }
        public float PreparationTime { get; set; }
        public float GameTime { get; set; }
        public float RealTime { get; set; }
        public int FinalGold { get; set; }
        public int FinalPlayerHealth { get; set; }
        public List<PlaytestStageLog> Stages { get; set; } = new();
        public List<PlaytestTowerCount> TowersAtEnd { get; set; } = new();
        public JToken LevelSnapshot { get; set; }
    }

    // InProgress — прогон ещё идёт; Interrupted — оборван: «Заново», выход в меню или Stop.
    public enum PlaytestResult
    {
        InProgress,
        Perfect,
        Average,
        Bad,
        Defeat,
        Interrupted,
    }

    public class PlaytestStageLog
    {
        public int Stage { get; set; }
        public float StageTime { get; set; }
        public float StartTime { get; set; }
        public float Duration { get; set; }
        public bool IsSkipped { get; set; }
        public PlaytestGoldLog Gold { get; set; } = new();
        public PlaytestHealthLog Health { get; set; } = new();
        public PlaytestTowersLog Towers { get; set; } = new();
        public List<PlaytestEnemyLog> Enemies { get; set; } = new();
        public List<PlaytestWaveLog> Waves { get; set; } = new();
    }

    // atEnd = atStart + kills + skip + sales − towerPurchases − abilityPurchases + other; other — то, что прошло мимо
    // этих статей (награда за рекламу).
    public class PlaytestGoldLog
    {
        public int AtStart { get; set; }
        public int Kills { get; set; }
        public int Skip { get; set; }
        public int Sales { get; set; }
        public int TowerPurchases { get; set; }
        public int AbilityPurchases { get; set; }
        public int Other { get; set; }
        public int AtEnd { get; set; }
    }

    public class PlaytestHealthLog
    {
        public int AtStart { get; set; }
        public int Lost { get; set; }
    }

    // Покупка второго и следующих уровней башни — это её улучшение.
    public class PlaytestTowersLog
    {
        public List<PlaytestTowerCount> AtStart { get; set; } = new();
        public List<PlaytestTowerDeal> Bought { get; set; } = new();
        public List<PlaytestTowerDeal> Sold { get; set; } = new();
        public List<PlaytestAbilityPurchase> Abilities { get; set; } = new();
    }

    public class PlaytestTowerCount
    {
        public TowerTypes Tower { get; set; }
        public int Level { get; set; }
        public int Count { get; set; }
    }

    public class PlaytestTowerDeal
    {
        public TowerTypes Tower { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
    }

    public class PlaytestAbilityPurchase
    {
        public TowerTypes Tower { get; set; }
        public string Ability { get; set; }
        public int Gold { get; set; }
    }

    // Враг — имя ассета CharacterConfig, как в срезах.
    public class PlaytestEnemyLog
    {
        public string Enemy { get; set; }
        public int Spawned { get; set; }
        public int Killed { get; set; }
        public int Leaked { get; set; }
        public int Damage { get; set; }
        public int KillGold { get; set; }
    }

    public class PlaytestWaveLog
    {
        public int Wave { get; set; }
        public int Spawned { get; set; }
        public int Killed { get; set; }
        public int Leaked { get; set; }
        public int Damage { get; set; }
    }
}
