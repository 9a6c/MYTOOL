using UnityEngine;

namespace MYTOOL
{
    public static class GameObjectExtension
    {
        public static GameObject Activate(this GameObject target)
        {
            target.SetActive(true);
            return target;
        }

        public static GameObject Deactivate(this GameObject target)
        {
            target.SetActive(false);
            return target;
        }

        public static GameObject ActiveInvert(this GameObject target)
        {
            target.SetActive(!target.activeSelf);
            return target;
        }

        /// <summary>
        /// 获取或添加组件
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            if (!target.TryGetComponent(out T component))
            {
                component = target.AddComponent<T>();
            }
            return component;
        }
    }
}