using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MYTOOL.UI
{
    public class TextType : MonoBehaviour
    {
        public delegate void OnComplete();

        [SerializeField] float defaultSpeed = 0.05f;

        private Text label;
        private string finalText = string.Empty;
        private Coroutine typeTextCoroutine;

        private static readonly string[] uguiSymbols = { "b", "i" };
        private static readonly string[] uguiCloseSymbols = { "b", "i", "size", "color" };

        private OnComplete OnCompleteCallback;

        private void InitText()
        {
            if (label == null) label = GetComponent<Text>();
        }

        public void Awake()
        {
            InitText();
        }

        public void SetText(string text, float speed = -1)
        {
            InitText();

            defaultSpeed = speed > 0 ? speed : defaultSpeed;
            finalText = ReplaceSpeed(text);
            label.text = "";

            if (typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
                typeTextCoroutine = null;
            }

            typeTextCoroutine = StartCoroutine(InnerTypeText(text));
        }

        public void SkipTypeText()
        {
            if (typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
                typeTextCoroutine = null;
            }

            label.text = finalText;

            OnCompleteCallback?.Invoke();
        }

        public IEnumerator InnerTypeText(string text)
        {
            string currentText = "";

            int length = text.Length;
            float speed = defaultSpeed;
            bool tagOpened = false;
            string tagType = "";
            bool symbolDetected = false;

            for (int i = 0; i < length; i++)
            {
                //匹配speed
                if (text[i] == '[' && i + 6 < length && text.Substring(i, 7).Equals("[speed="))
                {
                    string parseSpeed = "";
                    for (int j = i + 7; j < length; j++)
                    {
                        if (text[j] == ']')
                        {
                            break;
                        }

                        parseSpeed += text[j];
                    }

                    if (!float.TryParse(parseSpeed, out speed))
                    {
                        speed = defaultSpeed;
                    }

                    i += 8 + parseSpeed.Length - 1;
                    continue;
                }

                symbolDetected = false;
                //匹配 <i> 或 <b>
                for (int j = 0; j < uguiSymbols.Length; j++)
                {
                    string symbol = string.Format("<{0}>", uguiSymbols[j]);
                    if (text[i] == '<' && i + 1 + uguiSymbols[j].Length < length && text.Substring(i, 2 + uguiSymbols[j].Length).Equals(symbol))
                    {
                        currentText += symbol;
                        i += (2 + uguiSymbols[j].Length) - 1;
                        symbolDetected = true;
                        tagOpened = true;
                        tagType = uguiSymbols[j];
                        break;
                    }
                }

                //匹配富文本color格式
                if (text[i] == '<' && i + 1 + 15 < length && text.Substring(i, 2 + 6).Equals("<color=#") && text[i + 16] == '>')
                {
                    currentText += text.Substring(i, 2 + 6 + 8);
                    i += (2 + 14) - 1;
                    symbolDetected = true;
                    tagOpened = true;
                    tagType = "color";
                }

                //匹配富文本size格式
                if (text[i] == '<' && i + 5 < length && text.Substring(i, 6).Equals("<size="))
                {
                    string parseSize = "";
                    for (var j = i + 6; j < length; j++)
                    {
                        if (text[j] == '>')
                        {
                            break;
                        }

                        parseSize += text[j];
                    }

                    if (int.TryParse(parseSize, out _))
                    {
                        currentText += text.Substring(i, 7 + parseSize.Length);
                        i += (7 + parseSize.Length) - 1;
                        symbolDetected = true;
                        tagOpened = true;
                        tagType = "size";
                    }
                }

                //匹配富文本结束 </i> </b> </size> </color>
                for (int j = 0; j < uguiCloseSymbols.Length; j++)
                {
                    string symbol = string.Format("</{0}>", uguiCloseSymbols[j]);
                    if (text[i] == '<' && i + 2 + uguiCloseSymbols[j].Length < length && text.Substring(i, 3 + uguiCloseSymbols[j].Length).Equals(symbol))
                    {
                        currentText += symbol;
                        i += (3 + uguiCloseSymbols[j].Length) - 1;
                        symbolDetected = true;
                        tagOpened = false;
                        break;
                    }
                }

                if (symbolDetected) continue;

                currentText += text[i];
                string tagLabel = tagOpened ? string.Format("</{0}>", tagType) : "";
                label.text = currentText + tagLabel;
                yield return new WaitForSeconds(speed);

                if (tagOpened)
                {
                    if (label.text[..^tagLabel.Length] != currentText)
                    {
                        currentText = label.text[..^tagLabel.Length];
                    }
                }
                else
                {
                    if (label.text != currentText)
                    {
                        currentText = label.text;
                    }
                }
            }

            typeTextCoroutine = null;
            OnCompleteCallback?.Invoke();
        }

        private string ReplaceSpeed(string text)
        {
            return Regex.Replace(text, @"\[speed=\d+(\.\d+)?\]", "");
        }

        public bool IsSkippable()
        {
            return typeTextCoroutine != null;
        }

        public void SetOnComplete(OnComplete onComplete)
        {
            OnCompleteCallback = onComplete;
        }
    }

    public static class TypeTextUtility
    {
        public static void TypeText(this Text label, string text, float speed = 0.05f, TextType.OnComplete onComplete = null)
        {
            if (!label.TryGetComponent<TextType>(out var typeText))
            {
                typeText = label.gameObject.AddComponent<TextType>();
            }

            typeText.SetText(text, speed);
            typeText.SetOnComplete(onComplete);
        }
        public static bool IsSkippable(this Text label)
        {
            if (!label.TryGetComponent<TextType>(out var typeText))
            {
                typeText = label.gameObject.AddComponent<TextType>();
            }

            return typeText.IsSkippable();
        }
        public static void SkipTypeText(this Text label)
        {
            if (!label.TryGetComponent<TextType>(out var typeText))
            {
                typeText = label.gameObject.AddComponent<TextType>();
            }

            typeText.SkipTypeText();
        }
    }
}