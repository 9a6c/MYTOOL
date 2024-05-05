using UnityEngine;

namespace MYTOOL
{
    public static class ComponentExtension
    {
        public static T Activate<T>(this T target) where T : Component
        {
            target.gameObject.SetActive(true);
            return target;
        }

        public static T Deactivate<T>(this T target) where T : Component
        {
            target.gameObject.SetActive(false);
            return target;
        }

        public static T ActiveInvert<T>(this T target) where T : Component
        {
            target.gameObject.SetActive(!target.gameObject.activeSelf);
            return target;
        }

        /// <summary>
        /// 获取或添加组件
        /// </summary>
        public static T GetOrAddComponent<T>(this Component target) where T : Component
        {
            return target.gameObject.GetOrAddComponent<T>();
        }
    }
}