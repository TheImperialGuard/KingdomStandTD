using System;
using System.Collections.Generic;
using Assets._Project.Develop.Editor.LevelEditor.Window;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

namespace Assets._Project.Develop.Editor.LevelEditor.Roads
{
    // Панель «Дороги уровня» в Scene View: что рисовать. Появляется сама и только там, где есть дороги, — в Prefab Mode
    // префаба уровня или в Play Mode.
    [Overlay(typeof(SceneView), ID, DISPLAY_NAME, true, defaultDockZone = DockZone.RightColumn, defaultDockPosition = DockPosition.Bottom)]
    public class RoadsOverlay : Overlay, ITransientOverlay
    {
        private const string ID = "KingdomStandTD.LevelEditor.Roads";
        private const string DISPLAY_NAME = "Дороги уровня";

        private const string ROADS_TEXT = "Дороги";
        private const string ROADS_TOOLTIP =
            "Рисовать дороги уровня: центр со стрелками по ходу врагов, полосы и подписи. Рисунок виден при включённых Gizmos";

        private const string LANES_TEXT = "Полосы";
        private const string LANES_TOOLTIP =
            "Линии полос Л и П — ровно там, где идут враги.\n" + LevelEditorTexts.LANE_EXPLANATION;

        private const string LABELS_TEXT = "Подписи";
        private const string LABELS_TOOLTIP =
            "У спавна — дорога и длины полос, перед ним — буквы полос, у конца дороги — база. Буквы появляются, когда "
            + "полосы на экране не сливаются";

        private const string WAYPOINT_NUMBERS_TEXT = "Номера вейпоинтов";
        private const string WAYPOINT_NUMBERS_TOOLTIP = "Номер вейпоинта в списке RoadPath, с нуля";

        private readonly List<Toggle> _partToggles = new();

        bool ITransientOverlay.visible => StageRoads.Get().Count > 0;

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new();

            _partToggles.Clear();

            root.Add(CreateToggle(ROADS_TEXT, ROADS_TOOLTIP, RoadsSceneSettings.AreRoadsShown, OnRoadsToggled));
            root.Add(CreatePartToggle(LANES_TEXT, LANES_TOOLTIP, RoadsSceneSettings.AreLanesShown,
                value => RoadsSceneSettings.AreLanesShown = value));
            root.Add(CreatePartToggle(LABELS_TEXT, LABELS_TOOLTIP, RoadsSceneSettings.AreLabelsShown,
                value => RoadsSceneSettings.AreLabelsShown = value));
            root.Add(CreatePartToggle(WAYPOINT_NUMBERS_TEXT, WAYPOINT_NUMBERS_TOOLTIP, RoadsSceneSettings.AreWaypointNumbersShown,
                value => RoadsSceneSettings.AreWaypointNumbersShown = value));

            RefreshPartsEnabled();

            return root;
        }

        private Toggle CreatePartToggle(string text, string tooltip, bool value, Action<bool> onChanged)
        {
            Toggle toggle = CreateToggle(text, tooltip, value, onChanged);

            _partToggles.Add(toggle);

            return toggle;
        }

        private void OnRoadsToggled(bool value)
        {
            RoadsSceneSettings.AreRoadsShown = value;
            RefreshPartsEnabled();
        }

        // Части дороги без самих дорог не рисуются — их галочки неактивны.
        private void RefreshPartsEnabled()
        {
            foreach (Toggle toggle in _partToggles)
                toggle.SetEnabled(RoadsSceneSettings.AreRoadsShown);
        }

        private static Toggle CreateToggle(string text, string tooltip, bool value, Action<bool> onChanged)
        {
            Toggle toggle = new()
            {
                text = text,
                tooltip = tooltip,
                value = value,
            };

            toggle.RegisterValueChangedCallback(changeEvent => onChanged(changeEvent.newValue));

            return toggle;
        }
    }
}
