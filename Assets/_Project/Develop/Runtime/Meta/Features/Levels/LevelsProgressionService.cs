using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using System.Collections.Generic;
using System;

namespace Assets._Project.Develop.Runtime.Meta.Features.Levels
{
    public class LevelsProgressionService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private const int FirstLevel = 1;

        private readonly Dictionary<int, LevelResults> _completedLevels = new();

        public LevelsProgressionService(PlayerDataProvider playerDataProvider)
        {
            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public IReadOnlyDictionary<int, LevelResults> CompletedLevels => _completedLevels;

        public bool IsLevelCompleted(int levelNumber) => _completedLevels.ContainsKey(levelNumber);

        public void AddLevelToCompleted(int levelNumber, LevelResults result)
        {
            if (result == LevelResults.Defeat)
                throw new InvalidOperationException($"Trying to add level {levelNumber} to completed, that marked as defeated");

            if (IsLevelCompleted(levelNumber))
                _completedLevels[levelNumber] = result;
            else
                _completedLevels.Add(levelNumber, result);
        }

        public bool CanPlay(int levelNumber)
        {
            return levelNumber == FirstLevel || PreviousLevelCompleted(levelNumber);
        }

        private bool PreviousLevelCompleted(int levelNumber) => IsLevelCompleted(levelNumber - 1);

        public void ReadFrom(PlayerData data)
        {
            _completedLevels.Clear();

            foreach (KeyValuePair<int, LevelResults> levelWithResults in data.CompletedLevels)
                _completedLevels.Add(levelWithResults.Key, levelWithResults.Value);
        }

        public void WriteTo(PlayerData data)
        {
            data.CompletedLevels.Clear();

            foreach (KeyValuePair<int, LevelResults> levelWithResults in _completedLevels)
                data.CompletedLevels.Add(levelWithResults.Key, levelWithResults.Value);
        }
    }
}
