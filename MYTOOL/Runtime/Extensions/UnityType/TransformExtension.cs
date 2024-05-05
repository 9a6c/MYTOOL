using UnityEngine;

namespace MYTOOL
{
    public static class TransformExtension
    {
        public static Transform ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return transform;
        }

        public static Transform SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            transform.localPosition = pos;
            return transform;
        }

        public static Transform SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.y = y;
            transform.localPosition = pos;
            return transform;
        }

        public static Transform SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.z = z;
            transform.localPosition = pos;
            return transform;
        }

        public static Transform SetLocalPositionXY(this Transform transform, float x, float y)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            pos.y = y;
            transform.localPosition = pos;
            return transform;
        }

        public static Transform SetLocalPositionXYZ(this Transform transform, float x, float y, float z)
        {
            Vector3 pos = transform.localPosition;
            pos.x = x;
            pos.y = y;
            pos.z = z;
            transform.localPosition = pos;
            return transform;
        }
    }
}