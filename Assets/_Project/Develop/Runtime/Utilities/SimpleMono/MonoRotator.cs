using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.SimpleMono
{
    public class MonoRotator : MonoBehaviour
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        [SerializeField] private Axis _axis = Axis.Y;
        [SerializeField] private float _speed = 90f;
        [SerializeField] private bool _clockwise = true;

        private void Update()
        {
            float direction = _clockwise ? 1f : -1f;

            Vector3 delta = _axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.up
            };

            transform.Rotate(delta * (_speed * direction * Time.deltaTime), Space.Self);
        }
    }
}
