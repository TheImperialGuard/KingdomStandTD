#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.Browser
{
    // Размеры игровой области на странице браузера; вне WebGL-сборки — размеры экрана Unity.
    public static class BrowserScreen
    {
        // В WebGL-сборке — высота элемента canvas в CSS-точках, не зависящая от плотности пикселей экрана.
        // В редакторе и остальных сборках — высота экрана в пикселях.
        public static int LogicalHeight
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                return BrowserScreen_GetCanvasClientHeight();
#else
                return Screen.height;
#endif
            }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int BrowserScreen_GetCanvasClientHeight();
#endif
    }
}
