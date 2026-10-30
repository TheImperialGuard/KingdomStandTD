using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages
{
    // Откуда StageProviderService берёт стейджи: фиксированный список уровня или бесконечный генератор.
    // Номера стейджей — с единицы, как у StageProviderService.CurrentStageNumber.
    public interface IStagesSource
    {
        bool IsFinite { get; }

        // Число стейджей конечного источника; бесконечный на запрос бросает исключение
        int Count { get; }

        bool Has(int stageNumber);

        StageConfig Get(int stageNumber);
    }
}
