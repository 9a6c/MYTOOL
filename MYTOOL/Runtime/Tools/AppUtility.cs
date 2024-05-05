using UnityEngine;
using System.Threading;

namespace MYTOOL.Tools
{
    public class AppUtility
    {
        /// <summary>
        /// 重启应用
        /// </summary>
        public static void RestartApp()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

            //记录主线程
            SynchronizationContext sc = SynchronizationContext.Current;
            Thread thread = new(() =>
            {
                Thread.Sleep(1000);

                //传递到主线程调用isPlaying
                sc.Post((object o) =>
                {
                    UnityEditor.EditorApplication.isPlaying = true;
                }, null);
            });

            thread.Start();
#elif UNITY_ANDROID
            restartAndroid();
#elif UNITY_IOS
            //苹果没办法重启,进行强退(未测试)
            Debug.Log("[IOS] 重启应用");
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.Abort);
#else
            throw new System.NotImplementedException();
#endif
        }

#if UNITY_ANDROID
        private static void restartAndroid()
        {
            if (Application.isEditor) return;

            Debug.Log("[Android] 重启应用");

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            const int kIntent_FLAG_ACTIVITY_CLEAR_TASK = 0x00008000;
            const int kIntent_FLAG_ACTIVITY_NEW_TASK = 0x10000000;

            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            var intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

            intent.Call<AndroidJavaObject>("setFlags", kIntent_FLAG_ACTIVITY_NEW_TASK | kIntent_FLAG_ACTIVITY_CLEAR_TASK);
            currentActivity.Call("startActivity", intent);
            currentActivity.Call("finish");
            var process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
#endif
    }
}