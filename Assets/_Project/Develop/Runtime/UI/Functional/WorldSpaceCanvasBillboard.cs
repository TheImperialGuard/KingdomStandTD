using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Functional
{
    public class WorldSpaceCanvasBillboard : MonoBehaviour
    {
        private Camera _targetCamera;

        private void Awake()
        {
            _targetCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_targetCamera == null)
                return;

            transform.rotation = _targetCamera.transform.rotation;
        }
    }
}
