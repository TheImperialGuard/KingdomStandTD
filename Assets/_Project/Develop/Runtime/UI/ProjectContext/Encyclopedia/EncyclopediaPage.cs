namespace Assets._Project.Develop.Runtime.UI.ProjectContext.Encyclopedia
{
    // Разворот книги: раздел и номер записи в нём, по записи на разворот.
    public readonly struct EncyclopediaPage
    {
        public EncyclopediaPage(EncyclopediaSections section, int spread)
        {
            Section = section;
            Spread = spread;
        }

        public EncyclopediaSections Section { get; }

        public int Spread { get; }
    }
}
