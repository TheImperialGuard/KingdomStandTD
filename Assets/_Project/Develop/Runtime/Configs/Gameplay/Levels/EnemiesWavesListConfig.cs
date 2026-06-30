using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Levels/Waves/NewEnemiesWavesListConfig", fileName = "EnemiesWavesListConfig")]
    public class EnemiesWavesListConfig : ScriptableObject
    {
        [SerializeField] private List<WaveToPathNumber> _waveToPathNumbers;

        public List<EnemiesWaveConfig> GetWavesFor(PathNumber pathNumber)
        {
            List<EnemiesWaveConfig> waves = new List<EnemiesWaveConfig>();

            foreach (WaveToPathNumber waveToPathNumber in _waveToPathNumbers)
                if (waveToPathNumber.PathNumber == pathNumber)
                    waves.Add(waveToPathNumber.WaveConfig);

            if (waves.Any() == false)
                throw new ArgumentException($"Path with number {nameof(pathNumber)} not found in config");

            return waves;
        }

        [Serializable]
        private class WaveToPathNumber
        {
            [field: SerializeField] public EnemiesWaveConfig WaveConfig;
            [field: SerializeField] public PathNumber PathNumber;
        }
    }
}
