using System;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels.Stages;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Карточка группы шаблона волны: превью врага, выбор врага и кнопки перестановки и удаления в верхней строке,
    // количество и короткая сводка статов — в нижней.
    public class WaveGroupRow : VisualElement
    {
        private const string ROOT_CLASS = "group-row";
        private const string ICON_CLASS = "group-row__icon";
        private const string BODY_CLASS = "group-row__body";
        private const string HEADER_CLASS = "group-row__header";
        private const string FOOTER_CLASS = "group-row__footer";
        private const string ENEMY_CLASS = "group-row__enemy";
        private const string COUNT_CLASS = "group-row__count";
        private const string STATS_CLASS = "group-row__stats";
        private const string BUTTON_CLASS = "group-row__button";

        // Превью префаба строится асинхронно: пока его нет, проверяем снова.
        private const long PREVIEW_RETRY_INTERVAL_MS = 200;
        private const int PREVIEW_MAX_ATTEMPTS = 25;

        public event Action<int, CharacterConfig> EnemyChanged;
        public event Action<int, int> CountChanged;
        public event Action<int> MoveUpClicked;
        public event Action<int> MoveDownClicked;
        public event Action<int> RemoveClicked;

        private readonly EnemyCatalog _enemyCatalog;
        private readonly Image _icon;
        private readonly PopupField<CharacterConfig> _enemyField;
        private readonly IntegerField _countField;
        private readonly Label _statsLabel;
        private readonly Button _moveUpButton;
        private readonly Button _moveDownButton;

        private int _groupIndex;
        private IVisualElementScheduledItem _previewLoading;

        public WaveGroupRow(EnemyCatalog enemyCatalog)
        {
            _enemyCatalog = enemyCatalog;

            AddToClassList(ROOT_CLASS);

            _icon = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
            };

            _icon.AddToClassList(ICON_CLASS);

            _enemyField = new PopupField<CharacterConfig>(
                _enemyCatalog.Choices,
                0,
                EnemyCatalog.GetChoiceName,
                EnemyCatalog.GetChoiceName);

            _enemyField.AddToClassList(ENEMY_CLASS);
            _enemyField.RegisterValueChangedCallback(changeEvent => EnemyChanged?.Invoke(_groupIndex, changeEvent.newValue));

            _countField = new IntegerField("Количество")
            {
                isDelayed = true,
                tooltip = "Сколько врагов этой группы в одном проходе шаблона",
            };

            _countField.AddToClassList(COUNT_CLASS);
            _countField.RegisterValueChangedCallback(changeEvent => CountChanged?.Invoke(_groupIndex, changeEvent.newValue));

            _statsLabel = new Label();
            _statsLabel.AddToClassList(STATS_CLASS);

            _moveUpButton = CreateButton("↑", "Поднять группу", () => MoveUpClicked?.Invoke(_groupIndex));
            _moveDownButton = CreateButton("↓", "Опустить группу", () => MoveDownClicked?.Invoke(_groupIndex));
            Button removeButton = CreateButton("✕", "Удалить группу", () => RemoveClicked?.Invoke(_groupIndex));

            VisualElement header = CreateLine(HEADER_CLASS);
            header.Add(_enemyField);
            header.Add(_moveUpButton);
            header.Add(_moveDownButton);
            header.Add(removeButton);

            VisualElement footer = CreateLine(FOOTER_CLASS);
            footer.Add(_countField);
            footer.Add(_statsLabel);

            VisualElement body = new();
            body.AddToClassList(BODY_CLASS);
            body.Add(header);
            body.Add(footer);

            Add(_icon);
            Add(body);
        }

        public void Bind(int groupIndex, WaveEnemyGroup group, int groupsCount)
        {
            _groupIndex = groupIndex;

            CharacterConfig enemy = group.EnemyConfig != null ? group.EnemyConfig : null;
            string statsTooltip = EnemyCatalog.DescribeStats(enemy);

            _enemyCatalog.EnsureChoice(enemy);

            _enemyField.SetValueWithoutNotify(enemy);
            _enemyField.tooltip = statsTooltip;
            _icon.tooltip = statsTooltip;
            _statsLabel.text = EnemyCatalog.DescribeShortStats(enemy);
            _statsLabel.tooltip = statsTooltip;
            _countField.SetValueWithoutNotify(group.Count);

            _moveUpButton.SetEnabled(groupIndex > 0);
            _moveDownButton.SetEnabled(groupIndex < groupsCount - 1);

            ShowPreview(enemy);
        }

        private static VisualElement CreateLine(string lineClass)
        {
            VisualElement line = new();
            line.AddToClassList(lineClass);

            return line;
        }

        private Button CreateButton(string text, string buttonTooltip, Action clicked)
        {
            Button button = new(clicked)
            {
                text = text,
                tooltip = buttonTooltip,
            };

            button.AddToClassList(BUTTON_CLASS);

            return button;
        }

        private void ShowPreview(CharacterConfig enemy)
        {
            _previewLoading?.Pause();
            _icon.image = EnemyCatalog.GetPreview(enemy);

            if (_icon.image != null || enemy == null)
                return;

            int attempts = 0;

            _previewLoading = schedule
                .Execute(() =>
                {
                    attempts++;
                    _icon.image = EnemyCatalog.GetPreview(enemy);
                })
                .Every(PREVIEW_RETRY_INTERVAL_MS)
                .Until(() => _icon.image != null || attempts >= PREVIEW_MAX_ATTEMPTS);
        }
    }
}
