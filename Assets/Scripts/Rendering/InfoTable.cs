using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    abstract public class InfoTable
    {
        string title = null;

        GameObject go;
        public GameObject gameObject { get { return go; } }
        public RectTransform transform { get { return go.transform as RectTransform; } }
        int fontSize;


        /// <summary>
        /// Use this constructor if you want to make a 2 column table where the number of elements is fixed
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="info">The tuples in this list are the entries. The second item should return an object on which ToString() will be called.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        static public InfoTable New(Transform parent, List<Tuple<string, Func<object>>> info, int width = 200, int fontSize = 12, string title = null)
        {
            return new TwoColumnPassive(parent, info, width, fontSize, title);
        }

        /// <summary>
        /// Use this constructor if you want to make a 2 column table where the number of elements is variable
        /// BEWARE: this is badly optimised at the moment.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="script">This function must return the list with entries. The tuples in this list are the entries. The second item should return an object on which ToString() will be called.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        static public InfoTable New(Transform parent, Func<List<Tuple<string, Func<object>>>> script, int width = 200, int fontSize = 12, string title = null)
        {
            return new TwoColumnActive(parent, script, width, fontSize, title);
        }

        /// <summary>
        /// Use this constructor if you want to make a multi column table where the number of elements is fixed
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="info">The tuples in this list are the entries. The entries of the second List should return an object on which ToString() will be called.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        static public InfoTable New(Transform parent, List<Tuple<string, List<Func<object>>>> info, int width = 200, int fontSize = 12, string title = null, List<string> headers = null)
        {
            return new MultiColumnPassive(parent, info, width, fontSize, title, headers);
        }

        /// <summary>
        /// Use this constructor if you want to make a multi column table where the number of elements is variable
        /// BEWARE: this is badly optimised at the moment.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="script">This function must return the list with entries. The tuples in this list are the entries. The entries of the second List should return an object on which ToString() will be called.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        static public InfoTable New(Transform parent, Func<List<Tuple<string, List<Func<object>>>>> script, int width = 200, int fontSize = 12, string title = null, List<string> headers = null)
        {
            return new MultiColumnActive(parent, script, width, fontSize, title, headers);
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

        abstract public void Redraw();

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
        /// <param name="newInfo">The tuples in this list are the entries. The second item should return an object on which ToString() will be called.</param>
        public void SetInfo(List<Tuple<string, Func<object>>> newInfo)
        {
            if (this.GetType() == typeof(TwoColumnPassive))
                ((TwoColumnPassive)this).SetInfo(newInfo);
            else
                throw new ArgumentException("You are using the wrong 'SetInfo' for this type of infotable");
        }

        /// <summary>
        /// Adds one new line to the info
        /// BEWARE: make sure to call Redraw() after setting the info
        /// </summary>
        /// <param name="newInfo">The tuples is the entry. The second item should return an object on which ToString() will be called.</param>
        public void AddInfo(Tuple<string, Func<object>> newInfo)
        {
            if (this.GetType() == typeof(TwoColumnPassive))
                ((TwoColumnPassive)this).AddInfo(newInfo);
            else
                throw new ArgumentException("You are using the wrong 'AddInfo' for this type of infotable");
        }

        /// <summary>
        /// Removes the info from the table and sets new info.
        /// BEWARE: make sure to call Redraw() after setting the info
        /// </summary>
        /// <param name="newInfo">The tuples in this list are the entries. The entries of the second List should return an object on which ToString() will be called.</param>
        public void SetInfo(List<Tuple<string, List<Func<object>>>> newInfo)
        {
            if (this.GetType() == typeof(MultiColumnPassive))
                ((MultiColumnPassive)this).SetInfo(newInfo);
            else
                throw new ArgumentException("You are using the wrong 'SetInfo' for this type of infotable");
        }

        /// <summary>
        /// Adds one new line to the info
        /// BEWARE: make sure to call Redraw() after setting the info
        /// </summary>
        /// <param name="newInfo">The tuples is the entry. The entries of the second List should return an object on which ToString() will be called.</param>
        public void AddInfo(Tuple<string, List<Func<object>>> newInfo)
        {
            if (this.GetType() == typeof(MultiColumnPassive))
                ((MultiColumnPassive)this).AddInfo(newInfo);
            else
                throw new ArgumentException("You are using the wrong 'AddInfo' for this type of infotable");
        }

        abstract public void ResetInfo();

        protected void BaseRedraw2column(List<Tuple<string, Func<object>>> info)
        {
            for (int i = go.transform.childCount - 1; i >= 0; i--)      // Kill all previous lines
            {
                UnityEngine.Object.Destroy(go.transform.GetChild(i).gameObject);
                go.transform.GetChild(i).SetParent(null);
            }
            if (title != null)
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
                for (int i = 0; i < info.Count; i++)
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

        protected void BaseRedrawMulticolumn(List<Tuple<string, List<Func<object>>>> info, int numberOfCol)
        {
            for (int i = go.transform.childCount - 1; i >= 0; i--)      // Kill all previous lines
            {
                UnityEngine.Object.Destroy(go.transform.GetChild(i).gameObject);
                go.transform.GetChild(i).SetParent(null);
            }
            if (title != null)
            {
                CreateTitle();
            }
            if (info.Count == 0)        // no data - place a dummy line
            {
                GameObject line = CreateLine();
                GameObject nameCont = new GameObject("Name Container", typeof(RectTransform));
                nameCont.transform.SetParent(line.transform);
                TextBox name = new TextBox(nameCont.transform, "#####", null, fontSize, TextAnchor.MiddleLeft);
                nameCont.AddComponent<LayoutElement>().flexibleWidth = 1;

                for (int i = 0; i < numberOfCol; i++)
                {
                    GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                    dataCont.transform.SetParent(line.transform);
                    TextBox data = new TextBox(dataCont.transform, "#####", null, fontSize, TextAnchor.MiddleRight);
                }
            }
            else
            {
                for (int i = 0; i < info.Count; i++)
                {
                    GameObject line = CreateLine();
                    GameObject nameCont = new GameObject("Name Container", typeof(RectTransform));
                    nameCont.transform.SetParent(line.transform);
                    TextBox name = new TextBox(nameCont.transform, info[i].Item1, null, fontSize, TextAnchor.MiddleLeft);
                    nameCont.AddComponent<LayoutElement>().flexibleWidth = 1;

                    for (int j = 0; j < numberOfCol; j++)
                    {
                        GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                        dataCont.transform.SetParent(line.transform);
                        TextBox data = new TextBox(dataCont.transform, info[j].Item2[j], null, fontSize, TextAnchor.MiddleRight);
                    }
                }
            }
        }

        public class ActiveInfoTable : MonoBehaviour
        {
            public InfoTable parent;
            
            private void Update()
            {
                parent.Redraw();
            }
        }

        class TwoColumnPassive : InfoTable
        {
            List<Tuple<string, Func<object>>> info;

            /// <summary>
            /// Use this constructor if you want to make a 2 column table where the number of elements is fixed
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="info">The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public TwoColumnPassive(Transform parent, List<Tuple<string, Func<object>>> info, int width, int fontSize, string title) :
            base(parent, width, fontSize, title)
            {
                this.info = info;
                Redraw();
            }

            public override void Redraw()
            {
                BaseRedraw2column(info);
            }

            public new void SetInfo(List<Tuple<string, Func<object>>> newInfo)
            {
                info = newInfo;
            }

            public new void AddInfo(Tuple<string, Func<object>> newInfo)
            {
                info.Add(newInfo);
            }

            public override void ResetInfo()
            {
                info = new List<Tuple<string, Func<object>>>();
            }
        }

        class TwoColumnActive : InfoTable
        {
            List<Tuple<string, Func<object>>> info;
            public Func<List<Tuple<string, Func<object>>>> script;

            /// <summary>
            /// Use this constructor if the number of elements in the table is variable.
            /// BEWARE: this is badly optimised at the moment.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="script">This function must return the list with entries. The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public TwoColumnActive(Transform parent, Func<List<Tuple<string, Func<object>>>> script, int width, int fontSize, string title) :
            base(parent, width, fontSize, title)
            {
                info = null;
                ActiveInfoTable ait = go.AddComponent<ActiveInfoTable>();
                ait.parent = this;
                this.script = script;
                info = script();
                Redraw();
            }

            public override void Redraw()
            {
                var newinfo = script();
                if (newinfo != info) {
                    info = newinfo;
                    BaseRedraw2column(info);
                }
            }

            public override void ResetInfo()
            {
                info = new List<Tuple<string, Func<object>>>();
                script = null;
            }
        }

        class MultiColumnPassive : InfoTable
        {
            List<Tuple<string, List<Func<object>>>> info;
            int colms;
            private List<string> headers;

            /// <summary>
            /// Use this constructor if you want to make a 2 column table where the number of elements is fixed
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="info">The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public MultiColumnPassive(Transform parent, List<Tuple<string, List<Func<object>>>> info, int width, int fontSize, string title, List<string> headers) :
            base(parent, width, fontSize, title)
            {
                if (info == null || info.Count == 0)
                    throw new ArgumentException("The info of this infotable is empty. Please don't do this to me.");
                colms = info[0].Item2.Count;
                for (int i = 1; i < info.Count; i++)
                    if (info[i].Item2.Count != colms)
                        throw new ArgumentException("The info of this infotable has an inconsistant number of colums.");
                this.info = info;
                AddHeaders();
                Redraw();
            }

            public override void Redraw()
            {
                BaseRedrawMulticolumn(info, colms);
            }

            public new void SetInfo(List<Tuple<string, List<Func<object>>>> newInfo)
            {
                info = newInfo;
                AddHeaders();
            }

            public new void AddInfo(Tuple<string, List<Func<object>>> newInfo)
            {
                info.Add(newInfo);
            }

            public override void ResetInfo()
            {
                info = new List<Tuple<string, List<Func<object>>>>();
                AddHeaders();
            }

            private void AddHeaders()
            {
                if (headers != null && headers.Count == colms)
                    info.Insert(0, new Tuple<string, List<Func<object>>>("", headers.ConvertAll<Func<object>>(h => () => { return h; })));
                else if (headers != null && headers.Count == colms + 1)
                    info.Insert(0, new Tuple<string, List<Func<object>>>(headers[0], headers.GetRange(1, colms).ConvertAll<Func<object>>(h => () => { return h; })));
                else if (headers != null)
                    throw new ArgumentException("The headers of this infotable do not have a valid count. Use 'number of colums' or 'number of colums + 1'");
            }
        }

        class MultiColumnActive : InfoTable
        {
            List<Tuple<string, List<Func<object>>>> info;
            Func<List<Tuple<string, List<Func<object>>>>> script;
            List<string> headers;
            int colms;

            /// <summary>
            /// Use this constructor if the number of elements in the table is variable.
            /// BEWARE: this is badly optimised at the moment.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="script">This function must return the list with entries. The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public MultiColumnActive(Transform parent, Func<List<Tuple<string, List<Func<object>>>>> script, int width, int fontSize, string title, List<string> headers) :
            base(parent, width, fontSize, title)
            {
                info = null;
                ActiveInfoTable ait = go.AddComponent<ActiveInfoTable>();
                ait.parent = this;
                this.script = script;
                Redraw();
            }

            public override void Redraw()
            {
                var newinfo = script();
                if (newinfo == null || newinfo.Count == 0)
                    throw new ArgumentException("The info of this infotable is empty. Please don't do this to me.");
                colms = newinfo[0].Item2.Count;
                for (int i = 1; i < info.Count; i++)
                    if (newinfo[i].Item2.Count != colms)
                        throw new ArgumentException("The info of this infotable has an inconsistant number of colums.");
                if (newinfo != info)
                {
                    info = newinfo;
                    AddHeaders();
                    BaseRedrawMulticolumn(info, colms);
                }
            }

            public override void ResetInfo()
            {
                info = new List<Tuple<string, List<Func<object>>>>();
                script = null;
            }

            private void AddHeaders()
            {
                if (headers != null && headers.Count == colms)
                    info.Insert(0, new Tuple<string, List<Func<object>>>("", headers.ConvertAll<Func<object>>(h => () => { return h; })));
                else if (headers != null && headers.Count == colms + 1)
                    info.Insert(0, new Tuple<string, List<Func<object>>>(headers[0], headers.GetRange(1, colms).ConvertAll<Func<object>>(h => () => { return h; })));
                else if (headers != null)
                    throw new ArgumentException("The headers of this infotable do not have a valid count. Use 'number of colums' or 'number of colums + 1'");
            }
        }
    }
}
