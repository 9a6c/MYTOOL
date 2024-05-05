using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MYTOOL.UI
{
    public class TextSymbolFit : Text
    {
        /// <summary>
        /// 用于匹配标点符号
        /// </summary>
        private readonly string strRegex = @"\p{P}";

        /// <summary>
        /// 用于存储text组件中的内容
        /// </summary>
        private System.Text.StringBuilder MExplainText = null;

        /// <summary>
        /// 用于存储text生成器中的内容
        /// </summary>
        private IList<UILineInfo> MExpalinTextLine;

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            StartCoroutine(MClearUpExplainMode(this, text));
        }
        private IEnumerator MClearUpExplainMode(Text _component, string _text)
        {
            _component.text = _text;
            yield return new WaitForEndOfFrame();

            MExplainText = new System.Text.StringBuilder(_component.text);
            MExpalinTextLine = _component.cachedTextGenerator.lines;

            int mChangeIndex;

            // 从第二行开始进行检测
            for (int i = 1; i < MExpalinTextLine.Count; i++)
            {
                try
                {
                    if (MExpalinTextLine[i].startCharIdx >= _component.text.Length) continue;
                    //首位是否有标点
                    bool match = Regex.IsMatch(MExplainText.ToString()[MExpalinTextLine[i].startCharIdx].ToString(), strRegex);

                    if (match)
                    {
                        mChangeIndex = MExpalinTextLine[i].startCharIdx - 1;
                        // 解决联系多个都是标点符号的问题
                        for (int j = MExpalinTextLine[i].startCharIdx - 1; j > 0; j--)
                        {
                            match = Regex.IsMatch(MExplainText.ToString()[j].ToString(), strRegex);
                            if (match)
                            {
                                mChangeIndex--;
                            }
                            else
                            {
                                break;
                            }
                        }

                        MExplainText.Insert(mChangeIndex, "\n");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            _component.text = MExplainText.ToString();
        }
    }
}