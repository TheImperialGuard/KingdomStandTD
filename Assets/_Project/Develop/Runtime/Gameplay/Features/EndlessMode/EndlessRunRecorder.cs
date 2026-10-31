using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Meta.Features.Endless;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using UnityEngine;
#if UNITY_EDITOR
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
#endif

namespace Assets._Project.Develop.Runtime.Gameplay.Features.EndlessMode
{
    // Засчитывает забег в рекорд бесконечного режима. Результат — волна, на которой забег кончился, то же число, что в
    // HUD. Забег засчитывается при любом конце: поражении, выходе в меню и «Заново» из паузы, — но не в плейтесте из
    // редактора и не при старте с волны N. Повторный вызов безопасен: та же волна рекорд не побьёт.
    public class EndlessRunRecorder
    {
        private readonly StageProviderService _stageProviderService;
        private readonly EndlessStagesSource _stagesSource;
        private readonly EndlessRecordService _recordService;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public EndlessRunRecorder(
            StageProviderService stageProviderService,
            EndlessStagesSource stagesSource,
            EndlessRecordService recordService,
            PlayerDataProvider playerDataProvider,
            ICoroutinesPerformer coroutinesPerformer)
        {
            _stageProviderService = stageProviderService;
            _stagesSource = stagesSource;
            _recordService = recordService;
            _playerDataProvider = playerDataProvider;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public EndlessRunResult Record()
        {
            int reachedWave = _stageProviderService.CurrentStageNumber.Value;
            bool isRecordable = IsRecordable();
            bool isNewRecord = isRecordable && _recordService.TryUpdate(reachedWave);

            // Сохраняется только побитый рекорд: запись данных игрока на Яндекс Играх ограничена по частоте. Запись
            // проходит целиком в первом шаге корутины, ещё до перехода на другую сцену, а исполнитель корутин живёт
            // всю сессию
            if (isNewRecord)
                _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            Debug.Log($"Забег бесконечного режима: волна {reachedWave}, засчитан: {isRecordable}, рекорд: {_recordService.BestWave}");

            return new EndlessRunResult(reachedWave, _recordService.BestWave, isNewRecord);
        }

        private bool IsRecordable()
        {
            #if UNITY_EDITOR
                // Плейтест из редактора уровней рекорд игрока не трогает
                if (PlaytestHooks.IsPlaytestRunning)
                    return false;
            #endif

            // Старт с волны N — только проверка в редакторе
            return _stagesSource.FirstNumber == EndlessStagesSource.DefaultFirstWaveNumber;
        }
    }
}
