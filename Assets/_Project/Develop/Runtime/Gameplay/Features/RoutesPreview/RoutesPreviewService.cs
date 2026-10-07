using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelStages;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.RoutesPreview
{
    // Пока игрок готовится к старту стейджей, пускает цепочки стрелок по всем путям, которые используют волны уровня.
    // Цепочка идёт шагами: стрелка разгорается и гаснет, а на заданной доле её затухания впереди появляется следующая.
    // После старта стейджей стрелки убираются и больше не появляются.
    public class RoutesPreviewService : IInitializable, IDisposable
    {
        private const string RootName = "RoutesPreview";
        private const int FirstStepIndex = 0;
        private const int MinStepsCount = 1;

        private readonly StagesCycle _stagesCycle;
        private readonly IReadOnlyList<RoadPath> _roadPaths;
        private readonly IReadOnlyList<EnemiesWavesStageConfig> _stagesConfigs;
        private readonly RoutesPreviewConfig _config;
        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;

        private readonly List<Route> _routes = new();
        private readonly List<Arrow> _arrows = new();

        private RouteArrowView _arrowPrefab;
        private Transform _root;
        private ObjectPool<RouteArrowView> _arrowsPool;

        private float _timeSinceLastLaunch;
        private bool _isRunning;

        public RoutesPreviewService(
            StagesCycle stagesCycle,
            IReadOnlyList<RoadPath> roadPaths,
            IReadOnlyList<EnemiesWavesStageConfig> stagesConfigs,
            RoutesPreviewConfig config,
            ResourcesAssetsLoader resourcesAssetsLoader)
        {
            _stagesCycle = stagesCycle;
            _roadPaths = roadPaths;
            _stagesConfigs = stagesConfigs;
            _config = config;
            _resourcesAssetsLoader = resourcesAssetsLoader;
        }

        public void Initialize()
        {
            CreateRoutes();

            _arrowPrefab = _resourcesAssetsLoader.Load<RouteArrowView>(_config.PrefabPath);
            _root = new GameObject(RootName).transform;
            _arrowsPool = new ObjectPool<RouteArrowView>(CreateArrowView, view => view.Show(), view => view.Hide());

            _stagesCycle.Launched += OnStagesLaunched;

            // Первые цепочки выходят сразу, без ожидания интервала
            _timeSinceLastLaunch = _config.SpawnInterval;
            _isRunning = true;
        }

        public void Update(float deltaTime)
        {
            if (_isRunning == false)
                return;

            _timeSinceLastLaunch += deltaTime;

            if (_timeSinceLastLaunch >= _config.SpawnInterval)
            {
                _timeSinceLastLaunch -= _config.SpawnInterval;
                LaunchChains();
            }

            UpdateArrows(deltaTime);
        }

        public void Dispose()
        {
            _stagesCycle.Launched -= OnStagesLaunched;
        }

        private void CreateRoutes()
        {
            foreach (PathNumber pathNumber in GetUsedPathNumbers())
            {
                if (TryGetRoadPath(pathNumber, out RoadPath roadPath) == false)
                {
                    Debug.LogWarning($"На уровне нет дороги с номером {pathNumber}, её маршрут не будет показан");
                    continue;
                }

                if (_config.TryGetColor(pathNumber, out Color color) == false)
                {
                    Debug.LogWarning($"В конфиге превью маршрутов нет цвета для пути {pathNumber}, его стрелки будут белыми");
                    color = Color.white;
                }

                RouteLine line = new RouteLine(roadPath, _config.HeightOffset);

                // Шаг чуть уменьшается под длину пути, чтобы последняя стрелка встала ровно на последний вейпоинт
                int stepsCount = Mathf.Max(MinStepsCount, Mathf.CeilToInt(line.Length / _config.StepDistance));

                _routes.Add(new Route(line, color, stepsCount));
            }
        }

        private HashSet<PathNumber> GetUsedPathNumbers()
        {
            HashSet<PathNumber> pathNumbers = new();

            foreach (EnemiesWavesStageConfig stageConfig in _stagesConfigs)
            {
                foreach (EnemiesWaveConfig waveConfig in stageConfig.EnemiesWaveConfigs)
                    pathNumbers.Add(waveConfig.PathNumber);
            }

            return pathNumbers;
        }

        private bool TryGetRoadPath(PathNumber pathNumber, out RoadPath result)
        {
            foreach (RoadPath roadPath in _roadPaths)
            {
                if (roadPath.Number == pathNumber)
                {
                    result = roadPath;
                    return true;
                }
            }

            result = null;
            return false;
        }

        private RouteArrowView CreateArrowView() => GameObject.Instantiate(_arrowPrefab, _root);

        private void LaunchChains()
        {
            foreach (Route route in _routes)
                SpawnArrow(route, FirstStepIndex, 0f);
        }

        private void UpdateArrows(float deltaTime)
        {
            // Новые стрелки добавляются в конец списка, поэтому обход с конца не задевает их в этом кадре
            for (int i = _arrows.Count - 1; i >= 0; i--)
            {
                Arrow arrow = _arrows[i];

                arrow.Age += deltaTime;

                if (IsTimeToSpawnNext(arrow))
                {
                    // Перебор времени переходит к новой стрелке, чтобы длина кадра не сбивала ритм шагов
                    SpawnArrow(arrow.Route, arrow.StepIndex + 1, arrow.Age - _config.StepInterval);
                    arrow.IsNextSpawned = true;
                }

                if (arrow.Age >= _config.FadeDuration)
                {
                    _arrowsPool.Release(arrow.View);
                    _arrows.RemoveAt(i);
                    continue;
                }

                ApplyAppearance(arrow);
            }
        }

        private bool IsTimeToSpawnNext(Arrow arrow)
        {
            return arrow.IsNextSpawned == false
                && arrow.IsLast == false
                && arrow.Age >= _config.StepInterval;
        }

        private void SpawnArrow(Route route, int stepIndex, float age)
        {
            float distance = route.GetStepDistance(stepIndex);

            RouteArrowView view = _arrowsPool.Get();
            view.Place(route.Line.GetPoint(distance), route.Line.GetDirection(distance, _config.LookAheadDistance));

            Arrow arrow = new Arrow(view, route, stepIndex, age);

            ApplyAppearance(arrow);

            _arrows.Add(arrow);
        }

        // Яркость — произведение разгорания и затухания: разгорание смягчает появление,
        // а затухание, как и раньше, идёт с самого появления стрелки
        private void ApplyAppearance(Arrow arrow)
        {
            float lifeProgress = Mathf.Clamp01(arrow.Age / _config.FadeDuration);

            Color color = arrow.Route.Color;
            color.a *= GetAppearProgress(arrow.Age) * (1f - lifeProgress);

            arrow.View.SetColor(color);
            arrow.View.SetScale(_config.ScaleOverLifetime.Evaluate(lifeProgress));
        }

        private float GetAppearProgress(float age)
        {
            if (_config.AppearDuration <= 0)
                return 1f;

            return Mathf.Clamp01(age / _config.AppearDuration);
        }

        private void OnStagesLaunched()
        {
            _isRunning = false;
            _stagesCycle.Launched -= OnStagesLaunched;

            // Корень уничтожается вместе со всеми стрелками, в том числе лежащими в пуле
            GameObject.Destroy(_root.gameObject);

            _arrows.Clear();
            _arrowsPool.Clear();
        }

        private class Route
        {
            public Route(RouteLine line, Color color, int stepsCount)
            {
                Line = line;
                Color = color;
                StepsCount = stepsCount;
            }

            public RouteLine Line { get; }
            public Color Color { get; }
            public int StepsCount { get; }

            public float GetStepDistance(int stepIndex) => Line.Length * stepIndex / StepsCount;
        }

        private class Arrow
        {
            public Arrow(RouteArrowView view, Route route, int stepIndex, float age)
            {
                View = view;
                Route = route;
                StepIndex = stepIndex;
                Age = age;
            }

            public RouteArrowView View { get; }
            public Route Route { get; }
            public int StepIndex { get; }
            public float Age { get; set; }
            public bool IsNextSpawned { get; set; }

            public bool IsLast => StepIndex >= Route.StepsCount;
        }
    }
}
