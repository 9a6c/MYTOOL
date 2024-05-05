using UnityEngine;

namespace MYTOOL.Area
{
    /// <summary>
    /// 区域基类
    /// </summary>
    public abstract class AreaBase : MonoBehaviour
    {
        /// <summary>
        /// 判断目标点是否在区域内
        /// </summary>
        public abstract bool IsInArea(Vector3 targetPos);
    }
}