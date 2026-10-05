using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Справочник для балансировки — Docs/Balance/Reference.json: параметры GameConfig, враги и башни.
    public class BalanceReference
    {
        public GameReference Game { get; set; }
        public List<EnemyReference> Enemies { get; set; } = new();
        public List<TowerReference> Towers { get; set; } = new();
    }

    public class GameReference
    {
        public float GoldIndexForRemainingStageSeconds { get; set; }
        public int MaxGoldForSkipStage { get; set; }
        public float SellTowerGoldReturnIndex { get; set; }
        public int GoldRewardByAdvert { get; set; }
    }

    // Враг — MeleeConfig: другие конфиги персонажей EnemiesFactory не создаёт.
    public class EnemyReference
    {
        public string Asset { get; set; }
        public string Name { get; set; }
        public float MaxHealth { get; set; }
        public float MoveSpeed { get; set; }
        public DamageTypes DamageResistanceType { get; set; }
        public float DamageResistanceIndex { get; set; }
        public float EffectiveHealthPhysic { get; set; }
        public float EffectiveHealthMagic { get; set; }
        public int GoldOnDeath { get; set; }
        public int DamageOnFinishPath { get; set; }
        public bool IsBoss { get; set; }
    }

    // Уровень башни — как его видит TowersListConfig; null, если до конфига нельзя дойти улучшениями.
    // Cost — цена покупки или улучшения до этого уровня, TotalCost — сумма цен всех уровней до него.
    // Урон в секунду — без способностей и урона по площади.
    public class TowerReference
    {
        public string Asset { get; set; }
        public TowerTypes TowerType { get; set; }
        public int? Level { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int? TotalCost { get; set; }
        public float AttackDamage { get; set; }
        public DamageTypes DamageType { get; set; }
        public float AttackPerSecond { get; set; }
        public float DamagePerSecond { get; set; }
        public float? DamagePerSecondPer100Gold { get; set; }
        public float AttackRange { get; set; }
    }
}
