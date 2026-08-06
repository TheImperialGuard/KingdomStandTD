using UnityEngine;

public class TransformToggler : MonoBehaviour
{
    [Header("Enable toggling for these transforms")]
    [Space]
    [SerializeField] private bool _togglePosition;
    [SerializeField] private bool _toggleRotation;
    [SerializeField] private bool _toggleScale;

    [Header("Target values")]
    [Space]
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private Vector3 _targetRotation;
    [SerializeField] private Vector3 _targetScale = Vector3.one;

    [Header("Timing")]
    [Space]
    [SerializeField, Min(0.01f)] private float _toggleInterval = 1f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _initialScale;

    private bool _isTargetState;
    private float _timer;

    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
        _initialScale = transform.localScale;
        _timer = 0f;
        _isTargetState = false;
    }

    private void Update()
    {
        if (!_togglePosition && !_toggleRotation && !_toggleScale)
            return;

        _timer += Time.deltaTime;

        if (_timer >= _toggleInterval)
        {
            _timer -= _toggleInterval;
            _isTargetState = !_isTargetState;

            ApplyTransform();
        }
    }

    private void ApplyTransform()
    {
        if (_togglePosition)
        {
            transform.position = _isTargetState ? _targetPosition : _initialPosition;
        }

        if (_toggleRotation)
        {
            transform.rotation = _isTargetState ? Quaternion.Euler(_targetRotation) : _initialRotation;
        }

        if (_toggleScale)
        {
            transform.localScale = _isTargetState ? _targetScale : _initialScale;
        }
    }
}