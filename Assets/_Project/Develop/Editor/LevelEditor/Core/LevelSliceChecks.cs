using System.Collections.Generic;

namespace Assets._Project.Develop.Editor.LevelEditor
{
    // Проверка данных среза перед применением: пустые элементы списков и враги, которых нельзя однозначно
    // найти по имени. Адреса в сообщениях — позиции внутри проверяемых данных, а не в уровне.
    public static class LevelSliceChecks
    {
        public static List<string> CheckStages(IReadOnlyList<StageSlice> stages, LevelSliceEnemies enemies)
        {
            List<string> errors = new();

            for (int i = 0; i < stages.Count; i++)
            {
                if (stages[i] == null)
                    AddError(errors, "пустой элемент списка стейджей", i);
                else
                    CheckWaves(stages[i].Waves, enemies, i, errors);
            }

            return errors;
        }

        public static List<string> CheckWaves(IReadOnlyList<WaveSlice> waves, LevelSliceEnemies enemies)
        {
            List<string> errors = new();
            CheckWaves(waves, enemies, LevelAddress.NO_INDEX, errors);

            return errors;
        }

        private static void CheckWaves(IReadOnlyList<WaveSlice> waves, LevelSliceEnemies enemies, int stageIndex, List<string> errors)
        {
            for (int i = 0; i < waves.Count; i++)
            {
                WaveSlice wave = waves[i];

                if (wave == null)
                {
                    AddError(errors, "пустой элемент списка волн", stageIndex, i);
                    continue;
                }

                for (int j = 0; j < wave.Groups.Count; j++)
                {
                    string problem = FindGroupProblem(wave.Groups[j], enemies);

                    if (problem != null)
                        AddError(errors, problem, stageIndex, i, j);
                }
            }
        }

        private static string FindGroupProblem(EnemyGroupSlice group, LevelSliceEnemies enemies)
        {
            if (group == null)
                return "пустой элемент списка групп";

            if (string.IsNullOrEmpty(group.Enemy))
                return "не указан враг";

            return enemies.FindProblem(group.Enemy);
        }

        private static void AddError(
            List<string> errors,
            string message,
            int stageIndex,
            int waveIndex = LevelAddress.NO_INDEX,
            int groupIndex = LevelAddress.NO_INDEX)
        {
            string address = LevelAddress.Format(stageIndex, waveIndex, groupIndex);
            errors.Add($"{address}: {message}");
        }
    }
}
