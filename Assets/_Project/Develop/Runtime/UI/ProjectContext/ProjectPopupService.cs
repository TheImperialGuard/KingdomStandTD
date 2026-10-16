using System;
using Assets._Project.Develop.Runtime.Configs.Encyclopedia;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Towers;
using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext
{
    // Попапы, общие для всех сцен. Создаются на слое попапов текущей сцены; как вести себя книге, задаёт сцена.
    public abstract class ProjectPopupService : PopupService
    {
        protected readonly ProjectPresentersFactory ProjectPresentersFactory;

        public ProjectPopupService(ViewsFactory viewsFactory, ProjectPresentersFactory projectPresentersFactory)
            : base(viewsFactory)
        {
            ProjectPresentersFactory = projectPresentersFactory;
        }

        protected abstract EncyclopediaModes EncyclopediaMode { get; }

        // Книга на последнем открытом разделе (Р-22).
        public EncyclopediaPopupPresenter OpenEncyclopediaPopup(Action closedCallback = null)
            => OpenEncyclopediaPopupAt(null, closedCallback);

        // Книга на развороте записи (Р-14); если записи в книге нет — на первом развороте её раздела.
        public EncyclopediaPopupPresenter OpenEncyclopediaPopup(CharacterConfig enemy, Action closedCallback = null)
            => OpenEncyclopediaPopupAt(enemy, closedCallback);

        public EncyclopediaPopupPresenter OpenEncyclopediaPopup(ShootingTowerConfig tower, Action closedCallback = null)
            => OpenEncyclopediaPopupAt(tower, closedCallback);

        public EncyclopediaPopupPresenter OpenEncyclopediaPopup(EncyclopediaNoteConfig note, Action closedCallback = null)
            => OpenEncyclopediaPopupAt(note, closedCallback);

        private EncyclopediaPopupPresenter OpenEncyclopediaPopupAt(ScriptableObject startRecord, Action closedCallback)
        {
            EncyclopediaPopupView view = ViewsFactory.Create<EncyclopediaPopupView>(ViewIDs.EncyclopediaPopup, PopupLayer);

            EncyclopediaPopupPresenter popup = ProjectPresentersFactory
                .CreateEncyclopediaPopupPresenter(view, EncyclopediaMode, startRecord);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }
    }
}
