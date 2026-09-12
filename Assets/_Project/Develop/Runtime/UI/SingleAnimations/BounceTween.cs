using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.UI.SingleAnimations
{
    public class BounceTween : MonoBehaviour
    {
        [Header("Bounce settings")]
        [Range(0.1f, 5f)]
        public float duration = 0.6f;        // длительность одного прохода (туда-обратно считается как 2x)
        [Range(0.1f, 3f)]
        public float minScale = 0.9f;        // минимальный скейл
        [Range(0.1f, 3f)]
        public float maxScale = 1.1f;        // максимальный скейл

        private Tweener _bounceTween;

        private void OnEnable()
        {
            // Сохраняем исходный скейл, чтобы не ломать другие анимации
            var startScale = transform.localScale;

            // Создаём твин: от minScale до maxScale и обратно (Yoyo)
            _bounceTween = transform
                .DOScale(new Vector3(maxScale, maxScale, maxScale), duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false); // не убивать автоматически, чтобы можно было контролировать вручную
        }

        private void OnDisable()
        {
            // При отключении объекта убиваем твин, чтобы он не продолжал работать
            if (_bounceTween != null && _bounceTween.IsActive())
            {
                _bounceTween.Kill();
                _bounceTween = null;
            }

            // Опционально: вернуть скейл к исходному значению
            // transform.localScale = Vector3.one; // или сохраните startScale в поле, если нужно точнее
        }

        // Если нужно вручную остановить/запустить:
        public void StopBounce()
        {
            if (_bounceTween != null && _bounceTween.IsActive())
            {
                _bounceTween.Kill();
                _bounceTween = null;
            }
        }

        public void StartBounce()
        {
            // Пересоздаём твин (аналогично OnEnable)
            OnEnable();
        }
    }
}
