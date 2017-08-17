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
        string title = null;

        GameObject go;
        public GameObject gameObject { get { return go; } }
        public RectTransform transform { get { return go.transform as RectTransform; } }
        int fontSize;

        /// <summary>
        /// Use this constructor if you want to make a table where the number of elements is fixed
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="info">The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        public InfoTable(Transform parent, List<Tuple<string, Func<object>>> info, int width = 200, int fontSize = 12, string title = null) :
            this(parent, width, fontSize, title)
        {
            this.info = info;
            Redraw();
        }

        /// <summary>
        /// Use this constructor if the number of elements in the table is variable.
        /// BEWARE: this is badly optimised at the moment.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="script">This function must return the list with entries. The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        public InfoTable(Transform parent, Func<List<Tuple<string, Func<object>>>> script, int width = 200, int fontSize = 12, string title = null) :
            this(parent, width, fontSize, title)
        {
            info = null;
            ActiveInfoTable ait = go.AddComponent<ActiveInfoTable>();
            ait.parent = this;
            ait.script = script;
            info = script();
            Redraw();
        }

        InfoTable(Transform parent, int width = 200, int fontSize = 12, string title = null)
        {
            this.title = title;
            this.fontSize = fontSize;
            go = new GameObject("Info Table", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            ((RectTransform)go.transform).sizeDelta = new Vector2(width, 100);
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
            if(title != null)
            {
                CreateTitle();
            }
            if (info.Count == 0)        // no data - place a dummy line
            {
                GameObject line = CreateLine();
                GameObject nameCont = new GameObject("Name Container", typeof(RectTransform));
                nameCont.transform.SetParent(line.transform);
                TextBox name = new TextBox(nameCont.transform, "#####", "#####", fontSize, TextAnchor.MiddleLeft);
                nameCont.AddComponent<LayoutElement>().flexibleWidth = 1;


                GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                dataCont.transform.SetParent(line.transform);
                TextBox data = new TextBox(dataCont.transform, "#####", "#####", fontSize, TextAnchor.MiddleRight);
            }
            else
            {
                for (int i = 0; i < info.Count; i++)       ///TODO: OPTIMISATION - DO ONLY REDRAW IF THE LINES ARE NOT THE SAME
                {
                    GameObject line = CreateLine();
                    GameObject nameCont = new GameObject("Name Container", typeof(RectTransform));
                    nameCont.transform.SetParent(line.transform);
                    TextBox name = new TextBox(nameCont.transform, info[i].Item1, "#####", fontSize, TextAnchor.MiddleLeft);
                    nameCont.AddComponent<LayoutElement>().flexibleWidth = 1;

                    GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                    dataCont.transform.SetParent(line.transform);
                    TextBox data = new TextBox(dataCont.transform, info[i].Item2, "#####", fontSize, TextAnchor.MiddleRight);
                }
            }
        }

        private void CreateTitle()
        {
            TextBox titleTxt = new TextBox(go.transform, title, null, (int)(fontSize * 1.2f), TextAnchor.MiddleCenter);
            LayoutElement LayEl = titleTxt.gameObject.AddComponent<LayoutElement>();
            LayEl.minHeight = fontSize * 1.2f;
            LayEl.preferredHeight = fontSize * 2 * 1.2f;
            LayEl.flexibleWidth = 1;
            LayEl.flexibleHeight = 0;
        }

        private GameObject CreateLine()
        {
            GameObject line = new GameObject("Line", typeof(RectTransform));
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

        /// <summary>
        /// Removes the info from the table and sets new info.
        /// BEWARE: make sure to call Redraw() after setting the info
        /// </summary>
        /// <param name="newInfo"></param>
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

        public class ActiveInfoTable : MonoBehaviour
        {
            public InfoTable parent;

            public Func<List<Tuple<string, Func<object>>>> script;
            
            private void Update()
            {
                List<Tuple<string, Func<object>>> newInfo = script();
                if (parent.info.SequenceEqual(newInfo) == false)
                {
                    parent.info = newInfo;
                    parent.Redraw();
                }
            }
        }
    }
}
