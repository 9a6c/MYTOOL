using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MYTOOL.Pool;

namespace MYTOOL.UI.Hypertext
{
    public abstract class HypertextBase : Text, IPointerClickHandler
    {
        class Span
        {
            public readonly int StartIndex;
            public readonly int Length;
            public readonly Color Color;
            public readonly Action<string> Callback;
            public List<Rect> BoundingBoxes;

            public Span(int startIndex, int length, Color color, Action<string> callback)
            {
                StartIndex = startIndex;
                Length = length;
                Color = color;
                Callback = callback;
                BoundingBoxes = new List<Rect>();
            }
        };

        private readonly List<Span> spans = new List<Span>();

        // TODO: 识别所有不生成顶点的空格字符
        private readonly char[] invisibleChars =
         {
            Space,
            Tab,
            LineFeed
        };

        private static readonly ObjectPool<List<UIVertex>> verticesPool = new ObjectPool<List<UIVertex>>(null, l => l.Clear());

        private const int CharVerts = 6;
        private const char
             Tab = '\t',
             LineFeed = '\n',
             Space = ' ',
             LesserThan = '<',
             GreaterThan = '>';

        private int[] visibleCharIndexMap;

        private Canvas rootCanvas;
        private Canvas RootCanvas => rootCanvas != null ? rootCanvas : (rootCanvas = GetComponentInParent<Canvas>());

        /// <summary>
        /// 为指定的子字符串注册单击事件
        /// </summary>
        /// <param name="startIndex">子字符串的起始字符位置</param>
        /// <param name="length">子字符串长度</param>
        /// <param name="color">要添加到子字符串的颜色</param>
        /// <param name="onClick">单击子字符串时的回调</param>
        protected void OnClick(int startIndex, int length, Color color, Action<string> onClick)
        {
            if (onClick == null)
            {
                throw new ArgumentNullException(nameof(onClick));
            }

            if (startIndex < 0 || startIndex > text.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            if (length < 1 || startIndex + length > text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            spans.Add(new Span(startIndex, length, color, onClick));
        }

        /// <summary>
        /// 删除事件列表
        /// </summary>
        public virtual void RemoveListeners()
        {
            spans.Clear();
        }

        protected abstract void AddListeners();

        private readonly UIVertex[] tempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
            {
                return;
            }

            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;

            var settings = GetGenerationSettings(extents);
            settings.generateOutOfBounds = true;
            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            var verts = cachedTextGenerator.verts;
            float unitsPerPixel = 1 / pixelsPerUnit;
            int vertCount = verts.Count;

            if (vertCount <= 0)
            {
                toFill.Clear();
                return;
            }

            Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();

            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    tempVerts[tempVertsIndex] = verts[i];
                    tempVerts[tempVertsIndex].position *= unitsPerPixel;
                    tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    tempVerts[tempVertsIndex].position.y += roundingOffset.y;

                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(tempVerts);
                    }
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    tempVerts[tempVertsIndex] = verts[i];
                    tempVerts[tempVertsIndex].position *= unitsPerPixel;

                    if (tempVertsIndex == 3)
                    {
                        toFill.AddUIVertexQuad(tempVerts);
                    }
                }
            }

            var vertices = verticesPool.Get();
            toFill.GetUIVertexStream(vertices);

            GenerateVisibleCharIndexMap(vertices.Count < text.Length * CharVerts);

            spans.Clear();
            AddListeners();
            GenerateHrefBoundingBoxes(ref vertices);

            toFill.Clear();
            toFill.AddUIVertexTriangleStream(vertices);
            verticesPool.Release(vertices);

