using Assets._Project.Develop.Runtime.UI.Gameplay.StartStagesPopup;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.SkipStagePopup
{
    public class SkipStagePopupView : StartStagesPopupView
    {
        [SerializeField] private Image _lifetimeFill;

        public void SetLifetimeProgress(float progress) => _lifetimeFill.fillAmount = progress;
    }
}
