using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    public class InfoTable
    {
        List<Tuple<string, Func<object>>> info;

        public GameObject exampleText;

        GameObject go;
        int fontSize;

        public InfoTable(Transform parent, List<Tuple<string, Func<object>>> info, int fontSize = 12)
        {
            this.info = info;
            this.fontSize = fontSize;
            go = new GameObject("Info Table");
            go.transform.parent = parent;
            VerticalLayoutGroup VLayGr = go.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
        }

        public void Redraw()
        {
            for (int i = go.transform.childCount - 1; i >= 0; i--)      // Kill all previous lines
            {
                UnityEngine.Object.Destroy(go.transform.GetChild(i).gameObject);
                go.transform.GetChild(i).SetParent(null);
            }
            if (info.Count == 0)        // no data - place a dummy line
            {
                GameObject line = CreateLine();

                TextBox name = new TextBox(line.transform, "#####", "#####", fontSize, TextAnchor.MiddleLeft);
                name.SetFlexibleWidth(1);

                TextBox data = new TextBox(line.transform, "#####", "#####", fontSize, TextAnchor.MiddleRight);
            }
            else
            {
                for (int i = 0; i < info.Count; i++)       ///TODO: OPTIMISATION - DO ONLY REDRAW IF THE LINES ARE NOT THE SAME
                {
                    GameObject line = CreateLine();
                    TextBox name = new TextBox(line.transform, info[i].Item1, "#####", fontSize, TextAnchor.MiddleLeft);
                    name.SetFlexibleWidth(1);

                    TextBox data = new TextBox(line.transform, info[i].Item2, "#####", fontSize, TextAnchor.MiddleRight);
                }
            }
        }

        private GameObject CreateLine()
        {
            GameObject line = new GameObject("Line");
            line.transform.SetParent(go.transform, false);
            LayoutElement LayEl = line.AddComponent<LayoutElement>();
            LayEl.minHeight = fontSize;
            LayEl.preferredHeight = fontSize * 2;
            LayEl.flexibleWidth = 1;
            LayEl.flexibleHeight = 0;
            HorizontalLayoutGroup HLayGr = line.AddComponent<HorizontalLayoutGroup>();
            HLayGr.childForceExpandWidth = true;
            HLayGr.childForceExpandHeight = true;
            HLayGr.padding = new RectOffset((int)LayEl.minHeight, (int)LayEl.minHeight, 0, 0);
            return line;
        }

        public void SetInfo(List<Tuple<string, Func<object>>> newInfo)
        {
            info = newInfo;
        }

        public void SetInfo(Tuple<string, Func<object>> newInfo)
        {
            info = new List<Tuple<string, Func<object>>> {
                newInfo
            };
        }

        public void AddInfo(Tuple<string, Func<object>> newInfo)
        {
            info.Add(newInfo);
        }

        internal void ResetInfo()
        {
            info = new List<Tuple<string, Func<object>>>();
        }
    }
}
