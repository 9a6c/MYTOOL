using UnityEngine;

namespace MYTOOL.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways, DisallowMultipleComponent]
    public abstract class AdapterBase : MonoBehaviour
    {
        public abstract void Adapt();
    }
}