using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Rendering
{
    // Подгоняет квадрат, лежащий лицом вверх, под круг заданного радиуса в мировых единицах.
    public static class GroundQuadScaler
    {
        public static void FitToRadius(Transform quad, float radius)
        {
            float diameter = radius * 2;
            Vector3 parentScale = quad.parent.lossyScale;

            // Квадрат лежит лицом вверх, поэтому его локальная ось Y идёт вдоль оси Z родителя
            quad.localScale = new Vector3(diameter / parentScale.x, diameter / parentScale.z, quad.localScale.z);
        }
    }
}
