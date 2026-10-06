using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Playtest
{
    // Копия уровня для прогона плейтеста. Instantiate берёт данные из памяти, поэтому несохранённые правки окна попадают
    // в игру. В копии меняется стартовое золото и убираются стейджи до стартового — сам ассет не трогается.
    public static class PlaytestLevelCopy
    {
        public static LevelConfig Create(PlaytestLaunch launch)
        {
            LevelConfig copy = Object.Instantiate(launch.Level);
            copy.name = launch.Level.name;

            using (SerializedObject serializedCopy = new SerializedObject(copy))
            {
                if (launch.StartGold.HasValue)
                    LevelConfigFields.FindRequired(serializedCopy, LevelConfigFields.StartGold).intValue = launch.StartGold.Value;

                RemoveStagesBefore(serializedCopy, launch.StartStageIndex);

                serializedCopy.ApplyModifiedPropertiesWithoutUndo();
            }

            return copy;
        }

        // Последний стейдж остаётся всегда: стейджи могли удалить в окне прямо во время игры, перед «Заново».
        private static void RemoveStagesBefore(SerializedObject serializedCopy, int startStageIndex)
        {
            SerializedProperty stages = LevelConfigFields.FindRequired(serializedCopy, LevelConfigFields.STAGES);
            int removedCount = Mathf.Min(startStageIndex, stages.arraySize - 1);

            for (int i = 0; i < removedCount; i++)
                stages.DeleteArrayElementAtIndex(0);
        }
    }
}
