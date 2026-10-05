using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Буфер обмена окна — JSON в формате срезов Docs/Balance: уровень, стейдж или волна, враги — по имени ассета.
    // Вид содержимого распознаётся по ключам: у уровня есть stages, у стейджа — waves, у волны — groups.
    // Вставить можно и массив стейджей или волн — например, подготовленный в чате.
    public static class LevelClipboard
    {
        private const string STAGES_KEY = "stages";
        private const string WAVES_KEY = "waves";
        private const string GROUPS_KEY = "groups";
        private const string UNKNOWN_CONTENT_ERROR =
            "В буфере обмена не срез уровня, стейджа или волны. Ожидается JSON, как в Docs/Balance/Levels: "
            + "объект уровня (с ключом stages), стейджа (waves), волны (groups) или массив стейджей либо волн.";

        public static void Write(object slice) => WriteJson(LevelSliceSerializer.ToJson(slice));

        public static void WriteJson(string json) => EditorGUIUtility.systemCopyBuffer = json;

        public static LevelClipboardContent Read()
        {
            string json = EditorGUIUtility.systemCopyBuffer?.Trim();

            if (string.IsNullOrEmpty(json))
                return LevelClipboardContent.CreateError("Буфер обмена пуст.");

            try
            {
                return Read(LevelSliceSerializer.ParseToken(json));
            }
            catch (JsonException exception)
            {
                return LevelClipboardContent.CreateError($"Не удалось разобрать срез из буфера обмена: {exception.Message}");
            }
        }

        private static LevelClipboardContent Read(JToken token)
        {
            bool isArray = token is JArray;
            JObject firstItem = isArray ? token.First as JObject : token as JObject;

            if (firstItem == null)
                return LevelClipboardContent.CreateError(UNKNOWN_CONTENT_ERROR);

            if (firstItem.ContainsKey(STAGES_KEY) && isArray == false)
                return LevelClipboardContent.CreateLevel(LevelSliceSerializer.FromToken<LevelSlice>(token));

            if (firstItem.ContainsKey(WAVES_KEY))
                return LevelClipboardContent.CreateStages(ReadList<StageSlice>(token, isArray));

            if (firstItem.ContainsKey(GROUPS_KEY))
                return LevelClipboardContent.CreateWaves(ReadList<WaveSlice>(token, isArray));

            return LevelClipboardContent.CreateError(UNKNOWN_CONTENT_ERROR);
        }

        private static List<T> ReadList<T>(JToken token, bool isArray)
        {
            if (isArray)
                return LevelSliceSerializer.FromToken<List<T>>(token);

            return new List<T> { LevelSliceSerializer.FromToken<T>(token) };
        }
    }
}
