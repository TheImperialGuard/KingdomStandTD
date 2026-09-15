using Assets._Project.Develop.Runtime.Meta.Features.Levels;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopup
{
    public partial class WinPopupView : PopupViewBase
    {
        public event Action ContinueClicked;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private List<Transform> _stars;

        private LevelResults _levelResults;

        public void SetTitle(string title) => _title.text = title;
        public void SetResults(LevelResults results) => _levelResults = results;

        public void OnContinueClick() => ContinueClicked?.Invoke();

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            int starsCount = _levelResults switch
            {
                LevelResults.Perfect => 3,
                LevelResults.Average => 2,
                LevelResults.Bad => 1,
                LevelResults.Defeat => throw new InvalidOperationException("Win popup opened on defeat level results"),
                _ => throw new ArgumentException($"{_levelResults} not supported in {this}"),
            };

            if (starsCount < _stars.Count)
            {
                for (int i = starsCount; i < _stars.Count; i++)
                {
                    _stars[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < starsCount; i++)
            {
                animation
                    .Append(_stars[i].DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0))
                    .Join(_stars[i].DOLocalRotate(Vector3.forward * 360, 0.3f, RotateMode.LocalAxisAdd)
                        .SetEase(Ease.OutCubic)
                        .From(Vector3.zero));
                animation.AppendInterval(0.1f);
            }
        }
    }
}
