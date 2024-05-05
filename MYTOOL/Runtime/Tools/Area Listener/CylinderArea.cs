using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MYTOOL.Area
{
    /// <summary>
    /// 圆柱形区域
    /// </summary>
    public class CylinderArea : AreaBase
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public Vector3 center;
        /// <summary>
        /// 半径
        /// </summary>
        public float radius = 0.5f;
        /// <summary>
        /// 高度
        /// </summary>
        public float height = 1f;

        public override bool IsInArea(Vector3 targetPos)
        {
            if (radius <= 0f || height < 0f) return false;
            Vector3 relCenter = transform.position + center;
            if (targetPos.y > relCenter.y + height * .5f) return false;
            if (targetPos.y < relCenter.y - height * .5f) return false;
            relCenter.y = 0f;
            targetPos.y = 0f;
            float distance = Vector3.Distance(relCenter, targetPos);
            return distance <= radius;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CylinderArea))]
    public class CylinderAreaEditor : Editor
    {
        private CylinderArea _target;

        private void OnEnable()
        {
            _target = target as CylinderArea;
        }

        private void OnSceneGUI()
        {
            bool flag = _target.radius <= 0f || _target.height < 0f;
            if (flag)
            {
                Debug.LogWarning("圆柱形区域的半径或高度不应为负数", _target);
            }
            Vector3 center = _target.center + _target.transform.position;
            Vector3 topCenter = center + Vector3.up * _target.height * .5f;
            Vector3 bottomCenter = center + Vector3.down * _target.height * .5f;
            Vector3 topLeft = topCenter + Vector3.left * _target.radius;
            Vector3 topRight = topCenter + Vector3.right * _target.radius;
            Vector3 topForward = topCenter + Vector3.forward * _target.radius;
            Vector3 topBack = topCenter + Vector3.back * _target.radius;
            Vector3 bottomLeft = bottomCenter + Vector3.left * _target.radius;
            Vector3 bottomRight = bottomCenter + Vector3.right * _target.radius;
            Vector3 bottomForward = bottomCenter + Vector3.forward * _target.radius;
            Vector3 bottomBack = bottomCenter + Vector3.back * _target.radius;

            void Draw()
            {
                Handles.DrawWireArc(topCenter, Vector3.down, Vector3.right, 360, _target.radius);
                Handles.DrawWireArc(bottomCenter, Vector3.up, Vector3.right, 360, _target.radius);
                Handles.DrawLine(topLeft, bottomLeft);
                Handles.DrawLine(topRight, bottomRight);
                Handles.DrawLine(topForward, bottomForward);
                Handles.DrawLine(topBack, bottomBack);
            }

            Handles.zTest = CompareFunction.Less;
            Handles.color = flag ? Color.red : Color.green;
            Draw();
            Handles.zTest = CompareFunction.GreaterEqual;
            Color cacheColor = Handles.color;
            Handles.color = Color.gray;
            Draw();
            Handles.color = cacheColor;
        }
    }
#endif
}