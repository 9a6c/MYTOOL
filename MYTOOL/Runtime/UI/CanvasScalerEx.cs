using UnityEngine;

namespace MYTOOL.UI
{
    /// <summary>
    /// UI画布适配
    /// </summary>
    [DisallowMultipleComponent]
    public class CanvasScalerEx : UnityEngine.UI.CanvasScaler
    {
        protected override void HandleScaleWithScreenSize()
        {
            if (Screen.width / m_ReferenceResolution.x < Screen.height / m_ReferenceResolution.y)
                matchWidthOrHeight = 0f;
            else
                matchWidthOrHeight = 1f;
            base.HandleScaleWithScreenSize();
        }
    }
}