using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MYTOOL.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIBase : MonoBehaviour
    {
        //通过里式转换原则 来存储所有的控件
        private readonly Dictionary<string, List<UIBehaviour>> controlDic = Pool.Pool<Dictionary<string, List<UIBehaviour>>>.Get();
        private bool dirty = true;

        public bool IsOpened => gameObject.activeSelf;

        #region >> Unity 生命周期函数

        protected virtual void Awake()
        {
            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<InputField>();

            OnAwake();
        }

        protected virtual void Start()
        {
            OnStart();
        }

        protected virtual void Update()
        {
            if (dirty)
            {
                dirty = false;
                OnDirty();
            }

            OnUpdate();
        }

        protected virtual void OnDestroy()
        {
            OnUnload();
            controlDic.Clear();
            Pool.Pool<Dictionary<string, List<UIBehaviour>>>.Release(controlDic);
        }

        #endregion Unity 生命周期函数


        #region >> 生命周期函数(需子类重写)

        /// <summary>
        /// Awake生命周期
        /// </summary>
        protected virtual void OnAwake() { }
        /// <summary>
        /// Start生命周期
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// Update生命周期
        /// </summary>
        protected virtual void OnUpdate() { }
        /// <summary>
        /// Update生命周期中执行OnDirty
        /// </summary>
        protected virtual void OnDirty() { }
        /// <summary>
        /// OnDestroy生命周期
        /// </summary>
        protected virtual void OnUnload() { }

        protected virtual void OnOpen(object data) { }

        protected virtual void OnClose() { }

        protected virtual void OnClick(string btnName) { }

        protected virtual void OnValueChanged(string toggleName, bool value) { }

        #endregion 生命周期函数(需子类重写)

        /// <summary>
        /// 打开UI
        /// </summary>
        public void OpenUI(object data = null)
        {
            gameObject.SetActive(true);
            OnOpen(data);
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI()
        {
            gameObject.SetActive(false);
            OnClose();
        }

        public void SetDirty()
        {
            dirty = true;
        }

        /// <summary>
        /// 得到对应名字的对应控件脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName"></param>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < controlDic[controlName].Count; ++i)
                {
                    if (controlDic[controlName][i] is T)
                        return controlDic[controlName][i] as T;
                }
            }

            return null;
        }

        /// <summary>
        /// 找到子对象的对应控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            T[] controls = this.GetComponentsInChildren<T>();
            for (int i = 0; i < controls.Length; ++i)
            {
                string objName = controls[i].gameObject.name;
                if (!objName.StartsWith('@'))
                {
                    continue;
                }
                else
                {
                    objName = objName[1..];
                }

                if (controlDic.ContainsKey(objName))
                    controlDic[objName].Add(controls[i]);
                else
                    controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
                //如果是按钮控件
                if (controls[i] is Button)
                {
                    (controls[i] as Button).onClick.AddListener(() =>
                    {
                        OnClick(objName);
                    });
                }
                //如果是单选框或者多选框
                else if (controls[i] is Toggle)
                {
                    (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                    {
                        OnValueChanged(objName, value);
                    });
                }
            }
        }
    }
}