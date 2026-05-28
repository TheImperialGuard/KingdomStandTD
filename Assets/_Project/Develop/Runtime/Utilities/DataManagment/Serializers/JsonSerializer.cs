using Newtonsoft.Json;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment.Serializers
{
    public class JsonSerializer : IDataSerializer
    {
        public TData Deserialize<TData>(string serializedData)
        {
            return JsonConvert.DeserializeObject<TData>(serializedData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }

        public string Serialize<TData>(TData data)
        {
            Formatting formattingMode = Application.isEditor ? Formatting.Indented : Formatting.None;

            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                Formatting = formattingMode,
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }
    }
}
