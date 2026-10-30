namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Endless
{
    // Как таблица значений по волнам продолжается после последнего ключа.
    public enum WaveValueTailModes
    {
        // Держать значение последнего ключа
        Hold,

        // Прибавлять шаг за каждую волну
        Linear,

        // Умножать на (1 + шаг) за каждую волну — сложный процент
        Compound,
    }
}
