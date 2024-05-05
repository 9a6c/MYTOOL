using UnityEngine;
using UObject = UnityEngine.Object;

namespace MYTOOL
{
    public class UnityLogger
    {
        private static bool s_debugLogEnable = true;

        private static bool s_warningLogEnable = true;

        private static bool s_errorLogEnable = true;


        #region >> 日志开关

        /// <summary>
        /// 普通调试日志开关
        /// </summary>
        public static bool DebugEnable => s_debugLogEnable;

        /// <summary>
        /// 警告日志开关
        /// </summary>
        public static bool WarningEnable => s_warningLogEnable;

        /// <summary>
        /// 错误日志开关
        /// </summary>
        public static bool ErrorEnable => s_errorLogEnable;


        /// <summary>
        /// 关闭Debug Log
        /// </summary>
        public static void CloseDebugLog()
        {
            s_debugLogEnable = false;
        }

        /// <summary>
        /// 关闭Warning Log
        /// </summary>
        public static void CloseWarningLog()
        {
            s_warningLogEnable = false;
        }

        /// <summary>
        /// 关闭Error Log
        /// </summary>
        public static void CloseErrorLog()
        {
            s_errorLogEnable = false;
        }

        /// <summary>
        /// 关闭All Log
        /// </summary>
        public static void CloseAllLog()
        {
            s_debugLogEnable = false;
            s_warningLogEnable = false;
            s_errorLogEnable = false;
        }

        #endregion 日志开关


        #region >> 仅编辑器模式生效

        /// <summary>
        /// 编辑器下青蓝色日志
        /// </summary>
        public static void LogCyanForEditor(object message, UObject context = null)
        {
#if UNITY_EDITOR
            LogCyan($"[EDITOR] => {message}", context);
#endif
        }

        /// <summary>
        /// 编辑器下绿色日志
        /// </summary>
        public static void LogGreenForEditor(object message, UObject context = null)
        {
#if UNITY_EDITOR
            LogGreen($"[EDITOR] => {message}", context);
#endif
        }

        /// <summary>
        /// 编辑器下警告日志
        /// </summary>
        public static void LogWarningForEditor(object message, UObject context = null)
        {
#if UNITY_EDITOR
            LogWarning($"[EDITOR] => {message}", context);
#endif
        }

        /// <summary>
        /// 编辑器下错误日志
        /// </summary>
        public static void LogErrorForEditor(object message, UObject context = null)
        {
#if UNITY_EDITOR
            LogError($"[EDITOR] => {message}", context);
#endif
        }

        #endregion 仅编辑器模式生效


        #region >> 所有模式生效

        /// <summary>
        /// 青蓝色日志
        /// </summary>
        public static void LogCyan(object message, UObject context = null)
        {
            if (!s_debugLogEnable) return;

#if UNITY_EDITOR
            Debug.Log(FmtColor("#00ffff", message), context);
#else
            Debug.Log($"[{System.DateTime.Now}] - {message}", context);
#endif
        }

        /// <summary>
        /// 绿色日志
        /// </summary>
        public static void LogGreen(object message, UObject context = null)
        {
            if (!s_debugLogEnable) return;

#if UNITY_EDITOR
            Debug.Log(FmtColor("green", message), context);
#else
            Debug.Log($"[{System.DateTime.Now}] - {message}", context);
#endif
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        public static void LogWarning(object message, UObject context = null)
        {
            if (!s_warningLogEnable) return;

#if UNITY_EDITOR
            Debug.LogWarning(message, context);
#else
            Debug.LogWarning($"[{System.DateTime.Now}] - {message}", context);
#endif
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        public static void LogError(object message, UObject context = null)
        {
            if (!s_errorLogEnable) return;

#if UNITY_EDITOR
            Debug.LogError(message, context);
#else
            Debug.LogError($"[{System.DateTime.Now}] - {message}", context);
#endif
        }

        /// <summary>
        /// 格式化颜色日志
        /// </summary>
        private static object FmtColor(string color, object obj)
        {
            if (obj is string @string)
            {
#if !UNITY_EDITOR
                return obj;
#else
                return FmtColor(color, @string);
#endif
            }
            else
            {
#if !UNITY_EDITOR
                return obj;
#else
                return string.Format("<color={0}>{1}</color>", color, obj);
#endif
            }
        }

        /// <summary>
        /// 格式化颜色日志
        /// </summary>
        private static object FmtColor(string color, string msg)
        {
#if !UNITY_EDITOR
            return msg;
#else
            int p = msg.IndexOf('\n');
            if (p >= 0) p = msg.IndexOf('\n', p + 1);// 可以同时显示两行
            if (p < 0 || p >= msg.Length - 1) return string.Format("<color={0}>{1}</color>", color, msg);
            if (p > 2 && msg[p - 1] == '\r') p--;
            return string.Format("<color={0}>{1}</color>{2}", color, msg.Substring(0, p), msg.Substring(p));
#endif
        }

        #endregion 所有模式生效


        #region >> 解决日志双击溯源问题

#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            string stackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(stackTrace) && stackTrace.Contains("UnityLogger:Log"))
            {
                // 使用正则表达式匹配at的哪个脚本的哪一行
                var matches = System.Text.RegularExpressions.Regex.Match(stackTrace, @"\(at (.+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string pathLine;
                while (matches.Success)
                {
                    pathLine = matches.Groups[1].Value;

                    if (!pathLine.Contains("UnityLogger.cs"))
                    {
                        int splitIndex = pathLine.LastIndexOf(":");
                        // 脚本路径
                        string path = pathLine.Substring(0, splitIndex);
                        // 行号
                        line = System.Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                        fullPath += path;
                        // 跳转到目标代码的特定行
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }
                    matches = matches.NextMatch();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取当前日志窗口选中的日志的堆栈信息
        /// </summary>
        /// <returns></returns>
        static string GetStackTrace()
        {
            // 通过反射获取ConsoleWindow类
            var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            // 获取窗口实例
            var fieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow",
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic);
            var consoleInstance = fieldInfo.GetValue(null);
            if (consoleInstance != null)
            {
                if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
                {
                    // 获取m_ActiveText成员
                    fieldInfo = ConsoleWindowType.GetField("m_ActiveText",
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic);
                    // 获取m_ActiveText的值
                    string activeText = fieldInfo.GetValue(consoleInstance).ToString();
                    return activeText;
                }
            }

            return null;
        }
#endif

        #endregion 解决日志双击溯源问题
    }
}