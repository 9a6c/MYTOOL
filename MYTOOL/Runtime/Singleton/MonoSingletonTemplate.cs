using UnityEngine;

namespace MYTOOL
{
    /// <summary>
    /// 此单例继承于MonoBehaviour
    /// 绝大多情况下，都不需要使用此单例类型。请使用SingletonTemplate
    /// </summary>
    [DisallowMultipleComponent]
    public class MonoSingletonTemplate<T> : MonoBehaviour where T : MonoSingletonTemplate<T>
    {
        private static T instance;

        protected static bool ApplicationIsQuitting { get; private set; }
        /// <summary>
        /// 是否为全局单例
        /// </summary>
        protected static bool isGolbal = true;

        static MonoSingletonTemplate() { ApplicationIsQuitting = false; }

        private static readonly object locker = new object();

        public static T Instance
        {
            get
            {
                if (ApplicationIsQuitting)
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.LogWarning("[MonoSingletonTemplate] " + typeof(T) + " already destroyed on application quit. Won't create again - returning null.");
                    }

                    return null;
                }

                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            //先在场景中找寻
                            instance = (T)FindObjectOfType(typeof(T));

                            if (FindObjectsOfType(typeof(T)).Length > 1)
                            {
                                if (Debug.isDebugBuild)
                                {
                                    Debug.LogWarning("[MonoSingletonTemplate] " + typeof(T) + " should never be more than 1 in scene!");
                                }
                            }

                            //场景中找不到就创建新物体挂载
                            if (instance == null)
                            {
                                GameObject singletonObj = new GameObject($"[{typeof(T).Name}]");
                                instance = singletonObj.AddComponent<T>();

                                //注：Awake和OnEnable已经执行
                                if (isGolbal && Application.isPlaying)
                                {
                                    DontDestroyOnLoad(singletonObj);
                                }
                            }
                        }
                    }
                }
                return instance;
            }
        }

        public void OnApplicationQuit() { ApplicationIsQuitting = true; }

        public virtual void Init(params object[] args) { }

        public virtual void DeInit() { }
    }
}