using Assets._Project.Develop.Runtime.UI.Core.Views;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.BossHealthBar
{
    // Полоска здоровья босса вверху HUD: при уроне заливка сразу идёт к новому значению, светлый след догоняет её после
    // паузы; блок выезжает сверху при появлении и уезжает вверх, угасая, после гибели. Твины на масштабируемом времени,
    // в отличие от прочего интерфейса: полоска показывает состояние боя и в паузе стоит вместе с ним
    public class BossHealthBarView : MonoBehaviour, IView
    {
        private const float FillDuration = 0.12f;
        private const float TrailDelay = 0.4f;
        private const float TrailDuration = 0.35f;
        private const float AppearDuration = 0.3f;
        private const float LeaveDelay = 0.5f;
        private const float LeaveDuration = 0.3f;
        private const float HiddenOffset = 18f;

        [SerializeField] private RectTransform _rect;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image _fill;
        [SerializeField] private Image _trail;
        [SerializeField] private TMP_Text _name;

        // Обводка имени — отдельный текст под ним: у одного TMP обводка каждой буквы ложится на лицо предыдущей, а в
        // макете обводка (text-shadow) лежит под всеми буквами
        [SerializeField] private TMP_Text _nameOutline;

        private float _health;
        private float _restY;
        private bool _isRestYKnown;

        private Tween _fillAnimation;
        private Tween _trailAnimation;
        private Sequence _blockAnimation;

        public void Show(string bossName, float health)
        {
            gameObject.SetActive(true);
            RememberRestY();

            KillAll();

            _name.text = bossName;
            _nameOutline.text = bossName;
            _health = health;
            _fill.fillAmount = health;
            _trail.fillAmount = health;

            _group.alpha = 0f;
            _rect.anchoredPosition = new Vector2(_rect.anchoredPosition.x, _restY + HiddenOffset);

            _blockAnimation = DOTween.Sequence()
                .Insert(0f, _group.DOFade(1f, AppearDuration).SetEase(Ease.OutQuad))
                .Insert(0f, _rect.DOAnchorPosY(_restY, AppearDuration).SetEase(Ease.OutQuad));
        }

        public void SetHealth(float health)
        {
            _fillAnimation?.Kill();
            _trailAnimation?.Kill();

            bool isDamage = health < _health;
            _health = health;

            if (isDamage == false)
            {
                _fill.fillAmount = health;
                _trail.fillAmount = health;
                return;
            }

            _fillAnimation = _fill.DOFillAmount(health, FillDuration).SetEase(Ease.Linear);

            _trailAnimation = _trail.DOFillAmount(health, TrailDuration)
                .SetDelay(TrailDelay)
                .SetEase(Ease.OutQuad);
        }

        public void Leave()
        {
            _blockAnimation?.Kill();

            _blockAnimation = DOTween.Sequence()
                .AppendInterval(LeaveDelay)
                .Append(_group.DOFade(0f, LeaveDuration).SetEase(Ease.OutQuad))
                .Join(_rect.DOAnchorPosY(_restY + HiddenOffset, LeaveDuration).SetEase(Ease.OutQuad))
                .OnComplete(Hide);
        }

        private void OnDestroy() => KillAll();

        // Корень выключен в префабе до первого босса, поэтому место покоя запоминается при первом показе
        private void RememberRestY()
        {
            if (_isRestYKnown)
                return;

            _restY = _rect.anchoredPosition.y;
            _isRestYKnown = true;
        }

        private void Hide()
        {
            KillAll();
            gameObject.SetActive(false);
        }

        private void KillAll()
        {
            _fillAnimation?.Kill();
            _trailAnimation?.Kill();
            _blockAnimation?.Kill();
        }
    }
}
