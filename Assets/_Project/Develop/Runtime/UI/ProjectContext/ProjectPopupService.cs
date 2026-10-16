using Assets._Project.Develop.Runtime.UI.Core.Popups;
using Assets._Project.Develop.Runtime.UI.Core.Views;
using Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia;

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
    }
}
