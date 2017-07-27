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

        public TextBox(Transform parent, string textID, string mousoverID, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft)
        {
            TextID = textID; MousoverID = mousoverID;
            isData = false;
            go = new GameObject(textID, typeof(RectTransform));
            go.transform.parent = parent;
            text = go.AddComponent<Text>();
            text.text = Data.Localisation.GetText(textID);
            text.font = Data.Graphics.GetStandardFont();
            text.fontSize = size;
            text.alignment = allignment;
            TextBoxScript tbs = go.AddComponent<TextBoxScript>();
            tbs.parent = this;
        }

        public TextBox(Transform parent, Func<object> data, string mousoverID, int size = 12, TextAnchor allignment = TextAnchor.MiddleLeft)
        {
            TextID = null; MousoverID = mousoverID;
            isData = true;
            Data_ = data;
            go = new GameObject("dataText", typeof(RectTransform));
            go.transform.parent = parent;
            text = go.AddComponent<Text>();
            text.text = Data.Localisation.GetText(data().ToString());
            text.font = Data.Graphics.GetStandardFont();
            text.fontSize = size;
            text.alignment = allignment;
            TextBoxScript tbs = go.AddComponent<TextBoxScript>();
            tbs.parent = this;
        }

        public void SetFlexibleWidth(float width)
        {
            if (go.GetComponent<LayoutElement>() == null)
                go.AddComponent<LayoutElement>();
            go.GetComponent<LayoutElement>().flexibleWidth = width;
        }

        private void Update()
        {
            if (isData)
            {
                text.text = Data.Localisation.GetText(Data_.ToString());
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
