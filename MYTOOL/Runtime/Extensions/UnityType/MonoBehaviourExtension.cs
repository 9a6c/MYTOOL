using System;
using System.Collections;
using UnityEngine;
using MYTOOL.Tools;

namespace MYTOOL
{
    public static class MonoBehaviourExtension
    {
        /// <summary>
        /// 延迟调用(时间秒)
        /// </summary>
        public static Coroutine Delay(this MonoBehaviour monoBehaviour, float delayTime, Action action, TimeMode timeMode = TimeMode.Normal)
        {
            if (!monoBehaviour.gameObject.activeInHierarchy)
            {
#if UNITY_EDITOR
                //无法执行延迟调用
                Debug.LogWarning($"{monoBehaviour.name} activeInHierarchy is false", monoBehaviour);
#endif
                return null;
            }

            return monoBehaviour.StartCoroutine(DelayCoroutine(delayTime, action, timeMode));
        }

        internal static IEnumerator DelayCoroutine(float delayTime, Action action, TimeMode timeMode)
        {
            while (delayTime > 0)
            {
                delayTime -= UniUtility.GetDeltaTime(timeMode);
                yield return null;
            }

            action?.Invoke();
        }
    }
}