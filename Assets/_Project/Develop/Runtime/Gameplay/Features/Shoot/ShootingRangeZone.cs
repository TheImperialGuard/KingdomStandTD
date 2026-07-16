using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Shoot
{
    public class ShootingRangeZone : MonoBehaviour
    {
        public void SetRange(float range)
        {
            Vector3 scale = new(range, transform.localScale.y, range);

            transform.localScale = scale;
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}
