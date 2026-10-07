using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.RoutesPreview
{
    // Одна стрелка превью маршрута. Цвет задаётся через MaterialPropertyBlock,
    // поэтому стрелки разных путей и разной прозрачности не плодят копии материала.
    public class RouteArrowView : MonoBehaviour
    {
        private static readonly int _baseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private MeshRenderer _renderer;

        private MaterialPropertyBlock _propertyBlock;
        private Vector3 _baseScale;

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);

        public void Place(Vector3 position, Vector3 direction)
        {
            transform.position = position;

            if (direction.sqrMagnitude > 0)
                transform.rotation = Quaternion.LookRotation(direction);
        }

        public void SetColor(Color color)
        {
            _propertyBlock.SetColor(_baseColorId, color);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        // Множитель к масштабу, заданному в префабе
        public void SetScale(float scaleMultiplier)
        {
            transform.localScale = _baseScale * scaleMultiplier;
        }

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
            _baseScale = transform.localScale;
        }
    }
}
