using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Строка уровня под тулбаром: стартовое золото, здоровье игрока и итоги по уровню.
    public class LevelSummaryBar
    {
        private readonly IntegerField _startGoldField;
        private readonly IntegerField _startHealthField;
        private readonly Label _totalsLabel;

        private LevelEditingSession _session;

        public LevelSummaryBar(VisualElement root)
        {
            _startGoldField = root.Q<IntegerField>("startGoldField");
            _startHealthField = root.Q<IntegerField>("startHealthField");
            _totalsLabel = root.Q<Label>("levelTotalsLabel");

            _startGoldField.RegisterValueChangedCallback(changeEvent => _session?.SetStartGold(changeEvent.newValue));
            _startHealthField.RegisterValueChangedCallback(changeEvent => _session?.SetStartPlayerHealth(changeEvent.newValue));
        }

        public void Bind(LevelEditingSession session) => _session = session;

        public void Refresh()
        {
            _startGoldField.SetValueWithoutNotify(_session.Config.StartGold);
            _startHealthField.SetValueWithoutNotify(_session.Config.StartPlayerHealth);

            string totals = LevelEditorTexts.DescribeLevelTotals(_session.Config, _session.Stats);
            _totalsLabel.text = totals;
            _totalsLabel.tooltip = totals;
        }
    }
}
