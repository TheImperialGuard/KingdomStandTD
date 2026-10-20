namespace Assets._Project.Develop.Runtime.UI.Gameplay.TowerPopups
{
    // Строки боковой подсказки окна башни. Пустая строка прячет свой ряд: подзаголовок и параметры есть только у
    // способностей, большое число — только у продажи.
    public class TowerTipContent
    {
        public TowerTipContent(
            string title,
            string description,
            string subtitle = null,
            string parameters = null,
            string bigNumber = null)
        {
            Title = title;
            Description = description;
            Subtitle = subtitle;
            Parameters = parameters;
            BigNumber = bigNumber;
        }

        public string Title { get; }
        public string Description { get; }
        public string Subtitle { get; }
        public string Parameters { get; }
        public string BigNumber { get; }
    }
}
