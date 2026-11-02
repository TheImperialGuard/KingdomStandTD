using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.SimpleMono
{
    // Покачивание на месте, как у лодки на лёгкой волне: крен, тангаж и подъём идут синусами
    // с разными периодами, поэтому движение не складывается в заметный повтор.
    // Фаза у каждого объекта своя — соседи качаются вразнобой.
    public class MonoFloatingSway : MonoBehaviour
    {
        private const float FullCircle = Mathf.PI * 2f;

        [SerializeField] private float _rollAngle = 1.5f;
        [SerializeField, Min(0.01f)] private float _rollPeriod = 4.2f;
        [SerializeField] private float _pitchAngle = 0.8f;
        [SerializeField, Min(0.01f)] private float _pitchPeriod = 5.7f;
        [SerializeField] private float _bobHeight = 0.015f;
        [SerializeField, Min(0.01f)] private float _bobPeriod = 3.3f;

        private Vector3 _initialLocalPosition;
        private Quaternion _initialLocalRotation;
        private float _phase;

        private void Awake()
        {
            _initialLocalPosition = transform.localPosition;
            _initialLocalRotation = transform.localRotation;
            _phase = Random.value * FullCircle;
        }

        private void Update()
        {
            float time = Time.time;

            float roll = Wave(time, _rollPeriod) * _rollAngle;
            float pitch = Wave(time, _pitchPeriod) * _pitchAngle;
            float bob = Wave(time, _bobPeriod) * _bobHeight;

            transform.localRotation = _initialLocalRotation * Quaternion.Euler(pitch, 0f, roll);
            transform.localPosition = _initialLocalPosition + Vector3.up * bob;
        }

        private float Wave(float time, float period) => Mathf.Sin(time * FullCircle / period + _phase);
    }
}
