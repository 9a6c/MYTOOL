using UnityEngine;
using System.Collections;

namespace MYTOOL.Tools
{
    /// <summary>
    /// Unity MonoBehaviour生命周期事件
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class UnityMonoBehaviourEvent : MonoSingletonTemplate<UnityMonoBehaviourEvent>
    {
        private event System.Action UPDATE;
        private event System.Action LATE_UPDATE;
        private event System.Action FIXED_UPDATE;
        private event System.Action WAIT_FOR_FIXED_UPDATE;
        private event System.Action WAIT_FOR_END_OF_FRAME;

        public void AddUpdate(UpdateMode mode, System.Action action)
        {
            switch (mode)
            {
                case UpdateMode.Update: UPDATE += action; return;
                case UpdateMode.LateUpdate: LATE_UPDATE += action; return;
                case UpdateMode.FixedUpdate: FIXED_UPDATE += action; return;
                case UpdateMode.WaitForFixedUpdate: WAIT_FOR_FIXED_UPDATE += action; return;
                case UpdateMode.WaitForEndOfFrame: WAIT_FOR_END_OF_FRAME += action; return;
            }
        }
        public void RemoveUpdate(UpdateMode mode, System.Action action)
        {
            switch (mode)
            {
                case UpdateMode.Update: UPDATE -= action; return;
                case UpdateMode.LateUpdate: LATE_UPDATE -= action; return;
                case UpdateMode.FixedUpdate: FIXED_UPDATE -= action; return;
                case UpdateMode.WaitForFixedUpdate: WAIT_FOR_FIXED_UPDATE -= action; return;
                case UpdateMode.WaitForEndOfFrame: WAIT_FOR_END_OF_FRAME -= action; return;
            }
        }

        private void Start()
        {
            StartCoroutine(WaitForEndOfFrame());
            StartCoroutine(WaitForFixedUpdate());
        }

        private void Update()
        {
            UPDATE?.Invoke();
        }

        private void LateUpdate()
        {
            LATE_UPDATE?.Invoke();
        }

        private void FixedUpdate()
        {
            FIXED_UPDATE?.Invoke();
        }

        private IEnumerator WaitForEndOfFrame()
        {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            while (true)
            {
                yield return wait;
                WAIT_FOR_END_OF_FRAME?.Invoke();
            }
        }

        private IEnumerator WaitForFixedUpdate()
        {
            var wait = new WaitForFixedUpdate();
            while (true)
            {
                yield return wait;
                WAIT_FOR_FIXED_UPDATE?.Invoke();
            }
        }
    }
}