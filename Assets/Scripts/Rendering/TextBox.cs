using UnityEngine;
using UnityEngine.UI;
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

        bool isData;

        GameObject go;
        Text text;
        public GameObject gameObject { get { return go; } }
        public RectTransform transform { get { return go.transform as RectTransform; } }

        public TextBox(Transform parent, string textID, string mousoverID, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft, Color? color = null)
        {
            TextID = textID; MousoverID = mousoverID;
            isData = false;
            go = new GameObject(textID, typeof(RectTransform));
            StanConstr(parent, size, allignment);
            text.text = Data.Localisation.GetText(textID);
            text.color = color ?? Data.Graphics.Color_.text;
        }

        public TextBox(Transform parent, Func<object> data, string mousoverID, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft, Color? color = null)
        {
            TextID = null; MousoverID = mousoverID;
            isData = true;
            Data_ = data;
            go = new GameObject("dataText", typeof(RectTransform));
            StanConstr(parent, size, allignment);
            text.text = data().ToString();
            text.color = color ?? Color.red;
        }

        private void StanConstr(Transform parent, int size, TextAnchor allignment)
        {
            go.transform.SetParent(parent, false);
            RectTransform tr = (RectTransform)go.transform;
            tr.anchorMin = new Vector2(0, 0.5f);
            tr.anchorMax = new Vector2(0, 0.5f);
            tr.pivot = new Vector2(0, 0.5f);
            tr.anchoredPosition = new Vector2(0, 0);
            text = go.AddComponent<Text>();
            text.font = Data.Graphics.GetStandardFont();
            text.fontSize = size;
            text.alignment = allignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            TextBoxScript tbs = go.AddComponent<TextBoxScript>();
            tbs.parent = this;
        }

        public void SetFlexibleWidth(float width)
        {
            if (go.GetComponent<LayoutElement>() == null)
                go.AddComponent<LayoutElement>();
            go.GetComponent<LayoutElement>().flexibleWidth = width;
        }

        public void SetColor(Color col)
        {
            text.color = col;
        }

        private void Update()
        {
            if (isData)
            {
                text.text = Data_().ToString();
            }
        }

        class TextBoxScript : MonoBehaviour
        {
            public TextBox parent;

            private void Update()
            {
                parent.Update();
            }


        }
    }
}
