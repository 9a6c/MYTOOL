using System;
using System.Collections.Generic;

namespace MYTOOL.Event
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : SingletonTemplate<EventManager>
    {
        private readonly Dictionary<string, EventHandler> handlerMap = new Dictionary<string, EventHandler>(1000);


        #region >> 字符串类型事件

        /// <summary>
        /// 添加事件的监听者
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理函数</param>
        public void AddListener(string eventName, EventHandler handler)
        {
            if (handler == null) return;

            if (handlerMap.ContainsKey(eventName))
                handlerMap[eventName] += handler;
            else
                handlerMap.Add(eventName, handler);
        }
        /// <summary>
        /// 移除事件的监听者
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件处理函数</param>
        public void RemoveListener(string eventName, EventHandler handler)
        {
            if (handler == null) return;

            if (handlerMap.ContainsKey(eventName))
                handlerMap[eventName] -= handler;
        }
        /// <summary>
        /// 触发事件(无参数)
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">触发源</param>
        public void TriggerEvent(string eventName, object sender)
        {
            if (handlerMap.ContainsKey(eventName) && handlerMap[eventName] != null)
            {
                handlerMap[eventName].Invoke(sender, EventArgs.Empty);
            }
        }
        /// <summary>
        /// 触发事件(有参数)
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">触发源</param>
        /// <param name="args">事件参数</param>
        public void TriggerEvent(string eventName, object sender, EventArgs args)
        {
            if (handlerMap.ContainsKey(eventName) && handlerMap[eventName] != null)
            {
                handlerMap[eventName].Invoke(sender, args);
            }
        }
        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public bool Clear(string eventName)
        {
            return handlerMap.Remove(eventName);
        }

        #endregion


        #region >> 整数类型事件

        /// <summary>
        /// 添加事件的监听者
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="handler">事件处理函数</param>
        public void AddListener(int eventId, EventHandler handler)
        {
            AddListener(eventId.ToString(), handler);
        }
        /// <summary>
        /// 移除事件的监听者
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="handler">事件处理函数</param>
        public void RemoveListener(int eventId, EventHandler handler)
        {
            RemoveListener(eventId.ToString(), handler);
        }
        /// <summary>
        /// 触发事件(无参数)
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="sender">触发源</param>
        public void TriggerEvent(int eventId, object sender)
        {
            TriggerEvent(eventId.ToString(), sender);
        }
        /// <summary>
        /// 触发事件(有参数)
        /// </summary>
        /// <param name="eventId">事件Id</param>
        /// <param name="sender">触发源</param>
        /// <param name="args">事件参数</param>
        public void TriggerEvent(int eventId, object sender, EventArgs args)
        {
            TriggerEvent(eventId.ToString(), sender, args);
        }
        /// <summary>
        /// 清空指定事件
        /// </summary>
        /// <param name="eventId">事件Id</param>
        public bool Clear(int eventId)
        {
            return handlerMap.Remove(eventId.ToString());
        }

        #endregion


        /// <summary>
        /// 清空所有事件
        /// </summary>
        public void ClearAll()
        {
            handlerMap.Clear();
        }


        #region >> 静态方法触发单例中事件

        /// <summary>
        /// 触发事件(无参数)(单例中事件)
        /// </summary>
        public static void TriggerEvent(string eventName, UnityEngine.Object sender)
        {
            Instance.TriggerEvent(eventName, sender);
        }
        /// <summary>
        /// 触发事件(有参数)(单例中事件)
        /// </summary>
        public static void TriggerEvent(string eventName, UnityEngine.Object sender, EventArgs args)
        {
            Instance.TriggerEvent(eventName, sender, args);
        }

        /// <summary>
        /// 触发事件(无参数)(单例中事件)
        /// </summary>
        public static void TriggerEvent(int eventId, UnityEngine.Object sender)
        {
            Instance.TriggerEvent(eventId, sender);
        }
        /// <summary>
        /// 触发事件(有参数)(单例中事件)
        /// </summary>
        public static void TriggerEvent(int eventId, UnityEngine.Object sender, EventArgs args)
        {
            Instance.TriggerEvent(eventId, sender, args);
        }

        #endregion 静态方法触发单例中事件
    }


    /// <summary>
    /// 便于触发事件的扩展类
    /// </summary>
    public static class EventTriggerExtension
    {
        /// <summary>
        /// 触发事件(无参数)(单例中事件)
        /// </summary>
        /// <param name="sender">触发源</param>
        /// <param name="eventName">事件名称</param>
        public static void TriggerEvent(this object sender, string eventName)
        {
            EventManager.Instance.TriggerEvent(eventName, sender);
        }
        /// <summary>
        /// 触发事件(有参数)(单例中事件)
        /// </summary>
        /// <param name="sender">触发源</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="args">事件参数</param>
        public static void TriggerEvent(this object sender, string eventName, EventArgs args)
        {
            EventManager.Instance.TriggerEvent(eventName, sender, args);
        }

        /// <summary>
        /// 触发事件(无参数)(单例中事件)
        /// </summary>
        /// <param name="sender">触发源</param>
        /// <param name="eventId">事件Id</param>
        public static void TriggerEvent(this object sender, int eventId)
        {
            EventManager.Instance.TriggerEvent(eventId, sender);
        }
        /// <summary>
        /// 触发事件(有参数)(单例中事件)
        /// </summary>
        /// <param name="sender">触发源</param>
        /// <param name="eventId">事件Id</param>
        /// <param name="args">事件参数</param>
        public static void TriggerEvent(this object sender, int eventId, EventArgs args)
        {
            EventManager.Instance.TriggerEvent(eventId, sender, args);
        }
    }
}