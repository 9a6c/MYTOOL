using UnityEngine;

namespace MYTOOL.Tools
{
    public static class PositionConvert
    {
        /// <summary>
        /// 主摄像机/世界摄像机
        /// </summary>
        private static Camera worldCamera;
        private static Camera WorldCamera
        {
            get
            {
                if (worldCamera == null)
                {
                    worldCamera = Camera.main != null ? Camera.main : Object.FindObjectOfType<Camera>();
                }

                return worldCamera;
            }
        }

        /// <summary>
        /// 世界坐标转换为屏幕坐标
        /// </summary>
        /// <param name="worldPoint">世界坐标</param>
        /// <returns>屏幕坐标</returns>
        public static Vector2 WorldPointToScreenPoint(Vector3 worldPoint)
        {
            return WorldCamera.WorldToScreenPoint(worldPoint);
        }

        /// <summary>
        /// 屏幕坐标转换为世界坐标
        /// </summary>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="planeZ">距离摄像机 Z 平面的距离</param>
        /// <returns>世界坐标</returns>
        public static Vector3 ScreenPointToWorldPoint(Vector2 screenPoint, float planeZ)
        {
            return WorldCamera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, planeZ));
        }

        /// <summary>
        /// UI世界坐标转换为屏幕坐标
        /// </summary>
        /// <param name="worldPoint">UI世界坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <returns>屏幕坐标</returns>
        public static Vector2 UIPointToScreenPoint(Vector3 worldPoint, Camera uiCamera)
        {
            return RectTransformUtility.WorldToScreenPoint(uiCamera, worldPoint);
        }

        /// <summary>
        /// 屏幕坐标转换为 UGUI 坐标
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <returns></returns>
        public static Vector3 ScreenPointToUIPoint(RectTransform rt, Vector2 screenPoint, Camera uiCamera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, screenPoint, uiCamera, out Vector3 globalMousePos);
            return globalMousePos;
        }

        /// <summary>
        /// 屏幕坐标转换为 UGUI RectTransform 的 anchoredPosition
        /// </summary>
        /// <param name="parentRT"></param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <returns></returns>
        public static Vector2 ScreenPointToUILocalPoint(RectTransform parentRT, Vector2 screenPoint, Camera uiCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, screenPoint, uiCamera, out Vector2 localPos);
            return localPos;
        }
    }
}