using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MYTOOL.UI.Hypertext
{
    public class RegexHypertext : HypertextBase
    {
        private readonly List<Entry> entries = new List<Entry>();

        struct Entry
        {
            public readonly string RegexPattern;
            public readonly Color Color;
            public readonly Action<string> Callback;

            public Entry(string regexPattern, Color color, Action<string> callback)
            {
                RegexPattern = regexPattern;
                Color = color;
                Callback = callback;
            }
        }

        /// <summary>
        /// 在匹配正则表达式的部分字符串中注册点击事件列表。
        /// </summary>
        /// <param name="regexPattern">正则表达式</param>
        /// <param name="onClick">点击时的回调</param>
        public void OnClick(string regexPattern, Action<string> onClick)
        {
            OnClick(regexPattern, color, onClick);
        }

        /// <summary>
        /// 为匹配正则表达式的部分字符串注册颜色和点击事件列表。
        /// </summary>
        /// <param name="regexPattern">正则表达式</param>
        /// <param name="color">文本颜色</param>
        /// <param name="onClick">点击时的回调</param>
        public void OnClick(string regexPattern, Color color, Action<string> onClick)
        {
            if (string.IsNullOrEmpty(regexPattern) || onClick == null)
            {
                return;
            }

            entries.Add(new Entry(regexPattern, color, onClick));
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            entries.Clear();
        }

        protected override void AddListeners()
        {
            foreach (var entry in entries)
            {
                foreach (Match match in Regex.Matches(text, entry.RegexPattern))
                {
                    OnClick(match.Index, match.Value.Length, entry.Color, entry.Callback);
                }
            }
        }
    }
}