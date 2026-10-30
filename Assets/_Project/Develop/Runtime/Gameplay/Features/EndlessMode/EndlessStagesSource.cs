using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Волны бесконечного режима без конца. Генерирует волну по первому запросу и кеширует: окна старта и вызова читают
    // следующую волну заранее, и спавн должен получить тот же самый конфиг. Помнит, к какой волне относится каждый поток,
    // — по потоку заспавненного врага режим узнаёт номер его волны.
    public class EndlessStagesSource : IStagesSource
    {
        private const int FirstWaveNumber = 1;

        private readonly EndlessWavesGenerator _generator;

        private readonly Dictionary<int, EnemiesWavesStageConfig> _wavesConfigs = new();
        private readonly Dictionary<EnemiesWaveConfig, int> _streamsWavesNumbers = new();

        public EndlessStagesSource(EndlessWavesGenerator generator)
        {
            _generator = generator;
        }

        public bool IsFinite => false;

        public int Count => throw new NotSupportedException("У бесконечного режима нет числа волн");

        public bool Has(int stageNumber) => stageNumber >= FirstWaveNumber;

        public StageConfig Get(int stageNumber)
        {
            if (Has(stageNumber) == false)
                throw new ArgumentOutOfRangeException(nameof(stageNumber), stageNumber, "Волны нумеруются с единицы");

            if (_wavesConfigs.TryGetValue(stageNumber, out EnemiesWavesStageConfig cachedConfig))
                return cachedConfig;

            EnemiesWavesStageConfig config = _generator.Generate(stageNumber);

            _wavesConfigs.Add(stageNumber, config);

            foreach (EnemiesWaveConfig stream in config.EnemiesWaveConfigs)
                _streamsWavesNumbers.Add(stream, stageNumber);

            return config;
        }

        public bool TryGetWaveNumber(EnemiesWaveConfig stream, out int waveNumber)
            => _streamsWavesNumbers.TryGetValue(stream, out waveNumber);
    }
}
