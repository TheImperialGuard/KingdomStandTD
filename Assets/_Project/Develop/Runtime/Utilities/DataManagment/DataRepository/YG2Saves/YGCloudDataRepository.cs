using System;
using System.Collections;
using System.Collections.Generic;
using YG;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment.DataRepository.YG2Saves
{
    public class YGCloudDataRepository : IDataRepository
    {
        private Dictionary<string, string> CloudData => YG2.saves.Data;

        public IEnumerator Exists(string key, Action<bool> onExistsResult)
        {
            bool exists = CloudData.ContainsKey(key);

            onExistsResult?.Invoke(exists);

            yield break;
        }

        public IEnumerator Read(string key, Action<string> onRead)
        {
            string text = CloudData[key];

            onRead?.Invoke(text);

            yield break;
        }

        public IEnumerator Remove(string key)
        {
            CloudData.Remove(key);

            YG2.SaveProgress();

            yield break;
        }

        public IEnumerator Write(string key, string serializedData)
        {
            CloudData[key] = serializedData;

            YG2.SaveProgress();

            yield break;
        }
    }
}
