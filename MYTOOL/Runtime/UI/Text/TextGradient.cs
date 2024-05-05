using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MYTOOL.UI
{
    [RequireComponent(typeof(Text))]
    public class TextGradient : BaseMeshEffect
    {
        private const int DefaultVertexNumPerFont = 6;  // 每个字符默认的顶点数量

        public Color32 startColor = Color.white;
        public Color32 endColor = Color.black;
        public GradientDirection direction;

        private readonly List<UIVertex> _vertexBuffers = new List<UIVertex>();

        // 给顶点着色
        private static void ModifyVertexColor(IList<UIVertex> vertexList, int index, Color color)
        {
            UIVertex uiVertex = vertexList[index];
            uiVertex.color = color;
            vertexList[index] = uiVertex;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive()) return;

            vh.GetUIVertexStream(_vertexBuffers);

            int count = _vertexBuffers.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i += DefaultVertexNumPerFont)
                {
                    switch (direction)
                    {
                        case GradientDirection.Vertical:
                            ModifyVertexColor(_vertexBuffers, i, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 1, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 2, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 3, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 4, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 5, startColor);
                            break;
                        case GradientDirection.Horizontal:
                            ModifyVertexColor(_vertexBuffers, i, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 1, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 2, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 3, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 4, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 5, startColor);
                            break;
                        case GradientDirection.LeftUpToRightDown:
                            ModifyVertexColor(_vertexBuffers, i, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 1, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 2, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 3, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 4, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 5, startColor);
                            break;
                        case GradientDirection.LeftDownToRightUp:
                            ModifyVertexColor(_vertexBuffers, i, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 1, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 2, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 3, endColor);
                            ModifyVertexColor(_vertexBuffers, i + 4, startColor);
                            ModifyVertexColor(_vertexBuffers, i + 5, endColor);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(_vertexBuffers);
        }
    }

    public enum GradientDirection
    {
        Vertical,
        Horizontal,
        LeftUpToRightDown,
        LeftDownToRightUp
    }
}