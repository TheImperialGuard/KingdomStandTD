using System.Collections.Generic;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    public class LevelSliceImportResult
    {
        public LevelSliceImportResult(string levelName)
        {
            LevelName = levelName;
        }

        public string LevelName { get; }
        public List<string> Changes { get; } = new();
        public List<string> AnalysisChanges { get; } = new();
        public List<string> Errors { get; } = new();
        public List<string> Warnings { get; } = new();
        public bool IsApplied { get; set; }

        public bool HasChanges => Changes.Count > 0;
        public bool HasErrors => Errors.Count > 0;
    }
}
