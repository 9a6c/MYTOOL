using UnityEngine;

namespace MYTOOL.POI
{
    [RequireComponent(typeof(RectTransform))]
    public class POIRTComponent : POIComponent
    {
        protected override void Match()
        {
            POI.Match(flag, GetComponent<RectTransform>());
        }
    }
}