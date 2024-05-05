using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MYTOOL.Tools
{
    public static class UniUtility
    {
        public static float GetDeltaTime(TimeMode timeMode)
        {
            return timeMode == TimeMode.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
        }

        public static bool AnyKeyDown(KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
                if (Input.GetKeyDown(key))
                    return true;
            return false;
        }
        public static bool AnyKeyPressed(KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
                if (Input.GetKey(key))
                    return true;
            return false;
        }
        public static bool AnyKeyUp(KeyCode[] keys)
        {
            foreach (KeyCode key in keys)
                if (Input.GetKeyUp(key))
                    return true;
            return false;
        }

        /// <summary>
        /// 是否在UI或OnGUI元素上
        /// </summary>
        public static bool IsCursorOverUserInterface()
        {
            // IsPointerOverGameObject检查左键鼠标(默认)
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            // IsPointerOverGameObject检查触摸
            for (int i = 0; i < Input.touchCount; ++i)
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    return true;

            // OnGUI检查
            return GUIUtility.hotControl != 0;
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void SetCopy(string content)
        {
            UnityEngine.GUIUtility.systemCopyBuffer = content;
        }

        public static string GetCopy()
        {
            return UnityEngine.GUIUtility.systemCopyBuffer;
        }
    }
}