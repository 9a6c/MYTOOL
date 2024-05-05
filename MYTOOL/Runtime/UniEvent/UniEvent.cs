using System.Collections.Generic;

namespace MYTOOL.UniEvent
{
    public static class UniEvent
    {
        private class PostWrapper
        {
            public int PostFrame;
            public int EventID;
            public IEventMessage Message;

            public void OnRelease()
            {
                PostFrame = 0;
                EventID = 0;
                Message = null;
            }
        }

        private static bool _isInitialize = false;
        private static UnityEngine.GameObject _driver = null;
        private static readonly Dictionary<int, LinkedList<System.Action<IEventMessage>>> _listeners = new Dictionary<int, LinkedList<System.Action<IEventMessage>>>(1000);
        private static readonly List<PostWrapper> _postingList = new List<PostWrapper>(1000);

        /// <summary>
        /// 初始化事件系统
        /// </summary>
        public static void Initalize()
        {
            if (_isInitialize)
                throw new System.Exception($"{nameof(UniEvent)} is initialized !");

            if (_isInitialize == false)
            {
                // 创建驱动器
                _isInitialize = true;
                _driver = new UnityEngine.GameObject($"[{nameof(UniEvent)}]");
                _driver.AddComponent<UniEventDriver>();
                UnityEngine.Object.DontDestroyOnLoad(_driver);
                UnityLogger.LogCyanForEditor($"{nameof(UniEvent)} initalize !");
            }
        }

        /// <summary>
        /// 销毁事件系统
        /// </summary>
        public static void Destroy()
        {
            if (_isInitialize)
            {
                ClearAll();

                _isInitialize = false;
                if (_driver != null)
                    UnityEngine.Object.Destroy(_driver);
                UnityLogger.LogCyanForEditor($"{nameof(UniEvent)} destroy all !");
            }
        }

        /// <summary>
        /// 更新事件系统
        /// </summary>
        internal static void Update()
        {
            for (int i = _postingList.Count - 1; i >= 0; i--)
            {
                var wrapper = _postingList[i];
                if (UnityEngine.Time.frameCount > wrapper.PostFrame)
                {
                    SendMessage(wrapper.EventID, wrapper.Message);
                    _postingList.RemoveAt(i);
                    wrapper.OnRelease();
                }
            }
        }

        /// <summary>
        /// 清空所有监听
        /// </summary>
        public static void ClearAll()
        {
            foreach (int eventId in _listeners.Keys)
            {
                _listeners[eventId].Clear();
            }
            _listeners.Clear();
            _postingList.Clear();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof(TEvent);
            int eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }
        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener(System.Type eventType, System.Action<IEventMessage> listener)
        {
            int eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }
        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener(int eventId, System.Action<IEventMessage> listener)
        {
            if (_listeners.ContainsKey(eventId) == false)
                _listeners.Add(eventId, new LinkedList<System.Action<IEventMessage>>());
            if (_listeners[eventId].Contains(listener) == false)
                _listeners[eventId].AddLast(listener);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof(TEvent);
            int eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener(System.Type eventType, System.Action<IEventMessage> listener)
        {
            int eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }
        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener(int eventId, System.Action<IEventMessage> listener)
        {
            if (_listeners.ContainsKey(eventId))
            {
                if (_listeners[eventId].Contains(listener))
                    _listeners[eventId].Remove(listener);
            }
        }

        /// <summary>
        /// 实时广播事件
        /// </summary>
        public static void SendMessage(IEventMessage message)
        {
            int eventId = message.GetType().GetHashCode();
            SendMessage(eventId, message);
        }
        /// <summary>
        /// 实时广播事件
        /// </summary>
        public static void SendMessage(int eventId, IEventMessage message)
        {
            if (_listeners.ContainsKey(eventId) == false)
                return;

            LinkedList<System.Action<IEventMessage>> listeners = _listeners[eventId];
            if (listeners.Count > 0)
            {
                var currentNode = listeners.Last;
                while (currentNode != null)
                {
                    currentNode.Value.Invoke(message);
                    currentNode = currentNode.Previous;
                }
            }
        }
        /// <summary>
        /// 延迟广播事件
        /// </summary>
        public static void PostMessage(IEventMessage message, ushort delayFrame = 0)
        {
            int eventId = message.GetType().GetHashCode();
            PostMessage(eventId, message, delayFrame);
        }
        /// <summary>
        /// 延迟广播事件
        /// </summary>
        public static void PostMessage(int eventId, IEventMessage message, ushort delayFrame = 0)
        {
            var wrapper = new PostWrapper();
            wrapper.PostFrame = UnityEngine.Time.frameCount + delayFrame;
            wrapper.EventID = eventId;
            wrapper.Message = message;
            _postingList.Add(wrapper);
        }
    }
}