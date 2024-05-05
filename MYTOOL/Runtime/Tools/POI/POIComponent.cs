using UnityEngine;

namespace MYTOOL.POI
{
    public class POIComponent : MonoBehaviour
    {
        [POIFlag]
        [SerializeField] protected string flag;

        private void Start()
        {
            Match();
        }

        protected virtual void Match()
        {
            POI.Match(flag, gameObject);
        }

        public void Delete()
        {
            POI.Delete(flag);
        }
    }
}