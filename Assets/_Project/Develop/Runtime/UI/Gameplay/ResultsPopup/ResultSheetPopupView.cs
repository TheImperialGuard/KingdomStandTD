using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Functional;
using Assets._Project.Develop.Runtime.Utilities.Browser;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup
{
    // Окно результата боя — королевская грамота (result-screens spec.md): лист в масштабе меню по центру safe area,
    // заголовок и указ. Середину листа, кнопки и печать задаёт наследник. Раскладку окно считает само: перед показом и
    // при смене размера экрана.
    public abstract class ResultSheetPopupView : PopupViewBase
    {
        [SerializeField] private RectTransform _fit;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _decree;

        private Canvas _canvas;

        private Canvas RootCanvas => _canvas != null ? _canvas : _canvas = GetComponentInParent<Canvas>().rootCanvas;

        public void SetTitle(string title) => _title.text = title;

        public void SetDecree(string decree) => _decree.text = decree;

        protected override void OnPreShow()
        {
            base.OnPreShow();

            UpdateLayout();
        }

        // Корень растянут на слой попапов и меняет размер вместе с экраном, поэтому своего Update не нужно.
        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled)
                UpdateLayout();
        }

        // Кадры грамот собраны в масштабе меню, как пауза: корень листа масштабируется на отношение масштабов канвасов
        // меню и боя и ставится в центр safe area. Сжатие под высоту экрана не нужно: самый высокий лист помещается везде.
        private void UpdateLayout()
        {
            Vector2 screenSize = new(Screen.width, Screen.height);
            float battleScale = RootCanvas.scaleFactor;
            float menuScale = MenuCanvasScale.Calculate(screenSize, MenuCanvasScale.IsPhone(BrowserScreen.LogicalHeight));
            float scale = menuScale / battleScale;

            _fit.localScale = new Vector3(scale, scale, 1f);
            _fit.anchoredPosition = (Screen.safeArea.center - screenSize / 2f) / battleScale;
        }
    }
}
