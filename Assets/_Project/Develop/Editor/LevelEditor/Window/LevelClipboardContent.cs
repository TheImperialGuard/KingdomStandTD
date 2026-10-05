using System.Collections.Generic;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Разобранное содержимое буфера обмена: срез уровня, стейджи или волны. Если разобрать не удалось — текст ошибки.
    public class LevelClipboardContent
    {
        private LevelClipboardContent(LevelClipboardContentKind kind)
        {
            Kind = kind;
        }

        public LevelClipboardContentKind Kind { get; }
        public LevelSlice Level { get; private set; }
        public IReadOnlyList<StageSlice> Stages { get; private set; }
        public IReadOnlyList<WaveSlice> Waves { get; private set; }
        public string Error { get; private set; }

        public static LevelClipboardContent CreateLevel(LevelSlice level)
            => new(LevelClipboardContentKind.Level) { Level = level };

        public static LevelClipboardContent CreateStages(IReadOnlyList<StageSlice> stages)
            => new(LevelClipboardContentKind.Stages) { Stages = stages };

        public static LevelClipboardContent CreateWaves(IReadOnlyList<WaveSlice> waves)
            => new(LevelClipboardContentKind.Waves) { Waves = waves };

        public static LevelClipboardContent CreateError(string error)
            => new(LevelClipboardContentKind.Invalid) { Error = error };
    }
}
