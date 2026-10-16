using Assets._Project.Develop.Runtime.Infrastructure.DI;

namespace Assets._Project.Develop.Runtime.UI.ProjectContext
{
    // Презентеры интерфейса, общего для всех сцен, — например, энциклопедии.
    public class ProjectPresentersFactory
    {
        private readonly DIContainer _container;

        public ProjectPresentersFactory(DIContainer container)
        {
            _container = container;
        }
    }
}
