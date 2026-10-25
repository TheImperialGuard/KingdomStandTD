#if KS_PERF_PROBE
using System;
using System.Collections.Generic;
using System.Text;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Unity.Profiling;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PerformanceProbe
{
    // Замер для тестовых сборок под символом KS_PERF_PROBE — в релиз ни этот файл, ни обращения к нему не попадают.
    // Живой счётчик кадра и прогон по фазам, в которых у всех врагов по очереди выключаются модели, аниматоры и полоски
    // здоровья: по разнице времени кадра между фазами видно, какая часть врага дороже. Отладочный инструмент, поэтому
    // части врага ищет сам, через GetComponentsInChildren, а не через регистраторы сущности
    public class PerformanceProbeService : IInitializable, IDisposable
    {
        private const float CounterWindow = 1f;
        private const float PhaseSettleTime = 0.5f;
        private const float PhaseMeasureTime = 3f;
        private const int NoPhase = -1;
        private const string DrawCallsCounterName = "Draw Calls Count";
        private const string MissingValue = "-";

        private static readonly ProbePhase[] _phases =
        {
            new("base", EnemyParts.None),
            new("no render", EnemyParts.Models),
            new("no anim", EnemyParts.Animators),
            new("no bars", EnemyParts.HealthBars),
            new("all off", EnemyParts.All),
            new("base", EnemyParts.None),
        };

        private readonly EntitiesLifeContext _entitiesLifeContext;

        // Полоски здоровья всех врагов — один слой интерфейса, поэтому выключаются целиком, а не у каждого врага
        private readonly Canvas _healthBarsCanvas;

        private readonly Dictionary<Entity, EnemyObject> _liveEnemies = new();
        private readonly Dictionary<Transform, EnemyObject> _enemyObjects = new();

        private readonly FrameStats _counterStats = new();
        private readonly FrameStats _phaseStats = new();
        private readonly StringBuilder _resultsBuilder = new();

        private PerformanceProbeOverlay _overlay;
        private ProfilerRecorder _drawCallsRecorder;

        private EnemyParts _disabledParts;

        private int _phaseIndex = NoPhase;
        private float _phaseTime;

        public PerformanceProbeService(EntitiesLifeContext entitiesLifeContext, Canvas healthBarsCanvas)
        {
            _entitiesLifeContext = entitiesLifeContext;
            _healthBarsCanvas = healthBarsCanvas;
        }

        public void Initialize()
        {
            _drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DrawCallsCounterName);

            _overlay = new GameObject(nameof(PerformanceProbeOverlay)).AddComponent<PerformanceProbeOverlay>();
            _overlay.RunClicked += OnRunClicked;

            _entitiesLifeContext.Added += OnEntityAdded;
            _entitiesLifeContext.Released += OnEntityReleased;
        }

        public void Dispose()
        {
            _entitiesLifeContext.Added -= OnEntityAdded;
            _entitiesLifeContext.Released -= OnEntityReleased;

            if (_overlay != null)
            {
                _overlay.RunClicked -= OnRunClicked;
                GameObject.Destroy(_overlay.gameObject);
            }

            _drawCallsRecorder.Dispose();

            _liveEnemies.Clear();
            _enemyObjects.Clear();
        }

        public void Update(float deltaTime)
        {
            long drawCalls = _drawCallsRecorder.Valid ? _drawCallsRecorder.LastValue : 0;
            int enemiesCount = _liveEnemies.Count;

            _counterStats.Add(deltaTime, drawCalls, enemiesCount);

            if (_counterStats.TotalTime >= CounterWindow)
            {
                _overlay.SetCounter(FormatCounter(_counterStats));
                _counterStats.Reset();
            }

            if (_phaseIndex != NoPhase)
                UpdatePhase(deltaTime, drawCalls, enemiesCount);
        }

        private void OnRunClicked()
        {
            if (_phaseIndex != NoPhase)
                return;

            _resultsBuilder.Clear();
            _resultsBuilder.AppendLine("phase: frame ms, fps, enemies, draw calls");

            StartPhase(0);
        }

        private void UpdatePhase(float deltaTime, long drawCalls, int enemiesCount)
        {
            _phaseTime += deltaTime;

            // Первые кадры после переключения не считаются: в них ещё видна цена самого переключения
            if (_phaseTime <= PhaseSettleTime)
                return;

            _phaseStats.Add(deltaTime, drawCalls, enemiesCount);

            if (_phaseTime < PhaseSettleTime + PhaseMeasureTime)
                return;

            _resultsBuilder.AppendLine(FormatPhaseResult(_phaseIndex, _phaseStats));
            _overlay.SetResults(_resultsBuilder.ToString());

            StartPhase(_phaseIndex + 1);
        }

        private void StartPhase(int phaseIndex)
        {
            if (phaseIndex >= _phases.Length)
            {
                _phaseIndex = NoPhase;
                SetDisabledParts(EnemyParts.None);
                _overlay.SetRunning(false, string.Empty);

                return;
            }

            _phaseIndex = phaseIndex;
            _phaseTime = 0;
            _phaseStats.Reset();

            ProbePhase phase = _phases[phaseIndex];
            int phaseNumber = phaseIndex + 1;

            SetDisabledParts(phase.DisabledParts);
            _overlay.SetRunning(true, $"phase {phaseNumber}/{_phases.Length}: {phase.Name}");
        }

        private void SetDisabledParts(EnemyParts disabledParts)
        {
            _disabledParts = disabledParts;

            foreach (EnemyObject enemyObject in _liveEnemies.Values)
                enemyObject.Apply(disabledParts);

            _healthBarsCanvas.enabled = (disabledParts & EnemyParts.HealthBars) == 0;
        }

        // Враг, появившийся посреди прогона, получает выключатели текущей фазы
        private void OnEntityAdded(Entity entity)
        {
            if (entity.HasComponent<EnemyKind>() == false)
                return;

            EnemyObject enemyObject = GetEnemyObject(entity.Transform);
            enemyObject.Apply(_disabledParts);

            _liveEnemies.Add(entity, enemyObject);
        }

        // В пул объект уходит со всеми частями включёнными, как свежий экземпляр
        private void OnEntityReleased(Entity entity)
        {
            if (_liveEnemies.Remove(entity, out EnemyObject enemyObject))
                enemyObject.Apply(EnemyParts.None);
        }

        private EnemyObject GetEnemyObject(Transform root)
        {
            if (_enemyObjects.TryGetValue(root, out EnemyObject enemyObject) == false)
            {
                enemyObject = new EnemyObject(root);
                _enemyObjects.Add(root, enemyObject);
            }

            return enemyObject;
        }

        private string FormatCounter(FrameStats stats)
        {
            float fps = stats.Fps;
            float averageMilliseconds = stats.AverageFrameMilliseconds;
            float worstMilliseconds = stats.WorstFrameMilliseconds;
            int enemies = stats.AverageEnemies;
            string drawCalls = FormatDrawCalls(stats);

            return $"FPS {fps:F1}   avg {averageMilliseconds:F1} ms   worst {worstMilliseconds:F1} ms\n" +
                $"enemies {enemies}   draw calls {drawCalls}";
        }

        private string FormatPhaseResult(int phaseIndex, FrameStats stats)
        {
            int phaseNumber = phaseIndex + 1;
            string phaseName = _phases[phaseIndex].Name;
            float averageMilliseconds = stats.AverageFrameMilliseconds;
            float fps = stats.Fps;
            int enemies = stats.AverageEnemies;
            string drawCalls = FormatDrawCalls(stats);

            return $"{phaseNumber}. {phaseName}: {averageMilliseconds:F1} ms, {fps:F1} fps, {enemies}, {drawCalls}";
        }

        // Счётчик вызовов отрисовки в релизной сборке может быть недоступен — тогда прочерк, а не ноль
        private string FormatDrawCalls(FrameStats stats)
            => _drawCallsRecorder.Valid ? stats.AverageDrawCalls.ToString() : MissingValue;

        [Flags]
        private enum EnemyParts
        {
            None = 0,
            Models = 1 << 0,
            Animators = 1 << 1,
            HealthBars = 1 << 2,
            All = Models | Animators | HealthBars
        }

        private readonly struct ProbePhase
        {
            public readonly string Name;
            public readonly EnemyParts DisabledParts;

            public ProbePhase(string name, EnemyParts disabledParts)
            {
                Name = name;
                DisabledParts = disabledParts;
            }
        }

        // Части одного объекта врага. Берутся только включённые в префабе: выключенное префабом замер не трогает
        private class EnemyObject
        {
            private readonly SkinnedMeshRenderer[] _models;
            private readonly Animator[] _animators;

            public EnemyObject(Transform root)
            {
                _models = Array.FindAll(root.GetComponentsInChildren<SkinnedMeshRenderer>(true), model => model.enabled);
                _animators = Array.FindAll(root.GetComponentsInChildren<Animator>(true), animator => animator.enabled);
            }

            public void Apply(EnemyParts disabledParts)
            {
                SetEnabled(_models, (disabledParts & EnemyParts.Models) == 0);
                SetEnabled(_animators, (disabledParts & EnemyParts.Animators) == 0);
            }

            private void SetEnabled(SkinnedMeshRenderer[] models, bool isEnabled)
            {
                foreach (SkinnedMeshRenderer model in models)
                    model.enabled = isEnabled;
            }

            private void SetEnabled(Behaviour[] behaviours, bool isEnabled)
            {
                foreach (Behaviour behaviour in behaviours)
                    behaviour.enabled = isEnabled;
            }
        }

        private class FrameStats
        {
            private const float MillisecondsInSecond = 1000f;

            private int _frames;
            private float _worstFrameTime;
            private long _drawCallsSum;
            private long _enemiesSum;

            public float TotalTime { get; private set; }

            public float Fps => TotalTime > 0 ? _frames / TotalTime : 0;

            public float AverageFrameMilliseconds => _frames > 0 ? TotalTime / _frames * MillisecondsInSecond : 0;

            public float WorstFrameMilliseconds => _worstFrameTime * MillisecondsInSecond;

            public int AverageEnemies => _frames > 0 ? Mathf.RoundToInt((float)_enemiesSum / _frames) : 0;

            public long AverageDrawCalls => _frames > 0 ? _drawCallsSum / _frames : 0;

            public void Add(float deltaTime, long drawCalls, int enemiesCount)
            {
                _frames++;
                TotalTime += deltaTime;
                _worstFrameTime = Mathf.Max(_worstFrameTime, deltaTime);
                _drawCallsSum += drawCalls;
                _enemiesSum += enemiesCount;
            }

            public void Reset()
            {
                _frames = 0;
                TotalTime = 0;
                _worstFrameTime = 0;
                _drawCallsSum = 0;
                _enemiesSum = 0;
            }
        }
    }
}
#endif
