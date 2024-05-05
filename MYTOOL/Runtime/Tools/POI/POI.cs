using UnityEngine;
using System.Collections.Generic;

namespace MYTOOL.POI
{
    /// <summary>
    /// 兴趣点/信息点管理类
    /// </summary>
    public class POI : MonoBehaviour
    {
        //单例
        private static POI instance;
        //主相机 用于世界坐标转屏幕坐标
        private Camera mainCamera;
        //处理器列表
        private List<POIHandler> handlers;

        public static POI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("MYTOOL.POI").AddComponent<POI>();
                    instance.handlers = new List<POIHandler>();
                    Camera mainCamera = Camera.main;
                    if (mainCamera == null)
                    {
                        mainCamera = FindObjectOfType<Camera>();
                    }
                    if (mainCamera == null)
                    {
                        Debug.LogError("未找到任何Camera相机组件");
                    }
                    instance.mainCamera = mainCamera;
                }
                return instance;
            }
        }

        /// <summary>
        /// UI分辨率
        /// </summary>
        public static Vector2 UIResolution { get; private set; } = new Vector2(1920f, 1080f);

        private void Update()
        {
            if (mainCamera != null)
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    handlers[i].Update(mainCamera);
                }
            }
        }

        /// <summary>
        /// 根据标识符进行匹配
        /// </summary>
        /// <param name="flag">标识符</param>
        /// <param name="rectTransform">RectTransform组件</param>
        public static void Match(string flag, RectTransform rectTransform)
        {
            var handler = Instance.handlers.Find(m => m.flag == flag);
            if (handler == null)
            {
                handler = new POIHandler(flag);
                Instance.handlers.Add(handler);
            }
            handler.rectTransform = rectTransform;

            //将其锚点及轴心点都设为左下角
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
        }
        /// <summary>
        /// 根据标识符进行匹配
        /// </summary>
        /// <param name="flag">标识符</param>
        /// <param name="target">三维目标</param>
        public static void Match(string flag, GameObject target)
        {
            var handler = Instance.handlers.Find(m => m.flag == flag);
            if (handler == null)
            {
                handler = new POIHandler(flag);
                Instance.handlers.Add(handler);
            }
            handler.target = target.transform;
        }
        /// <summary>
        /// 根据标识符移除
        /// </summary>
        /// <param name="flag">标识符</param>
        /// <returns>移除成功返回true 否则返回false</returns>
        public static bool Delete(string flag)
        {
            var target = Instance.handlers.Find(m => m.flag == flag);
            if (target != null)
            {
                Instance.handlers.Remove(target);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear()
        {
            Instance.handlers.Clear();
        }
    }
}