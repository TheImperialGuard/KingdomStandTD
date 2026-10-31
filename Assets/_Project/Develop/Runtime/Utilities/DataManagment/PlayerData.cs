using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.Meta.Features.Settings;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment
{
    public class PlayerData : ISaveData
    {
        public Dictionary<int, LevelResults> CompletedLevels;
        public AudioSettingsData AudioSettings;
        public int EndlessBestWave;
        //Upgrades info
    }
}
