using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Net;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic
{
    public class BallisticTrajectoryCalculateSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<TrajectoryResult> _trajectoryResult;

        private ReactiveVariable<Entity> _currentTarget;
        private ReactiveVariable<float> _projectileSpeed;
        private ReactiveVariable<float> _trajectoryMaxHeight;

        private Transform _shootPoint;

        private Vector3 _previousTargetPos;

        public void OnInit(Entity entity)
        {
            _trajectoryResult = entity.BallisticTrajectory;

            _currentTarget = entity.CurrentTarget;
            _projectileSpeed = entity.ProjectileSpeed;
            _trajectoryMaxHeight = entity.BallisticTrajectoryMaxHeight;

            _shootPoint = entity.ShootPoint;

            _previousTargetPos = Vector3.zero;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_currentTarget.Value != null)
            {
                Vector3 leadPoint = CalculateLeadPoint();

                _trajectoryResult.Value = CalculateTrajectory(
                _shootPoint.position,
                leadPoint,
                _projectileSpeed.Value,
                _trajectoryMaxHeight.Value);
            }
        }

        private Vector3 CalculateLeadPoint(int iterations = 3)
        {
            if (_currentTarget.Value.TryGetAimingPoint(out Transform aimingPoint) == false)
                throw new Exception($"Not found aiming point for target: {_currentTarget.Value.Transform.gameObject.name}");

            if (aimingPoint == null)
                return _previousTargetPos;

            Vector3 currentTargetPosition = aimingPoint.position;
            Vector3 currentTargetSpeed = _currentTarget.Value.Rigidbody.linearVelocity;
            Vector3 shootPointPosition = _shootPoint.position;

            float projectileSpeed = _projectileSpeed.Value;
            float maxHeight = _trajectoryMaxHeight.Value;
            float g = Mathf.Abs(Physics.gravity.y);

            // Если цель стоит на месте — упреждение не нужно
            if (currentTargetSpeed == Vector3.zero || currentTargetSpeed.magnitude < 0.001f)
                return currentTargetPosition;

            // === ИТЕРАТИВНЫЙ РАСЧЁТ УПРЕЖДЕНИЯ ===

            Vector3 predictedTargetPosition = currentTargetPosition;
            float flightTime = 0f;

            for (int i = 0; i < iterations; i++)
            {
                // Вектор от точки выстрела до предсказанной позиции цели
                Vector3 toPredictedTarget = predictedTargetPosition - shootPointPosition;

                // Горизонтальное расстояние (XZ плоскость)
                float horizontalDistance = new Vector3(toPredictedTarget.x, 0, toPredictedTarget.z).magnitude;

                // Вертикальная разница
                float verticalDifference = toPredictedTarget.y;

                // === РАСЧЁТ РЕАЛЬНОГО ВРЕМЕНИ ПОЛЁТА ПО БАЛЛИСТИКЕ ===
                // Время подъёма до вершины траектории
                float timeToApex = Mathf.Sqrt(2f * maxHeight / g);

                // Абсолютная высота вершины
                float apexAbsoluteY = shootPointPosition.y + maxHeight;

                // Высота падения с вершины до предсказанной позиции цели
                float fallHeight = apexAbsoluteY - predictedTargetPosition.y;

                // Защита от отрицательной высоты падения (если цель выше траектории)
                if (fallHeight < 0.01f)
                    fallHeight = 0.01f;

                // Время падения с вершины до цели
                float timeFromApexToTarget = Mathf.Sqrt(2f * fallHeight / g);

                // Общее время полёта
                flightTime = timeToApex + timeFromApexToTarget;

                // === ОБНОВЛЯЕМ ПРЕДСКАЗАННУЮ ПОЗИЦИЮ ===
                // Цель будет в этой позиции через flightTime секунд
                predictedTargetPosition = currentTargetPosition + currentTargetSpeed * flightTime;
            }

            _previousTargetPos = predictedTargetPosition;

            return predictedTargetPosition;
        }

        private TrajectoryResult CalculateTrajectory(
        Vector3 spawnPoint,
        Vector3 targetPoint,
        float projectileSpeed,
        float maxHeight,
        float? gravity = null)
        {
            TrajectoryResult result = new TrajectoryResult();
            result.isValid = false;
            result.errorMessage = "";

            // Используем гравитацию из физики Unity, если не передана своя
            float g = gravity ?? Mathf.Abs(Physics.gravity.y);

            // Вектор от точки спавна к цели
            Vector3 toTarget = targetPoint - spawnPoint;

            // Горизонтальное расстояние (XZ плоскость)
            float horizontalDistance = new Vector3(toTarget.x, 0, toTarget.z).magnitude;

            // Вертикальное расстояние
            float verticalDistance = toTarget.y;

            // Проверка валидности входных данных
            if (horizontalDistance < 0.001f)
            {
                result.errorMessage = "Цель слишком близко к точке спавна (горизонтальное расстояние ~0)";
                return result;
            }

            if (projectileSpeed <= 0)
            {
                result.errorMessage = "Скорость снаряда должна быть больше 0";
                return result;
            }

            if (maxHeight <= 0)
            {
                result.errorMessage = "Максимальная высота должна быть больше 0";
                return result;
            }

            // maxHeight задана относительно spawnPoint, поэтому абсолютная высота вершины:
            float apexAbsoluteY = spawnPoint.y + maxHeight;

            // Проверяем, что цель не выше максимальной высоты
            if (targetPoint.y > apexAbsoluteY)
            {
                result.errorMessage = $"Цель (Y={targetPoint.y}) выше максимальной высоты траектории (Y={apexAbsoluteY})";
                return result;
            }

            // === РАСЧЁТ ТРАЕКТОРИИ ===

            // Время подъёма до вершины: t_up = sqrt(2 * maxHeight / g)
            float timeToApex = Mathf.Sqrt(2f * maxHeight / g);

            // Общее время полёта: время до вершины + время падения с вершины до цели
            // Время падения: t_down = sqrt(2 * (apexAbsoluteY - targetY) / g)
            float fallHeight = apexAbsoluteY - targetPoint.y;
            float timeFromApexToTarget = Mathf.Sqrt(2f * fallHeight / g);
            float totalFlightTime = timeToApex + timeFromApexToTarget;

            // Горизонтальная скорость (постоянная на всём протяжении)
            float horizontalVelocity = horizontalDistance / totalFlightTime;

            // Вертикальная скорость в начальный момент: vy = g * timeToApex
            float initialVerticalVelocity = g * timeToApex;

            // Направление горизонтального движения (нормализованный вектор в XZ плоскости)
            Vector3 horizontalDirection = new Vector3(toTarget.x, 0, toTarget.z).normalized;

            // Полный вектор начальной скорости
            Vector3 launchVelocity = horizontalDirection * horizontalVelocity + Vector3.up * initialVerticalVelocity;

            // Проверяем, что полученная скорость не превышает заданную projectileSpeed
            // (из-за ограничений физики может оказаться, что заданная скорость недостаточна)
            float calculatedSpeed = launchVelocity.magnitude;

            if (calculatedSpeed > projectileSpeed)
            {
                result.errorMessage = $"Заданная скорость ({projectileSpeed}) недостаточна для достижения цели с высотой {maxHeight}. Требуется минимум {calculatedSpeed:F2}";
                return result;
            }

            // Направление дула = нормализованный вектор начальной скорости
            Vector3 launchDirection = launchVelocity.normalized;

            // Позиция вершины траектории
            // X и Z вершины находятся на расстоянии (horizontalVelocity * timeToApex) от spawnPoint
            Vector3 apexPosition = spawnPoint + horizontalDirection * (horizontalVelocity * timeToApex) + Vector3.up * maxHeight;

            // Заполняем результат
            result.launchVelocity = launchVelocity;
            result.launchDirection = launchDirection;
            result.flightTime = totalFlightTime;
            result.apexPosition = apexPosition;
            result.isValid = true;

            return result;
        }
    }
}
