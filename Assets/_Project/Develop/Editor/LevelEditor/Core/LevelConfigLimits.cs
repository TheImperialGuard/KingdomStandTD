namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Нижние границы значений уровня — как атрибуты Min у полей конфигов. Импорт и SerializedObject
    // эти атрибуты обходят, поэтому по границам проверяет валидатор и ограничивает ввод окно редактора.
    public static class LevelConfigLimits
    {
        public const int MIN_START_GOLD = 0;
        public const int MIN_PLAYER_HEALTH = 1;
        public const float MIN_STAGE_TIME = 0f;
        public const float MIN_TIME_TO_SKIP_STAGE = 0f;
        public const int MIN_PATTERN_REPEATS = 1;
        public const float MIN_START_DELAY = 0f;
        public const float MIN_DELAY_BETWEEN_SPAWNS = 0.1f;
        public const int MIN_GROUP_COUNT = 1;
    }
}
