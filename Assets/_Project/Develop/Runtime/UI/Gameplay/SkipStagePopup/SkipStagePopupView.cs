using Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup
{
    // Кнопка пропуска: поверх лица медальона растёт тёмная дорожка — прошедшая доля окна пропуска; в подсказке — живое
    // золото за досрочный вызов.
    public class SkipStagePopupView : StartStagesPopupView
    {
        [SerializeField] private Image _lifetimeFill;
        [SerializeField] private Sprite _lifetimeTrack;
        [SerializeField] private Sprite _phoneLifetimeTrack;

        [SerializeField] private TMP_Text _earnedGoldText;

        public void SetLifetimeProgress(float progress) => _lifetimeFill.fillAmount = progress;

        public void SetEarnedGold(int gold)
        {
            _earnedGoldText.text = $"+{gold}";
            RefreshTip();
        }

        protected override void ApplyScreenMode(bool isPhone)
        {
            base.ApplyScreenMode(isPhone);

            _lifetimeFill.sprite = isPhone ? _phoneLifetimeTrack : _lifetimeTrack;
        }
    }
}
