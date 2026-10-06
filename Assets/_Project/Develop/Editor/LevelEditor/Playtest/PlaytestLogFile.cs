using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Файлы логов плейтеста — Logs/Playtests в корне проекта: папка Logs в .gitignore, в git логи не попадают.
    // Формат JSON тот же, что у срезов.
    public static class PlaytestLogFile
    {
        private const string LOGS_FOLDER = "Logs/Playtests";
        private const string FILE_TIME_FORMAT = "yyyy-MM-dd_HH-mm-ss";
        private const string FILE_EXTENSION = ".json";

        private static readonly Encoding _utf8WithoutBom = new UTF8Encoding(false);

        public static string CreatePath(string levelName, DateTime startedAt)
        {
            string fileTime = startedAt.ToString(FILE_TIME_FORMAT, CultureInfo.InvariantCulture);

            return LevelSliceExporter.ToFullPath($"{LOGS_FOLDER}/{fileTime}_{levelName}{FILE_EXTENSION}");
        }

        public static void Write(string path, PlaytestLog log)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, LevelSliceSerializer.ToJson(log), _utf8WithoutBom);
        }
    }
}
