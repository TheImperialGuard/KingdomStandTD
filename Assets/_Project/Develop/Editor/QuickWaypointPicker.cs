using Assets._Project.Develop.Runtime.Gameplay.Features.LevelNavigation;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor
{
    public class QuickWaypointPicker : EditorWindow
    {
        private RoadPath targetPath;
        private bool picking = false;
        private Vector2 scroll;

        [MenuItem("Window/Quick Waypoint Picker")]
        public static void ShowWindow()
        {
            GetWindow<QuickWaypointPicker>("Waypoint Picker");
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.playModeStateChanged += _ => Repaint();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnGUI()
        {
            GUILayout.Label("Quick Waypoint Picker", EditorStyles.boldLabel);

            targetPath = (RoadPath)EditorGUILayout.ObjectField("Target RoadPath", targetPath, typeof(RoadPath), true);

            if (targetPath == null && Selection.activeGameObject != null)
            {
                var rp = Selection.activeGameObject.GetComponent<RoadPath>();
                if (rp != null) targetPath = rp;
            }

            GUILayout.Space(6);

            GUI.enabled = targetPath != null;
            if (!picking)
            {
                if (GUILayout.Button("Start Picking"))
                {
                    picking = true;
                    EditorApplication.wantsToQuit += OnWantsToQuit;
                    Repaint();
                }
            }
            else
            {
                if (GUILayout.Button("Stop Picking"))
                {
                    picking = false;
                    EditorApplication.wantsToQuit -= OnWantsToQuit;
                    Repaint();
                }
                EditorGUILayout.HelpBox("Pick Waypoint objects in the Hierarchy (single-click). Hold Ctrl to avoid adding duplicates.", MessageType.Info);
            }
            GUI.enabled = true;

            GUILayout.Space(6);
            GUILayout.Label("Current Waypoints:", EditorStyles.label);

            if (targetPath != null)
            {
                var list = targetPath.Waypoints;
                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(120));
                if (list != null)
                {
                    foreach (var w in list)
                    {
                        EditorGUILayout.ObjectField(w, typeof(MonoBehaviour), true);
                    }
                }
                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Clear List"))
                {
                    Undo.RecordObject(targetPath, "Clear Waypoints");
                    var so = new SerializedObject(targetPath);
                    var prop = so.FindProperty("_waypoints");
                    prop.ClearArray();
                    so.ApplyModifiedProperties();
                }
            }
        }

        private bool OnWantsToQuit()
        {
            picking = false;
            return true;
        }

        private void OnSelectionChanged()
        {
            if (!picking) return;
            if (targetPath == null) return;

            var go = Selection.activeGameObject;
            if (go == null) return;

            var wp = go.GetComponent<Waypoint>();
            if (wp == null) return;

            // Добавляем через SerializedObject/SerializedProperty
            var so = new SerializedObject(targetPath);
            var listProp = so.FindProperty("_waypoints");
            // Проверка на дубликат
            for (int i = 0; i < listProp.arraySize; i++)
            {
                var elem = listProp.GetArrayElementAtIndex(i);
                if (elem.objectReferenceValue == wp) return;
            }

            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            var newElem = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
            newElem.objectReferenceValue = wp;
            so.ApplyModifiedProperties();

            // Пометить сцену изменённой
            EditorUtility.SetDirty(targetPath);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(targetPath.gameObject.scene);
        }
    }
}
