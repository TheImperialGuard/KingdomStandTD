using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Audio
{
    public static class SoundRetriggerLimiter
    {
        private const float MinRetriggerInterval = 0.06f;
        private const int MaxSimultaneousCopies = 4;

        private static readonly Dictionary<AudioClip, List<float>> _startTimes = new();

        public static bool TryStart(AudioClip clip)
        {
            if (clip == null)
                return false;

            float now = Time.realtimeSinceStartup;

            if (_startTimes.TryGetValue(clip, out List<float> startTimes) == false)
            {
                startTimes = new List<float>();
                _startTimes.Add(clip, startTimes);
            }

            startTimes.RemoveAll(startTime => now - startTime >= clip.length);

            if (startTimes.Count >= MaxSimultaneousCopies)
                return false;

            if (startTimes.Count > 0 && now - startTimes[startTimes.Count - 1] < MinRetriggerInterval)
                return false;

            startTimes.Add(now);

            return true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStartTimes()
        {
            _startTimes.Clear();
        }
    }
}
