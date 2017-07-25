using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    public class InfoTable : MonoBehaviour
    {
        List<Tuple<string, string>> info;

        public GameObject exampleText;

        public void Awake()
        {
            if (exampleText.GetComponentInChildren<Text>() == null)
                throw new ArgumentException("You did not include a Text component in your example text.");
        }

        public void Start()
        {
            var VLayGr = gameObject.GetComponent<VerticalLayoutGroup>();
            if (VLayGr == null)
                VLayGr = gameObject.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
        }

        public void Redraw()
        {
            if (transform.childCount == 0)  // No child exist, so create the first line
            {
                GameObject line = new GameObject("Line");
                line.transform.SetParent(transform, false);
                var LayEl = line.AddComponent<LayoutElement>();
                LayEl.minHeight = exampleText.GetComponent<Text>().fontSize;
                LayEl.preferredHeight = exampleText.GetComponent<Text>().fontSize * 2;
                LayEl.flexibleWidth = 1;
                LayEl.flexibleHeight = 0;
                var HLayGr = line.AddComponent<HorizontalLayoutGroup>();
                HLayGr.childForceExpandWidth = true;
                HLayGr.childForceExpandHeight = true;
                HLayGr.padding = new RectOffset((int)LayEl.minHeight, (int)LayEl.minHeight, 0, 0);

                GameObject name = Instantiate(exampleText);
                name.name = "Name";
                name.transform.SetParent(line.transform);
                LayEl = name.AddComponent<LayoutElement>();
                LayEl.flexibleWidth = 1;
                name.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

                GameObject data = Instantiate(exampleText);
                data.name = "Data";
                data.transform.SetParent(line.transform);
                data.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
                //LayEl = name.AddComponent<LayoutElement>();
                //LayEl.flexibleWidth = 1;
            }
            if (info.Count == 0)
            {
                transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "#####";
                transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "#####";
            }
            int i;
            for (i = 0; i < info.Count; i++)
            {
                if (transform.childCount <= i)  // There are not enough childeren (lines)
                {
                    Transform nLine = Instantiate(transform.GetChild(0).gameObject).transform;
                    nLine.SetParent(transform);
                    nLine.SetAsLastSibling();
                }
                transform.GetChild(i).GetChild(0).GetComponent<Text>().text = info[i].Item1;
                transform.GetChild(i).GetChild(1).GetComponent<Text>().text = info[i].Item2;
            }
            if (i == 0) i++;
            while (transform.childCount < i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
                i++;
            }
        }

        public void SetInfo(List<Tuple<string,string>> newInfo)
        {
            info = newInfo;
        }

        public void SetInfo(Tuple<string, string> newInfo)
        {
            info = new List<Tuple<string, string>> {
                newInfo
            };
        }

        public void AddInfo(Tuple<string, string> newInfo)
        {
            info.Add(newInfo);
        }

        internal void ResetInfo()
        {
            info = new List<Tuple<string, string>>();
        }

        public void FullRedraw()
        {
            for(int i = transform.childCount-1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
                transform.GetChild(i).SetParent(null);
            }
            Redraw();
        }
    }
}
