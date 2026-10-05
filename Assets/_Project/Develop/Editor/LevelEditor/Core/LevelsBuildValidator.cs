using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Перед сборкой проверяет уровни кампании: ошибки останавливают сборку, предупреждения пишутся в консоль.
    public class LevelsBuildValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            LevelsListConfig campaign = LevelEditorAssets.FindLevelsList();

            if (campaign == null || campaign.Levels == null || campaign.Levels.Count == 0)
                throw new BuildFailedException("Проверка уровней: не найден LevelsListConfig или в нём нет уровней");

            List<string> errors = new();
            List<string> warnings = new();

            for (int i = 0; i < campaign.Levels.Count; i++)
                CollectIssues(campaign.Levels[i], i, errors, warnings);

            if (warnings.Count > 0)
            {
                string warningsText = string.Join(Environment.NewLine, warnings);
                Debug.LogWarning($"Проверка уровней перед сборкой, предупреждения:{Environment.NewLine}{warningsText}");
            }

            if (errors.Count > 0)
            {
                string errorsText = string.Join(Environment.NewLine, errors);
                throw new BuildFailedException($"Проверка уровней перед сборкой, ошибки:{Environment.NewLine}{errorsText}");
            }
        }

        private static void CollectIssues(LevelConfig level, int levelIndex, List<string> errors, List<string> warnings)
        {
            if (level == null)
            {
                int levelNumber = levelIndex + 1;
                errors.Add($"Уровень №{levelNumber} в LevelsListConfig не задан");
                return;
            }

            LevelPrefabInfo prefab = LevelPrefabReader.Read(level);

            foreach (LevelIssue issue in LevelValidator.Validate(level, prefab))
            {
                string issueText = issue.ToString();
                string line = $"{level.name}: {issueText}";

                if (issue.IsError)
                    errors.Add(line);
                else
                    warnings.Add(line);
            }
        }
    }
}
