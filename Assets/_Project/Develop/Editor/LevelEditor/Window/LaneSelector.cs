using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Переключатель полосы «Л | Ц | П». Стороны — по ходу врагов, как в данных и в игре.
    public class LaneSelector : VisualElement
    {
        private const string ROOT_CLASS = "lane-selector";
        private const string BUTTON_CLASS = "lane-selector__button";
        private const string SELECTED_BUTTON_CLASS = "lane-selector__button--selected";

        public event Action<PathLane> ValueChanged;

        private readonly Dictionary<PathLane, Button> _buttons = new();

        public LaneSelector()
        {
            AddToClassList(ROOT_CLASS);
            tooltip = LevelEditorTexts.LANE_EXPLANATION;

            AddButton(PathLane.Left);
            AddButton(PathLane.Center);
            AddButton(PathLane.Right);
        }

        public PathLane Value { get; private set; }

        public void SetValueWithoutNotify(PathLane lane)
        {
            Value = lane;

            foreach (KeyValuePair<PathLane, Button> laneButton in _buttons)
                laneButton.Value.EnableInClassList(SELECTED_BUTTON_CLASS, laneButton.Key == lane);
        }

        private void AddButton(PathLane lane)
        {
            Button button = new(() => OnButtonClicked(lane))
            {
                text = LevelEditorTexts.GetLaneLetter(lane),
            };

            button.AddToClassList(BUTTON_CLASS);

            _buttons.Add(lane, button);
            Add(button);
        }

        private void OnButtonClicked(PathLane lane)
        {
            if (lane == Value)
                return;

            SetValueWithoutNotify(lane);
            ValueChanged?.Invoke(lane);
        }
    }
}
