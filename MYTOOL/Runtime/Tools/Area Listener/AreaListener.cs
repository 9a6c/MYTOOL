using UnityEngine;

namespace MYTOOL.Area
{
    /// <summary>
    /// 区域监听器
    /// </summary>
    public class AreaListener
    {
        /// <summary>
        /// 是否进入标识
        /// </summary>
        public bool isEnter;
        /// <summary>
        /// 区域
        /// </summary>
        public AreaBase area;
        /// <summary>
        /// 监听目标
        /// </summary>
        public Transform target;
        /// <summary>
        /// 进入事件
        /// </summary>
        public System.Action enterEvent;
        /// <summary>
        /// 停留事件
        /// </summary>
        public System.Action stayEvent;
        /// <summary>
        /// 退出事件
        /// </summary>
        public System.Action exitEvent;

        public AreaListener(AreaBase area, Transform target)
        {
            this.area = area;
            this.target = target;
        }
    }
}