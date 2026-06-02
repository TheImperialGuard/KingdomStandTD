using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment
{
    public class PlayerData : ISaveData
    {
        public Dictionary<int, LevelResults> CompletedLevels;
        //Upgrades info
    }
}
