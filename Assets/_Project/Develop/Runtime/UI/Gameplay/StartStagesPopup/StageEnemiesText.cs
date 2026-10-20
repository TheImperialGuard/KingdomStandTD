using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup
{
    // Враги стейджа для подсказки волны: «Имя xN, …», сумма по волнам × PatternRepeats. Каждая запись неразрывна вместе
    // со своей запятой — длинный список переносится только между записями.
    public static class StageEnemiesText
    {
        private const string EntryStart = "<nobr>";
        private const string EntryEnd = "</nobr>";
        private const string EntrySeparator = ",</nobr> ";

        public static string Build(StageConfig config)
        {
            if (config is EnemiesWavesStageConfig enemiesWavesStageConfig == false)
                throw new ArgumentException($"{typeof(StageConfig)}");

            Dictionary<string, int> allEnemiesInWaves = new();

            foreach (EnemiesWaveConfig wave in enemiesWavesStageConfig.EnemiesWaveConfigs)
            {
                foreach (WaveEnemyGroup enemyGroup in wave.EnemyGroups)
                {
                    string characterName = enemyGroup.EnemyConfig.CharacterName;
                    int count = enemyGroup.Count * wave.PatternRepeats;

                    if (allEnemiesInWaves.ContainsKey(characterName))
                    {
                        allEnemiesInWaves[characterName] += count;
                    }
                    else
                    {
                        allEnemiesInWaves.Add(characterName, count);
                    }
                }
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, int> kvp in allEnemiesInWaves)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(EntrySeparator);

                stringBuilder
                    .Append(EntryStart)
                    .Append(kvp.Key)
                    .Append(" x")
                    .Append(kvp.Value);
            }

            return stringBuilder.Append(EntryEnd).ToString();
        }
    }
}
