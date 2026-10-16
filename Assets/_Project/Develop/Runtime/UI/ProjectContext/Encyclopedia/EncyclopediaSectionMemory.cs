namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Раздел, на котором книгу закрыли в прошлый раз: с него она откроется снова. Живёт до перезапуска игры,
    // в сохранения не пишется.
    public class EncyclopediaSectionMemory
    {
        public EncyclopediaSections LastSection { get; private set; } = EncyclopediaSections.Tutorial;

        public void Remember(EncyclopediaSections section) => LastSection = section;
    }
}
