using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Characters;
using Assets._Project.Develop.Runtime.Gameplay.Features.Level;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Assets._Project.Develop.Editor
{
    public class LevelWavesEditorWindow : EditorWindow
    {
        private Level _level;
        private SerializedObject _so;
        private SerializedProperty _stagesProp;
        private ReorderableList _stagesList;

        private readonly Dictionary<string, ReorderableList> _wavesLists = new();
        private Vector2 _scroll;

        [MenuItem("Window/Level Waves Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelWavesEditorWindow>("Level Waves");
        }

        private void OnDisable()
        {
            _stagesList = null;
            _wavesLists.Clear();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            _level = (Level)EditorGUILayout.ObjectField("Target Level", _level, typeof(Level), true);
            if (EditorGUI.EndChangeCheck())
                Bind();

            if (_level == null)
            {
                EditorGUILayout.HelpBox("Assign a Level object.", MessageType.Info);
                return;
            }

            if (_so == null || _stagesProp == null)
                Bind();

            if (_stagesList == null)
                BuildStagesList();

            if (_stagesList == null)
                return;

            _so.Update();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _stagesList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            _so.ApplyModifiedProperties();
        }

        private void Bind()
        {
            _so = _level != null ? new SerializedObject(_level) : null;
            _stagesProp = _so != null ? _so.FindProperty("_enemiesWavesStageConfigs") : null;

            _stagesList = null;
            _wavesLists.Clear();
        }

        private void BuildStagesList()
        {
            if (_so == null || _stagesProp == null || !_stagesProp.isArray)
                return;

            _stagesList = new ReorderableList(_so, _stagesProp, true, true, true, true);
            _stagesList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Enemies Wave Stages");
            };

            _stagesList.elementHeightCallback = index =>
            {
                var stage = SafeGetArrayElement(_stagesProp, index);
                if (stage == null)
                    return 24f;

                var waves = stage.FindPropertyRelative("_enemiesWaveConfigs");
                int waveCount = waves != null && waves.isArray ? waves.arraySize : 0;

                float stageHeader = 20f;
                float fieldsRow = 20f;
                float wavesHeader = 18f;
                float waveElementHeight = 86f;
                float wavesFooter = 22f;
                float bottomPadding = 30f;

                return stageHeader
                       + fieldsRow
                       + wavesHeader
                       + Mathf.Max(1, waveCount) * waveElementHeight
                       + wavesFooter
                       + bottomPadding;
            };

            _stagesList.drawElementCallback = DrawStageElement;

            _stagesList.onAddCallback = list =>
            {
                _stagesProp.arraySize++;
                _so.ApplyModifiedProperties();
                _wavesLists.Clear();
            };

            _stagesList.onRemoveCallback = list =>
            {
                if (EditorUtility.DisplayDialog("Remove Stage", "Remove this stage?", "Yes", "No"))
                {
                    _stagesProp.DeleteArrayElementAtIndex(list.index);
                    _so.ApplyModifiedProperties();
                    _wavesLists.Clear();
                }
            };
        }

        private void DrawStageElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var stage = SafeGetArrayElement(_stagesProp, index);
            if (stage == null)
                return;

            rect.y += 2f;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, 18f), $"Stage {index}", EditorStyles.boldLabel);

            float y = rect.y + 22f;

            var stageTime = stage.FindPropertyRelative("<StageTime>k__BackingField");
            var timeToSkip = stage.FindPropertyRelative("<TimeToSkipStage>k__BackingField");

            float half = (rect.width - 6f) * 0.5f;
            if (stageTime != null)
                EditorGUI.PropertyField(new Rect(rect.x, y, half, 18f), stageTime, new GUIContent("Stage Time"));

            if (timeToSkip != null)
                EditorGUI.PropertyField(new Rect(rect.x + half + 6f, y, half, 18f), timeToSkip, new GUIContent("Skip Time"));

            y += 22f;

            var wavesProp = stage.FindPropertyRelative("_enemiesWaveConfigs");
            if (wavesProp == null || !wavesProp.isArray)
            {
                EditorGUI.HelpBox(new Rect(rect.x, y, rect.width, 40f), "_enemiesWaveConfigs not found.", MessageType.Error);
                return;
            }

            var list = GetOrCreateWavesList(wavesProp);
            if (list != null)
            {
                float listHeight = GetWavesListHeight(wavesProp);
                list.DoList(new Rect(rect.x, y, rect.width, listHeight));
            }
        }

        private float GetWavesListHeight(SerializedProperty wavesProp)
        {
            if (wavesProp == null || !wavesProp.isArray)
                return 24f;

            float header = 18f;
            float elements = Mathf.Max(1, wavesProp.arraySize) * 86f;
            float footer = 22f;
            return header + elements + footer;
        }

        private ReorderableList GetOrCreateWavesList(SerializedProperty wavesProp)
        {
            if (wavesProp == null)
                return null;

            string key = wavesProp.propertyPath;

            if (_wavesLists.TryGetValue(key, out var cached))
            {
                if (cached != null && cached.serializedProperty == wavesProp)
                    return cached;
            }

            var list = new ReorderableList(_so, wavesProp, true, true, true, true);
            list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Waves");
            list.elementHeight = 86f;

            list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var wave = wavesProp.GetArrayElementAtIndex(index);
                if (wave == null)
                    return;

                DrawWaveElement(rect, wave);
            };

            list.onAddCallback = l =>
            {
                wavesProp.arraySize++;
                _so.ApplyModifiedProperties();
                Repaint();
            };

            list.onRemoveCallback = l =>
            {
                if (EditorUtility.DisplayDialog("Remove Wave", "Remove this wave?", "Yes", "No"))
                {
                    wavesProp.DeleteArrayElementAtIndex(l.index);
                    _so.ApplyModifiedProperties();
                    Repaint();
                }
            };

            _wavesLists[key] = list;
            return list;
        }

        private void DrawWaveElement(Rect rect, SerializedProperty wave)
        {
            float x = rect.x + 4f;
            float y = rect.y + 2f;

            float leftW = Mathf.Max(240f, rect.width * 0.68f);
            float rightX = x + leftW + 8f;
            float rightW = rect.width - leftW - 8f;

            var characterProp = wave.FindPropertyRelative("<EnemyConfig>k__BackingField");
            var roadProp = wave.FindPropertyRelative("<RoadPath>k__BackingField");
            var startDelay = wave.FindPropertyRelative("<StartDelay>k__BackingField");
            var delayBetween = wave.FindPropertyRelative("<DelayBetweenSpawns>k__BackingField");
            var enemiesCount = wave.FindPropertyRelative("<EnemiesCount>k__BackingField");

            if (characterProp != null)
                EditorGUI.PropertyField(new Rect(x, y, leftW, 18f), characterProp, new GUIContent("Character Config"));

            DrawPrefabPreview(new Rect(rightX, y, rightW, 56f), characterProp?.objectReferenceValue as CharacterConfig);

            y += 22f;

            if (roadProp != null)
                EditorGUI.PropertyField(new Rect(x, y, leftW, 18f), roadProp, new GUIContent("Road Path"));

            y += 20f;

            float w1 = leftW * 0.32f;
            float w2 = leftW * 0.33f;
            float w3 = leftW * 0.30f;

            if (startDelay != null)
                EditorGUI.PropertyField(new Rect(x, y, w1, 18f), startDelay, new GUIContent("Start"));

            if (delayBetween != null)
                EditorGUI.PropertyField(new Rect(x + w1 + 6f, y, w2, 18f), delayBetween, new GUIContent("Delay"));

            if (enemiesCount != null)
                EditorGUI.PropertyField(new Rect(x + w1 + w2 + 12f, y, w3, 18f), enemiesCount, new GUIContent("Count"));
        }

        private void DrawPrefabPreview(Rect rect, CharacterConfig config)
        {
            EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.1f));

            if (config == null || string.IsNullOrWhiteSpace(config.PrefabPath))
            {
                EditorGUI.LabelField(rect, "No preview", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            string resourcesPath = NormalizeResourcesPath(config.PrefabPath);
            if (string.IsNullOrEmpty(resourcesPath))
            {
                EditorGUI.LabelField(rect, "Bad path", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            var prefab = Resources.Load<GameObject>(resourcesPath);
            if (prefab == null)
            {
                EditorGUI.LabelField(rect, "Not found", EditorStyles.centeredGreyMiniLabel);
                EditorGUI.LabelField(
                    new Rect(rect.x + 4f, rect.y + 16f, rect.width - 8f, 16f),
                    resourcesPath,
                    EditorStyles.miniLabel
                );
                return;
            }

            Texture preview = AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab);
            if (preview != null)
            {
                float imgSize = Mathf.Min(rect.height - 4f, 48f);
                GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 2f, imgSize, imgSize), preview, ScaleMode.ScaleToFit);
            }

            EditorGUI.LabelField(
                new Rect(rect.x + 54f, rect.y + 4f, rect.width - 58f, 16f),
                resourcesPath,
                EditorStyles.miniLabel
            );
        }

        private static string NormalizeResourcesPath(string prefabPath)
        {
            if (string.IsNullOrWhiteSpace(prefabPath))
                return string.Empty;

            string p = prefabPath.Replace('\\', '/').Trim();

            const string marker = "/Resources/";
            int idx = p.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
                p = p[(idx + marker.Length)..];

            p = p.TrimStart('/');

            int dot = p.LastIndexOf('.');
            if (dot > 0)
                p = p[..dot];

            return p;
        }

        private SerializedProperty FindWaveByIndex(int waveIndex)
        {
            foreach (var kv in _wavesLists)
            {
                var prop = kv.Value?.serializedProperty;
                if (prop != null && prop.isArray && waveIndex >= 0 && waveIndex < prop.arraySize)
                    return prop.GetArrayElementAtIndex(waveIndex);
            }

            return null;
        }

        private static SerializedProperty SafeGetArrayElement(SerializedProperty arrayProp, int index)
        {
            if (arrayProp == null || !arrayProp.isArray || index < 0 || index >= arrayProp.arraySize)
                return null;

            return arrayProp.GetArrayElementAtIndex(index);
        }
    }
}
