using UnityEngine;
using UnityEngine.EventSystems;

namespace MYTOOL.UI
{
    [DisallowMultipleComponent]
    public class UIEventTriggerListener : MonoBehaviour,
        IPointerClickHandler,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerUpHandler
    {
        public delegate void UIEventHandle<T>(GameObject target, T eventData) where T : BaseEventData;

        public class UIEvent<T> where T : BaseEventData
        {
            public UIEvent() { }

            public void AddListener(UIEventHandle<T> handle)
            {
                m_UIEventHandle += handle;
            }
            public void SetListener(UIEventHandle<T> handle)
            {
                RemoveAllListeners();
                m_UIEventHandle += handle;
            }
            public void RemoveListener(UIEventHandle<T> handle)
            {
                m_UIEventHandle -= handle;
            }
            public void RemoveAllListeners()
            {
                m_UIEventHandle -= m_UIEventHandle;
                m_UIEventHandle = null;
            }

            public void Invoke(GameObject target, T eventData)
            {
                if (m_UIEventHandle != null)
                    m_UIEventHandle.Invoke(target, eventData);
            }

            private event UIEventHandle<T> m_UIEventHandle = null;
        }

        public UIEvent<PointerEventData> onClick = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDoubleClick = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onPress = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onUp = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onDown = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onEnter = new UIEvent<PointerEventData>();
        public UIEvent<PointerEventData> onExit = new UIEvent<PointerEventData>();

        #region >> 获取 UIEventTriggerListener 组件

        public static UIEventTriggerListener GetListener(UnityEngine.UI.Image target)
        {
            if (target == null) return null;
            return GetListener(target.gameObject);
        }
        public static UIEventTriggerListener GetListener(UnityEngine.UI.Text target)
        {
            if (target == null) return null;
            return GetListener(target.gameObject);
        }
        public static UIEventTriggerListener GetListener(RectTransform target)
        {
            if (target == null) return null;
            return GetListener(target.gameObject);
        }
        public static UIEventTriggerListener GetListener(GameObject target)
        {
            if (target == null) return null;

            UIEventTriggerListener eventTrigger = target.GetComponent<UIEventTriggerListener>();
            if (eventTrigger == null) eventTrigger = target.AddComponent<UIEventTriggerListener>();
            return eventTrigger;
        }

        #endregion 获取 UIEventTriggerListener 组件

        private void Update()
        {
            if (m_IsPointDown)
            {
                if (Time.unscaledTime - m_CurrDonwTime >= PRESS_TIME)
                {
                    m_IsPress = true;
                    m_IsPointDown = false;
                    m_CurrDonwTime = 0f;
                    onPress.Invoke(gameObject, null);
                }
            }

            if (m_ClickCount > 0)
            {
                if (Time.unscaledTime - m_CurrDonwTime >= DOUBLE_CLICK_TIME)
                {
                    if (m_ClickCount < 2)
                    {
                        onClick.Invoke(gameObject, m_OnUpEventData);
                        m_OnUpEventData = null;
                    }
                    m_ClickCount = 0;
                }

                if (m_ClickCount >= 2)
                {
                    onDoubleClick.Invoke(gameObject, m_OnUpEventData);
                    m_OnUpEventData = null;
                    m_ClickCount = 0;
                }
            }
        }

        private void OnDestroy() { RemoveAllListeners(); }

        public void RemoveAllListeners()
        {
            onClick.RemoveAllListeners();
            onDoubleClick.RemoveAllListeners();
            onDown.RemoveAllListeners();
            onEnter.RemoveAllListeners();
            onExit.RemoveAllListeners();
            onUp.RemoveAllListeners();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_IsPointDown = true;
            m_IsPress = false;
            m_CurrDonwTime = Time.unscaledTime;
            onDown.Invoke(gameObject, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_IsPointDown = false;
            m_OnUpEventData = eventData;
            if (!m_IsPress)
            {
                m_ClickCount++;
            }
            onUp.Invoke(gameObject, eventData);
        }

        public void OnPointerClick(PointerEventData eventData) { }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnter.Invoke(gameObject, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onExit.Invoke(gameObject, eventData);
        }

        private const float DOUBLE_CLICK_TIME = 0.2f;
        private const float PRESS_TIME = 0.5f;

        private float m_CurrDonwTime = 0f;
        private bool m_IsPointDown = false;
        private bool m_IsPress = false;
        private int m_ClickCount = 0;
        private PointerEventData m_OnUpEventData = null;
    }
}