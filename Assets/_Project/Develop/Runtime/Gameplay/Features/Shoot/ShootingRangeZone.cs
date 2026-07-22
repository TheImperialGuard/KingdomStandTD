using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class ShootingRangeZone : MonoBehaviour
    {
        public void SetRange(float range)
        {
            Vector3 scale = new(range * 2, transform.localScale.y, range * 2);

            transform.localScale = scale;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
