using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MYTOOL.UI
{
    /// <summary>
    /// Text背景自适配工具
    /// </summary>
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class TextBackgroundAdapter : MonoBehaviour
    {
        public enum AdapterType
        {
            Horizontal,
            HorizontalAndVertical
        }

        [SerializeField, Tooltip("Text的背景")] private RectTransform background;
        [SerializeField, Tooltip("适配模式")] private AdapterType adapterType;
        [SerializeField, Tooltip("水平方向上的最大宽度(HorizontalAndVertical模式下起作用)")] private float maxHorizontalWidth = 300f;
        [SerializeField, Tooltip("水平方向上的边界宽度")] private float boarderWidthOnHorizontal = 50f;
        [SerializeField, Tooltip("垂直方向上的最大宽度(HorizontalAndVertical模式下起作用)")] private float boarderWidthOnVertical = 30f;
        [SerializeField, Tooltip("是否自动扩展其父级大小")] private bool forceExpandParent;

        private Text text;
        private string cacheContent;
        private ContentSizeFitter contentSizeFitter;

        private void Awake()
        {
            text = GetComponent<Text>();
            contentSizeFitter = GetComponent<ContentSizeFitter>();
        }

        private void Update()
        {
            if (background == null) return;
            //文本内容发生变更
            if (cacheContent != text.text)
            {
                cacheContent = text.text;
                StartCoroutine(AdapterCoroutine());
            }
        }

        private IEnumerator AdapterCoroutine()
        {
            yield return null;
            if (background == null) yield break;
            background.gameObject.SetActive(true);
            switch (adapterType)
            {
                case AdapterType.Horizontal:
                    contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                    background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth + boarderWidthOnHorizontal);
                    break;
                case AdapterType.HorizontalAndVertical:
                    contentSizeFitter.horizontalFit = text.preferredWidth <= maxHorizontalWidth
                        ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
                    contentSizeFitter.verticalFit = text.preferredWidth <= maxHorizontalWidth
                        ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;
                    text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHorizontalWidth);
                    background.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (text.preferredWidth <= maxHorizontalWidth
                        ? text.preferredWidth : maxHorizontalWidth) + boarderWidthOnHorizontal);
                    background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, text.preferredHeight + boarderWidthOnVertical);
                    break;
            }

            if (forceExpandParent)
            {
                RectTransform parent = background.parent as RectTransform;
                parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, background.rect.width);
                parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, background.rect.height);
            }
        }
    }
}