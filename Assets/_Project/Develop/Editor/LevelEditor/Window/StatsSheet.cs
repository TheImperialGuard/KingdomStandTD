using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Строки «название — значение» внутри блока статистики.
    public class StatsSheet
    {
        private const string ROW_CLASS = "stats-row";
        private const string NAME_CLASS = "stats-row__name";
        private const string VALUE_CLASS = "stats-row__value";
        private const string WARNING_ROW_CLASS = "stats-row--warning";
        private const string NESTED_ROW_CLASS = "stats-row--nested";

        private readonly VisualElement _container;

        public StatsSheet(VisualElement container)
        {
            _container = container;
        }

        public void Clear() => _container.Clear();

        public void AddRow(string name, string value, string rowTooltip = null, bool isWarning = false)
            => _container.Add(CreateRow(name, value, rowTooltip, isWarning));

        // Вложенная строка — с отступом, например враг в составе стейджа.
        public void AddNestedRow(string name, string value)
        {
            VisualElement row = CreateRow(name, value, null, false);
            row.AddToClassList(NESTED_ROW_CLASS);

            _container.Add(row);
        }

        private static VisualElement CreateRow(string name, string value, string rowTooltip, bool isWarning)
        {
            VisualElement row = new()
            {
                tooltip = rowTooltip,
            };

            row.AddToClassList(ROW_CLASS);
            row.EnableInClassList(WARNING_ROW_CLASS, isWarning);

            Label nameLabel = new(name);
            nameLabel.AddToClassList(NAME_CLASS);

            Label valueLabel = new(value);
            valueLabel.AddToClassList(VALUE_CLASS);

            row.Add(nameLabel);
            row.Add(valueLabel);

            return row;
        }
    }
}
