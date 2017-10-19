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
        TextRef title = null;

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
        static public InfoTable Create(Transform parent, List<Tuple<TextRef, TextRef>> info, int width = 200, int fontSize = 12, TextRef title = null)
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
        static public InfoTable Create(Transform parent, Func<List<Tuple<TextRef, TextRef>>> script, int width = 200, int fontSize = 12, TextRef title = null)
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
        static public InfoTable Create(Transform parent, List<List<TextRef>> info, int width = 200, int fontSize = 12, TextRef title = null, List<TextRef> headers = null)
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
        static public InfoTable Create(Transform parent, Func<List<List<TextRef>>> script, int width = 200, int fontSize = 12, TextRef title = null, List<TextRef> headers = null)
        {
            return new MultiColumnActive(parent, script, width, fontSize, title, headers);
        }

        InfoTable(Transform parent, int width = 200, int fontSize = 12, TextRef title = null)
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
            TextBox titleTxt = new TextBox(go.transform, TextRef.Create(title), (int)(fontSize * 1.2f), TextAnchor.MiddleCenter);
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
        public void SetInfo(List<Tuple<TextRef, TextRef>> newInfo)
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
        public void AddInfo(Tuple<TextRef, TextRef> newInfo)
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
        public void SetInfo(List<List<TextRef>> newInfo)
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
        public void AddInfo(List<TextRef> newInfo)
        {
            if (this.GetType() == typeof(MultiColumnPassive))
                ((MultiColumnPassive)this).AddInfo(newInfo);
            else
                throw new ArgumentException("You are using the wrong 'AddInfo' for this type of infotable");
        }

        abstract public void ResetInfo();

        protected void BaseRedraw2column(List<Tuple<TextRef, TextRef>> info)
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
                TextBox name = new TextBox(nameCont.transform, TextRef.Create("#####", false), fontSize, TextAnchor.MiddleLeft);
                nameCont.AddComponent<LayoutElement>().flexibleWidth = 1;

                GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                dataCont.transform.SetParent(line.transform);
                TextBox data = new TextBox(dataCont.transform, TextRef.Create("#####", false), fontSize, TextAnchor.MiddleRight);
            }
            else
            {
                for (int i = 0; i < info.Count; i++)
                {
                    GameObject line = CreateLine();
                    GameObject nameCont = new GameObject("Name Container", typeof(RectTransform));
                    nameCont.transform.SetParent(line.transform);
                    TextBox name = new TextBox(nameCont.transform, info[i].Item1, fontSize, TextAnchor.MiddleLeft);
                    nameCont.AddComponent<LayoutElement>().flexibleWidth = 1;

                    GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                    dataCont.transform.SetParent(line.transform);
                    TextBox data = new TextBox(dataCont.transform, info[i].Item2, fontSize, TextAnchor.MiddleRight);
                }
            }
        }

        protected void BaseRedrawMulticolumn(List<List<TextRef>> info, int numberOfCol)
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
                for (int i = 0; i < numberOfCol; i++)
                {
                    GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                    dataCont.transform.SetParent(line.transform);
                    TextBox data = new TextBox(dataCont.transform, TextRef.Create("#####", false), fontSize, i == 0 ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
                    if (i == 0) dataCont.AddComponent<LayoutElement>().flexibleWidth = 1;
                }
            }
            else
            {
                for (int i = 0; i < info.Count; i++)
                {
                    GameObject line = CreateLine();
                    for (int j = 0; j < numberOfCol; j++)
                    {
                        GameObject dataCont = new GameObject("Data Container", typeof(RectTransform));
                        dataCont.transform.SetParent(line.transform);
                        ((RectTransform)dataCont.transform).sizeDelta = new Vector2(((RectTransform)line.transform.parent).rect.width / numberOfCol, 50);
                        Debug.Log("rect width: " + ((RectTransform)line.transform.parent).rect.width);
                        Debug.Log("size delta x: " + ((RectTransform)dataCont.transform).sizeDelta.x);
                        TextBox data = new TextBox(dataCont.transform, info[i][j], fontSize, j == 0 ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
                        dataCont.AddComponent<LayoutElement>();
                        if (j==0) dataCont.GetComponent<LayoutElement>().flexibleWidth = 1;
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
            List<Tuple<TextRef, TextRef>> info;

            /// <summary>
            /// Use this constructor if you want to make a 2 column table where the number of elements is fixed
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="info">The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public TwoColumnPassive(Transform parent, List<Tuple<TextRef, TextRef>> info, int width, int fontSize, TextRef title) :
            base(parent, width, fontSize, title)
            {
                this.info = info;
                Redraw();
            }

            public override void Redraw()
            {
                BaseRedraw2column(info);
            }

            public new void SetInfo(List<Tuple<TextRef, TextRef>> newInfo)
            {
                info = newInfo;
            }

            public new void AddInfo(Tuple<TextRef, TextRef> newInfo)
            {
                info.Add(newInfo);
            }

            public override void ResetInfo()
            {
                info = new List<Tuple<TextRef, TextRef>>();
            }
        }

        class TwoColumnActive : InfoTable
        {
            List<Tuple<TextRef, TextRef>> info;
            public Func<List<Tuple<TextRef, TextRef>>> script;

            /// <summary>
            /// Use this constructor if the number of elements in the table is variable.
            /// BEWARE: this is badly optimised at the moment.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="script">This function must return the list with entries. The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public TwoColumnActive(Transform parent, Func<List<Tuple<TextRef, TextRef>>> script, int width, int fontSize, TextRef title) :
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
                info = new List<Tuple<TextRef, TextRef>>();
                script = null;
            }
        }

        class MultiColumnPassive : InfoTable
        {
            List<List<TextRef>> info;
            int colms;
            private List<TextRef> headers;

            /// <summary>
            /// Use this constructor if you want to make a 2 column table where the number of elements is fixed
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="info">The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public MultiColumnPassive(Transform parent, List<List<TextRef>> info, int width, int fontSize, TextRef title, List<TextRef> headers) :
            base(parent, width, fontSize, title)
            {
                if (info == null || info.Count == 0)
                    throw new ArgumentException("The info of this infotable is empty. Please don't do this to me.");
                colms = info[0].Count;
                for (int i = 1; i < info.Count; i++)
                    if (info[i].Count != colms)
                        throw new ArgumentException("The info of this infotable has an inconsistant number of colums.");
                this.info = info;
                this.headers = headers;
                AddHeaders();
                Redraw();
            }

            public override void Redraw()
            {
                BaseRedrawMulticolumn(info, colms);
            }

            public new void SetInfo(List<List<TextRef>> newInfo)
            {
                info = newInfo;
                AddHeaders();
            }

            public new void AddInfo(List<TextRef> newInfo)
            {
                info.Add(newInfo);
            }

            public override void ResetInfo()
            {
                info = new List<List<TextRef>>();
                AddHeaders();
            }

            private void AddHeaders()
            {
                if (headers != null && headers.Count == colms) {
                    List<TextRef> blank = new List<TextRef>() { TextRef.Create("") };
                    info.Insert(0, blank.Concat(headers).ToList());
                }
                else if (headers != null && headers.Count == colms + 1)
                    info.Insert(0, headers);
                else if (headers != null)
                    throw new ArgumentException("The headers of this infotable do not have a valid count. Use 'number of colums' or 'number of colums + 1'");
            }
        }

        class MultiColumnActive : InfoTable
        {
            List<List<TextRef>> info;
            Func<List<List<TextRef>>> script;
            List<TextRef> headers;
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
            public MultiColumnActive(Transform parent, Func<List<List<TextRef>>> script, int width, int fontSize, TextRef title, List<TextRef> headers) :
            base(parent, width, fontSize, title)
            {
                info = null;
                ActiveInfoTable ait = go.AddComponent<ActiveInfoTable>();
                ait.parent = this;
                this.script = script;
                this.headers = headers;
                Redraw();
            }

            public override void Redraw()
            {
                List<List<TextRef>> newinfo = script();
                if (newinfo == null || newinfo.Count == 0)
                    throw new ArgumentException("The info of this infotable is empty. Please don't do this to me.");
                colms = newinfo[0].Count;
                for (int i = 1; i < newinfo.Count; i++)
                    if (newinfo[i].Count != colms)
                        throw new ArgumentException("The info of this infotable has an inconsistant number of colums.");
                bool isEqual = true;
                if (info == null || newinfo == null) isEqual = false;
                else if (info.Count != newinfo.Count) isEqual = false;
                else
                    for (int i = 0; i < info.Count; i++)
                    {
                        if ((string)info[i][0] != newinfo[i][0]) { isEqual = false; break; }
                    }
                if (isEqual == false)
                {
                    info = newinfo;
                    AddHeaders();
                    BaseRedrawMulticolumn(info, colms);
                }
            }

            public override void ResetInfo()
            {
                info = new List<List<TextRef>>();
                script = null;
            }

            private void AddHeaders()
            {
                if (headers != null && headers.Count == colms)
                {
                    List<TextRef> blank = new List<TextRef>() { TextRef.Create("") };
                    info.Insert(0, blank.Concat(headers).ToList());
                }
                else if (headers != null && headers.Count == colms + 1)
                    info.Insert(0, headers);
                else if (headers != null)
                    throw new ArgumentException("The headers of this infotable do not have a valid count. Use 'number of colums' or 'number of colums + 1'");
            }
        }
    }
}
