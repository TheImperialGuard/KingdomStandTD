using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets._Project.Develop.Editor.LevelEditor.Window
{
    // Макет и стили окон и инспекторов редактора уровней лежат рядом с их скриптами, поэтому путь к ним не зависит
    // от того, где в проекте находится папка редактора.
    public static class LevelEditorWindowFiles
    {
        public static T LoadNextToScript<T>(ScriptableObject owner, string fileName) where T : Object
        {
            MonoScript script = MonoScript.FromScriptableObject(owner);
            string scriptFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
            string filePath = Path.Combine(scriptFolder, fileName).Replace('\\', '/');

            return AssetDatabase.LoadAssetAtPath<T>(filePath);
        }
    }
}
