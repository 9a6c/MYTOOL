using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
#endif

namespace MYTOOL.POI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class POIFlagAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(POIFlagAttribute))]
    public class POIFlagAttributeDrawer : PropertyDrawer
    {
        private string[] flagArray;
        private GUIContent[] flagContentArray;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (flagContentArray == null)
            {
                List<FieldInfo> constants = new List<FieldInfo>();
                FieldInfo[] fieldInfos = typeof(POIFlagConstant).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    var fi = fieldInfos[i];
                    if (fi.IsLiteral && !fi.IsInitOnly) constants.Add(fi);
                }
                flagArray = new string[constants.Count];
                flagContentArray = new GUIContent[constants.Count];
                for (int i = 0; i < constants.Count; i++)
                {
                    flagArray[i] = constants[i].GetValue(null) as string;
                    flagContentArray[i] = new GUIContent(constants[i].Name);
                }
            }
            var index = Array.IndexOf(flagArray, property.stringValue);
            index = Mathf.Clamp(index, 0, flagArray.Length);
            index = EditorGUI.Popup(position, label, index, flagContentArray);
            property.stringValue = flagArray[index];
        }
    }
#endif
}