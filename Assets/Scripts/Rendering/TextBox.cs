using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace Assets.Scripts.Rendering
{
    public class TextBox
    {
        public TextRef Text;

        public float Width { get { return text.preferredWidth / 2; } }

        GameObject go;
        Text text;

        /// <summary>
        /// The gameobject that contains the text.
        /// WARNING: Do not attach a layout element to this gameobjet. If you need one, use a container
        /// as parent for the TextBox.
        /// </summary>
        public GameObject gameObject { get { return go; } }
        public RectTransform transform { get { return go.transform as RectTransform; } }

        const float MOUSE_OVER_DISPLAY_TRESHOLD = 0.0f;

        public TextBox(Transform parent, TextRef Text, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft, Color? color = null)
        {
            this.Text = Text;
            go = new GameObject(Text, typeof(RectTransform));
            StandardConstructor(parent, size, allignment);
            text.text = Data.Localisation.GetText(Text);
            text.color = color ?? Data.Graphics.Color_.text;
        }

        private void StandardConstructor(Transform parent, int size, TextAnchor allignment)
        {
            go.transform.SetParent(parent, false);
            RectTransform tr = (RectTransform)go.transform;
            float anchX = 0; float anchY = 0;
            switch (allignment)
            {
            case TextAnchor.LowerLeft: anchX = 0; anchY = 0; break;
            case TextAnchor.MiddleLeft: anchX = 0; anchY = 0.5f; break;
            case TextAnchor.UpperLeft: anchX = 0; anchY = 1; break;
            case TextAnchor.LowerCenter: anchX = 0.5f; anchY = 0; break;
            case TextAnchor.MiddleCenter: anchX = 0.5f; anchY = 0.5f; break;
            case TextAnchor.UpperCenter: anchX = 0.5f; anchY = 1; break;
            case TextAnchor.LowerRight: anchX = 1; anchY = 0; break;
            case TextAnchor.MiddleRight: anchX = 1; anchY = 0.5f; break;
            case TextAnchor.UpperRight: anchX = 1; anchY = 1; break;

            }
            tr.anchorMin = new Vector2(anchX, anchY);
            tr.anchorMax = new Vector2(anchX, anchY);
            tr.pivot = new Vector2(anchX, anchY);
            tr.anchoredPosition = new Vector2(0, 0);
            tr.localScale = new Vector3(0.5f, 0.5f, 1);
            tr.sizeDelta = new Vector2(((RectTransform)parent).sizeDelta.x * 2, (size + 2) * 2);
            text = go.AddComponent<Text>();
            text.font = Data.Graphics.GetStandardFont();
            text.fontSize = size * 2;
            text.alignment = allignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            TextBoxScript tbs = go.AddComponent<TextBoxScript>();
            tbs.parent = this;
            tbs.hasMouseover = Text.AltText != null;
        }

        public void SetColor(Color col)
        {
            text.color = col;
        }

        private void Update()
        {
            if (Text.isChanging)
                text.text = Text;
        }

        class TextBoxScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public TextBox parent;
            public bool hasMouseover = false;
            bool mouseActive = false;
            public float mouseTimeActive = 0;

            private void Update()
            {
                parent.Update();
                if(hasMouseover && mouseActive)
                {
                    mouseTimeActive += Time.deltaTime;
                    if(mouseTimeActive > MOUSE_OVER_DISPLAY_TRESHOLD)
                    {
                        MouseOver.Activate(parent.Text.AltText);
                    }
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                mouseActive = true;
                mouseTimeActive = 0;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                mouseActive = false;
                mouseTimeActive = 0;
                MouseOver.Deactivate();
            }
        }
    }

    /// <summary>
    /// Text reference class. This is a container class for all text that has to be send to
    /// the UI system. This can be a simple string, a reference to the localisation file or
    /// a reference to a value somewhere.
    /// You can use a TextRef implicitly as a string.
    /// </summary>
    public class TextRef
    {
        TextRef() { }

        string text;
        string text2nd;

        Func<object> script;
        Func<object> script2nd;

        enum RefType { direct, localised, reference}
        RefType refType;
        public bool isChanging { get { return refType == RefType.reference; } }

        /// <summary>
        /// Create a new text reference that can store a string which can be read from the localisation files.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="localised">Whether or not this value is a key in the localisation files.</param>
        /// <returns></returns>
        public static TextRef Create(string text, bool localised = true)
        {
            TextRef tr = new TextRef();
            tr.text = text;
            if (localised)
                tr.refType = RefType.localised;
            else
                tr.refType = RefType.direct;
            return tr;
        }

        /// <summary>
        /// Create a new text reference that can store a string and an alternative string which can be read
        /// from the localisation files.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="altText">This is mainly used for mouseover text.</param>
        /// <param name="localised">Whether or not this value is a key in the localisation files.</param>
        /// <returns></returns>
        public static TextRef Create(string text, string altText, bool localised = true)
        {
            TextRef tr = new TextRef() {
                text = text,
                text2nd = altText
            };
            if (localised)
                tr.refType = RefType.localised;
            else
                tr.refType = RefType.direct;
            return tr;
        }

        /// <summary>
        /// Create a new text reference that will remember the reference to a value in
        /// the program. Refer to the object, not to the ToString of the object. The
        /// TextRef object will make sure numbers are properly formatted.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static TextRef Create(Func<object> reference)
        {
            TextRef tr = new TextRef() {
                script = reference,
                refType = RefType.reference
            };
            return tr;
        }

        /// <summary>
        /// Create a new text reference that will remember the reference to a value in
        /// the program. Refer to the object, not to the ToString of the object. The
        /// TextRef object will make sure numbers are properly formatted. This version
        /// of Create can also link an alternative text.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="altRef"></param>
        /// <returns></returns>
        public static TextRef Create(Func<object> reference, Func<object> altRef)
        {
            TextRef tr = new TextRef() {
                script = reference,
                script2nd = altRef,
                refType = RefType.reference
            };
            return tr;
        }

        public static implicit operator string(TextRef tr)
        {
            switch (tr.refType)
            {
            case RefType.direct: return tr.text;
            case RefType.localised: return Data.Localisation.GetText(tr.text);
            case RefType.reference: return tr.ExtractData();
            }
            throw new Exception("There exist another text ref type?");
        }

        private string ExtractData(bool alt = false)
        {
            object d;
            if (alt)
                d = script2nd();
            else
                d = script();
            Type t = d.GetType();
            if (t == typeof(double))
                return ToSI((double)d, "0.##");
            else if (t == typeof(float))
                return ToSI((float)d, "0.##");
            else if (t == typeof(int))
                return ToSI((int)d, "0.##");
            else if (t == typeof(long))
                return ToSI((long)d, "0.##");
            else
                return d.ToString();
        }

        public string Text { get { return this; } }

        public string AltText
        {
            get
            {
                if (text2nd == null && script2nd == null) return null;
                switch (refType)
                {
                case RefType.direct: return text2nd;
                case RefType.localised: return Data.Localisation.GetText(text2nd);
                case RefType.reference: return ExtractData(true);
                }
                throw new Exception("There exist another text ref type?");
            }
        }


        /// <summary>
        /// Found on stackoverflow
        /// https://stackoverflow.com/questions/12181024/formatting-a-number-with-a-metric-prefix
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        string ToSI(double d, string format = null)
        {
            if (d == 0 || (d >= 0.1 && d < 10000)) return d.ToString(format);

            char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
            char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

            int degree = (int)Math.Floor(Math.Log10(Math.Abs(d)) / 3);
            double scaled = d * Math.Pow(1000, -degree);

            if (degree - 1 >= incPrefixes.Length) return "~inf";
            if (-degree - 1 >= decPrefixes.Length) return "~0";
            char? prefix = null;
            switch (Math.Sign(degree))
            {
            case 1: prefix = incPrefixes[degree - 1]; break;
            case -1: prefix = decPrefixes[-degree - 1]; break;
            }

            return scaled.ToString(format) + prefix;
        }
    }
}
