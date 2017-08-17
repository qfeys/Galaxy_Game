using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace Assets.Scripts.Rendering
{
    public class TextBox
    {
        public string TextID { get { return _textID; } set { _textID = value; } }
        public string _textID;
        public string MousoverID { get { return _mousoverID; } set { _mousoverID = value; } }
        public string _mousoverID;
        public Func<object> Data_ { get { return _data; } set { _data = value; } }
        public Func<object> _data;

        public float Width { get { return text.preferredWidth / 2; } }

        bool isData;

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

        public TextBox(Transform parent, string textID, string mousoverID, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft, Color? color = null)
        {
            TextID = textID; MousoverID = mousoverID;
            isData = false;
            go = new GameObject(textID, typeof(RectTransform));
            StandardConstructor(parent, size, allignment);
            text.text = Data.Localisation.GetText(textID);
            text.color = color ?? Data.Graphics.Color_.text;
        }

        public TextBox(Transform parent, Func<object> data, string mousoverID, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft, Color? color = null)
        {
            TextID = null; MousoverID = mousoverID;
            isData = true;
            Data_ = data;
            go = new GameObject("dataText", typeof(RectTransform));
            StandardConstructor(parent, size, allignment);
            text.color = color ?? Color.red;
            Update();
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
            text = go.AddComponent<Text>();
            text.font = Data.Graphics.GetStandardFont();
            text.fontSize = size * 2;
            text.alignment = allignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            TextBoxScript tbs = go.AddComponent<TextBoxScript>();
            tbs.parent = this;
            tbs.hasMouseover = MousoverID != null;
        }

        public void SetColor(Color col)
        {
            text.color = col;
        }

        private void Update()
        {
            if (isData)
            {
                object d = Data_();
                if (d.GetType() == typeof(double))
                    text.text = ((double)d).ToString("0.##");
                else
                    text.text = Data_().ToString();
            }
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
                        MouseOver.Activate(Data.Localisation.GetText(parent.MousoverID));
                    }
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                Debug.Log("POINTER ENTER:" + parent.TextID);
                mouseActive = true;
                mouseTimeActive = 0;
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                Debug.Log("POINTER EXIT");
                mouseActive = false;
                mouseTimeActive = 0;
                MouseOver.Deactivate();
            }
        }
    }
}
