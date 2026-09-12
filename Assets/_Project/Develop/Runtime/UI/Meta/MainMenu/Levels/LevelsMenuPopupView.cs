using Assets._Project.Develop.Runtime.UI.Core.Popups;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Meta.MainMenu.Levels
{
    public class LevelsMenuPopupView : PopupViewBase
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private LevelTitlesListView _levelTitlesListView;

        public LevelTitlesListView LevelTitlesListView => _levelTitlesListView;

        public void SetTitle(string title) => _title.text = title;

        protected override void ModifyShowAnimation(Sequence animation)
        {
            base.ModifyShowAnimation(animation);

            foreach (LevelTileView levelTileView in _levelTitlesListView.Elements)
            {
                animation.Append(levelTileView.Show());
                animation.AppendInterval(0.1f);
            }
        }
    }
}
