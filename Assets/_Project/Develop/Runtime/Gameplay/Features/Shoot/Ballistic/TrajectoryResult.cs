using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot.Ballistic
{
    public struct TrajectoryResult
    {
        public Vector3 launchVelocity;      // Начальный вектор скорости для Rigidbody
        public Vector3 launchDirection;     // Направление дула (куда смотреть пушке)
        public float flightTime;            // Общее время полёта
        public Vector3 apexPosition;        // Верхняя точка траектории
        public bool isValid;                // Валидна ли траектория
        public string errorMessage;         // Сообщение об ошибке, если не валидна
    }
}
