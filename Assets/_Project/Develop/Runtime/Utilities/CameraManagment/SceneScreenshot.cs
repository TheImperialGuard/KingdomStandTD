using System.IO;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.CameraManagment
{
    public class SceneScreenshot : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private int width = 3840;
        [SerializeField] private int height = 2160;

        [ContextMenu("Take Screenshot")]
        public void TakeScreenshot()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            RenderTexture renderTexture = new RenderTexture(
                width,
                height,
                24,
                RenderTextureFormat.ARGB32
            );

            Texture2D screenshot = new Texture2D(
                width,
                height,
                TextureFormat.RGB24,
                false
            );

            RenderTexture previousTarget = targetCamera.targetTexture;
            RenderTexture previousActive = RenderTexture.active;

            targetCamera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            targetCamera.Render();

            screenshot.ReadPixels(
                new Rect(0, 0, width, height),
                0,
                0
            );

            screenshot.Apply();

            targetCamera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;

            byte[] pngData = screenshot.EncodeToPNG();
            string path = Path.Combine(
                Application.dataPath,
                "..",
                "SceneScreenshot.png"
            );

            File.WriteAllBytes(path, pngData);

            Debug.Log($"Скриншот сохранён: {Path.GetFullPath(path)}");

            DestroyImmediate(screenshot);
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }
    }
}
