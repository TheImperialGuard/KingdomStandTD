using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    // Стейджи, заданные списком в конфиге уровня, — уровни кампании.
    public class FixedStagesSource : IStagesSource
    {
        private const int FirstStageNumber = 1;

        private readonly List<StageConfig> _stagesConfigs;

        public FixedStagesSource(IReadOnlyList<StageConfig> stagesConfigs)
        {
            _stagesConfigs = new(stagesConfigs);
        }

        public bool IsFinite => true;

        public int FirstNumber => FirstStageNumber;

        public int Count => _stagesConfigs.Count;

        public bool Has(int stageNumber) => stageNumber >= FirstStageNumber && stageNumber <= Count;

        public StageConfig Get(int stageNumber) => _stagesConfigs[stageNumber - FirstStageNumber];
    }
}
