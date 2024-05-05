using UnityEngine;
using System.Collections;

namespace MYTOOL.Area
{
    public static class AreaExtension
    {
        public static AreaListener Listen<T>(this T self, Transform target) where T : AreaBase
        {
            AreaListener listener = new AreaListener(self, target);
            self.StartCoroutine(ExecuteCoroutine(listener));
            return listener;
        }
        public static AreaListener OnEnter(this AreaListener self, System.Action callback)
        {
            self.enterEvent = callback;
            return self;
        }
        public static AreaListener OnStay(this AreaListener self, System.Action callback)
        {
            self.stayEvent = callback;
            return self;
        }
        public static AreaListener OnExit(this AreaListener self, System.Action callback)
        {
            self.exitEvent = callback;
            return self;
        }
        private static IEnumerator ExecuteCoroutine(AreaListener listener)
        {
            yield return null;

            while (true)
            {
                if (listener.target == null) yield break;

                bool flag = listener.area.IsInArea(listener.target.position);
                if (flag)
                {
                    if (!listener.isEnter)
                    {
                        listener.isEnter = true;
                        listener.enterEvent?.Invoke();
                        yield return null;
                    }
                    listener.stayEvent?.Invoke();
                }
                else
                {
                    if (listener.isEnter)
                    {
                        listener.isEnter = false;
                        listener.exitEvent?.Invoke();
                    }
                }
                yield return null;
            }
        }
    }
}