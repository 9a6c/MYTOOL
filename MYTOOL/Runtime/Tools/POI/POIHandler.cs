using UnityEngine;

namespace MYTOOL.POI
{
    /// <summary>
    /// 兴趣点/信息点 处理器
    /// </summary>
    public class POIHandler
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public readonly string flag;
        /// <summary>
        /// 三维位置
        /// </summary>
        public Transform target;
        /// <summary>
        /// 二维位置
        /// </summary>
        public RectTransform rectTransform;

        public POIHandler(string flag)
        {
            this.flag = flag;
        }

        public void Update(Camera mainCamera)
        {
            if (rectTransform != null && target != null)
            {
                //如果RectTransform处于显示状态 计算坐标
                if (rectTransform.gameObject.activeSelf)
                {
                    //世界转屏幕坐标
                    Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position);
                    //计算UI所用分辨率与屏幕大小的比例
                    float wRatio = POI.UIResolution.x / Screen.width;
                    float hRatio = POI.UIResolution.y / Screen.height;
                    //通过比例换算坐标
                    screenPosition.x *= wRatio;
                    screenPosition.y *= hRatio;
                    if(screenPosition.z < 0)
                    {
                        screenPosition.z = -10000f;
                    }
                    //计算后的坐标进行赋值
                    rectTransform.anchoredPosition3D = screenPosition;
                }
            }
        }
    }
}