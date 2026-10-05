using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Newtonsoft.Json;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Выход одного врага волны: время от начала стейджа и конфиг врага.
    public readonly struct WaveSpawn
    {
        public WaveSpawn(float time, CharacterConfig enemy)
        {
            Time = time;
            Enemy = enemy;
        }

        public float Time { get; }
        public CharacterConfig Enemy { get; }
    }

    // Статистика волны. Времена — в секундах от начала стейджа.
    // Приход на базу — оценка без убийств, null — время в пути неизвестно.
    public class WaveStats
    {
        public int EnemiesCount { get; set; }
        public float Health { get; set; }
        public int KillGold { get; set; }
        public int BreakthroughDamage { get; set; }
        public float SpawnEnd { get; set; }
        public float? FirstArrival { get; set; }
        public float? LastArrival { get; set; }
    }

    // Статистика стейджа. Золото к началу стейджа считается так, будто враги прошлых стейджей убиты.
    // Состав — число врагов по имени ассета с учётом повторов шаблона.
    public class StageStats
    {
        public int WavesCount { get; set; }
        public int EnemiesCount { get; set; }
        public SortedDictionary<string, int> EnemiesByType { get; set; } = new(StringComparer.Ordinal);
        public float Health { get; set; }
        public float EffectiveHealthPhysic { get; set; }
        public float EffectiveHealthMagic { get; set; }
        public float HealthPerSecond { get; set; }
        public int KillGold { get; set; }
        public int MaxSkipGold { get; set; }
        public int GoldAtStartWithoutSkips { get; set; }
        public int GoldAtStartWithMaxSkips { get; set; }
        public int BreakthroughDamage { get; set; }
        public float BreakthroughThreat { get; set; }
        public float SpawnEnd { get; set; }
        public float SpawnOverflow { get; set; }
        public float? LastArrival { get; set; }
        public int? PeakEnemies { get; set; }
        public List<RoadTrafficStats> Traffic { get; set; } = new();

        [JsonIgnore]
        public List<WaveStats> Waves { get; set; } = new();
    }

    public class LevelStats
    {
        public int StagesCount { get; set; }
        public int WavesCount { get; set; }
        public int EnemiesCount { get; set; }
        public SortedDictionary<string, int> EnemiesByType { get; set; } = new(StringComparer.Ordinal);
        public float Health { get; set; }
        public float EffectiveHealthPhysic { get; set; }
        public float EffectiveHealthMagic { get; set; }
        public int KillGold { get; set; }
        public int MaxSkipGold { get; set; }
        public int TotalGoldWithoutSkips { get; set; }
        public int TotalGoldWithMaxSkips { get; set; }
        public int BreakthroughDamage { get; set; }
        public float BreakthroughThreat { get; set; }
        public float DurationWithoutSkips { get; set; }
        public float DurationWithMaxSkips { get; set; }
        public int? PeakEnemies { get; set; }
        public List<RoadTrafficStats> Traffic { get; set; } = new();

        [JsonIgnore]
        public List<StageStats> Stages { get; set; } = new();

        [JsonIgnore]
        public List<RoadStats> Roads { get; set; } = new();
    }

    // Дорога префаба: длины по полосам и время до базы по центру дороги для врагов, которые по ней идут.
    public class RoadStats
    {
        public PathNumber PathNumber { get; set; }
        public string Name { get; set; }
        public int WaypointsCount { get; set; }
        public float? LengthCenter { get; set; }
        public float? LengthLeft { get; set; }
        public float? LengthRight { get; set; }
        public SortedDictionary<string, float> TimeToBase { get; set; } = new(StringComparer.Ordinal);
    }

    // Сколько волн, врагов и здоровья идёт по одной полосе одной дороги.
    public class RoadTrafficStats
    {
        public PathNumber PathNumber { get; set; }
        public PathLane Lane { get; set; }
        public int WavesCount { get; set; }
        public int EnemiesCount { get; set; }
        public float Health { get; set; }
    }
}
