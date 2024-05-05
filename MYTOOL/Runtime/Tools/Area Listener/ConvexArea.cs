using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MYTOOL.Area
{
    /// <summary>
    /// 凸边形区域
    /// </summary>
    public class ConvexArea : AreaBase
    {
        /// <summary>
        /// 顶点位置数组
        /// </summary>
        public Transform[] vertexPoints = new Transform[3];
        /// <summary>
        /// 高度
        /// </summary>
        public float height = 1f;

        public override bool IsInArea(Vector3 targetPos)
        {
            if (targetPos.y > height * .5f || targetPos.y < -height * .5f) return false;
            Vector3 comparePoint = (vertexPoints[0].position + vertexPoints[1].position) * 0.5f;
            comparePoint += (comparePoint - targetPos).normalized * 10000;
            int count = 0;
            for (int i = 0; i < vertexPoints.Length; i++)
            {
                var currVertex = vertexPoints[i % vertexPoints.Length];
                var nextVertex = vertexPoints[(i + 1) % vertexPoints.Length];
                var crossA = Mathf.Sign(Vector3.Cross(comparePoint - targetPos, currVertex.position - targetPos).y);
                var crossB = Mathf.Sign(Vector3.Cross(comparePoint - targetPos, nextVertex.position - targetPos).y);
                if (Mathf.Approximately(crossA, crossB)) continue;
                var crossC = Mathf.Sign(Vector3.Cross(nextVertex.position - currVertex.position, targetPos - currVertex.position).y);
                var crossD = Mathf.Sign(Vector3.Cross(nextVertex.position - currVertex.position, comparePoint - currVertex.position).y);
                if (Mathf.Approximately(crossC, crossD)) continue;
                count++;
            }
            return count % 2 == 1;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ConvexArea))]
    public class ConvexAreaEditor : Editor
    {
        private ConvexArea _target;

        private void OnEnable()
        {
            _target = target as ConvexArea;
        }

        private void OnSceneGUI()
        {
            bool flag = _target.vertexPoints.Length < 3;
            if (flag)
            {
                Debug.LogWarning("凸边形区域的顶点数量不应小于3", _target);
                return;
            }
            flag = _target.height < 0f;
            if (flag)
            {
                Debug.LogWarning("凸边形区域的高度不应为负数", _target);
            }

            var cacheMatrix = Handles.matrix;
            Handles.matrix = _target.transform.localToWorldMatrix;
            void Draw()
            {
                for (int i = 0; i < _target.vertexPoints.Length; i++)
                {
                    var currVertex = _target.vertexPoints[i];
                    var nextVertex = _target.vertexPoints[(i + 1) % _target.vertexPoints.Length];
                    if (currVertex == null) continue;
                    if (nextVertex == null) continue;

                    Vector3 pos = Handles.PositionHandle(_target.vertexPoints[i].localPosition, Quaternion.identity);
                    pos.y = 0f;
                    _target.vertexPoints[i].localPosition = pos;

                    var minA = _target.transform.worldToLocalMatrix.MultiplyPoint3x4(currVertex.position);
                    var minB = _target.transform.worldToLocalMatrix.MultiplyPoint3x4(nextVertex.position);
                    var maxA = _target.transform.worldToLocalMatrix.MultiplyPoint3x4(currVertex.position);
                    var maxB = _target.transform.worldToLocalMatrix.MultiplyPoint3x4(nextVertex.position);

                    minA.y = -_target.height * .5f;
                    minB.y = -_target.height * .5f;
                    maxA.y = _target.height * .5f;
                    maxB.y = _target.height * .5f;

                    Handles.DrawAAPolyLine(minA, minB);
                    Handles.DrawAAPolyLine(maxA, maxB);
                    Handles.DrawAAPolyLine(minA, maxA);
                    Handles.DrawAAPolyLine(minB, maxB);

                    Handles.Label(currVertex.localPosition, string.Format("Vertex{0}", i));
                }
            }
            Handles.zTest = CompareFunction.Less;
            Handles.color = flag ? Color.red : Color.green;
            Draw();
            Handles.zTest = CompareFunction.GreaterEqual;
            var cacheColor = Handles.color;
            Handles.color = Color.gray;
            Draw();
            Handles.color = cacheColor;
            Handles.matrix = cacheMatrix;
        }
    }
#endif
}