            m_DisableFontTextureRebuiltCallback = false;
        }

        private void GenerateHrefBoundingBoxes(ref List<UIVertex> vertices)
        {
            int verticesCount = vertices.Count;

            for (int i = 0; i < spans.Count; i++)
            {
                var span = spans[i];

                int startIndex = visibleCharIndexMap[span.StartIndex];
                int endIndex = visibleCharIndexMap[span.StartIndex + span.Length - 1];

                for (int textIndex = startIndex; textIndex <= endIndex; textIndex++)
                {
                    int vertexStartIndex = textIndex * CharVerts;
                    if (vertexStartIndex + CharVerts > verticesCount)
                    {
                        break;
                    }

                    Vector2 min = Vector2.one * float.MaxValue;
                    Vector2 max = Vector2.one * float.MinValue;

                    for (int vertexIndex = 0; vertexIndex < CharVerts; vertexIndex++)
                    {
                        var vertex = vertices[vertexStartIndex + vertexIndex];
                        vertex.color = span.Color;
                        vertices[vertexStartIndex + vertexIndex] = vertex;

                        var pos = vertices[vertexStartIndex + vertexIndex].position;

                        if (pos.y < min.y)
                        {
                            min.y = pos.y;
                        }

                        if (pos.x < min.x)
                        {
                            min.x = pos.x;
                        }

                        if (pos.y > max.y)
                        {
                            max.y = pos.y;
                        }

                        if (pos.x > max.x)
                        {
                            max.x = pos.x;
                        }
                    }

                    span.BoundingBoxes.Add(new Rect { min = min, max = max });
                }

                // 将逐个字符的边界框合并为逐行边界框
                span.BoundingBoxes = CalculateLineBoundingBoxes(span.BoundingBoxes);
            }
        }

        private static List<Rect> CalculateLineBoundingBoxes(List<Rect> charBoundingBoxes)
        {
            var lineBoundingBoxes = new List<Rect>();
            var lineStartIndex = 0;

            for (int i = 1; i < charBoundingBoxes.Count; i++)
            {
                if (charBoundingBoxes[i].xMin >= charBoundingBoxes[i - 1].xMin)
                {
                    continue;
                }

                lineBoundingBoxes.Add(CalculateAABB(charBoundingBoxes.GetRange(lineStartIndex, i - lineStartIndex)));
                lineStartIndex = i;
            }

            if (lineStartIndex < charBoundingBoxes.Count)
            {
                lineBoundingBoxes.Add(CalculateAABB(charBoundingBoxes.GetRange(lineStartIndex, charBoundingBoxes.Count - lineStartIndex)));
            }

            return lineBoundingBoxes;
        }

        private static Rect CalculateAABB(IReadOnlyList<Rect> rects)
        {
            Vector2 min = Vector2.one * float.MaxValue;
            Vector2 max = Vector2.one * float.MinValue;

            for (int i = 0; i < rects.Count; i++)
            {
                if (rects[i].xMin < min.x)
                {
                    min.x = rects[i].xMin;
                }

                if (rects[i].yMin < min.y)
                {
                    min.y = rects[i].yMin;
                }

                if (rects[i].xMax > max.x)
                {
                    max.x = rects[i].xMax;
                }

                if (rects[i].yMax > max.y)
                {
                    max.y = rects[i].yMax;
                }
            }

            return new Rect { min = min, max = max };
        }

        private void GenerateVisibleCharIndexMap(bool verticesReduced)
        {
            if (visibleCharIndexMap == null || visibleCharIndexMap.Length < text.Length)
            {
                Array.Resize(ref visibleCharIndexMap, text.Length);
            }

            if (!verticesReduced)
            {
                for (int i = 0; i < visibleCharIndexMap.Length; i++)
                {
                    visibleCharIndexMap[i] = i;
                }
                return;
            }

            int offset = 0;
            bool inTag = false;

            for (int i = 0; i < text.Length; i++)
            {
                var character = text[i];

                if (inTag)
                {
                    offset--;

                    if (character == GreaterThan)
                    {
                        inTag = false;
                    }
                }
                else if (supportRichText && character == LesserThan)
                {
                    offset--;
                    inTag = true;
                }
                else if (invisibleChars.Contains(character))
                {
                    offset--;
                }

                visibleCharIndexMap[i] = Mathf.Max(0, i + offset);
            }
        }

        private Vector3 CalculateLocalPosition(Vector3 position, Camera pressEventCamera)
        {
            if (!RootCanvas)
            {
                return Vector3.zero;
            }

            if (RootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return transform.InverseTransformPoint(position);
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                position,
                pressEventCamera,
                out var localPosition
            );

            return localPosition;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Vector3 localPosition = CalculateLocalPosition(eventData.position, eventData.pressEventCamera);

            for (int s = 0; s < spans.Count; s++)
            {
                for (var b = 0; b < spans[s].BoundingBoxes.Count; b++)
                {
                    if (spans[s].BoundingBoxes[b].Contains(localPosition))
                    {
                        spans[s].Callback(text.Substring(spans[s].StartIndex, spans[s].Length));
                        break;
                    }
                }
            }
        }
    }
}