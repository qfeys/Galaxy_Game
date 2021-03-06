﻿using System;
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
        protected List<TextRef> headers;
        bool highlightLines = true;
        GameObject highlightedLine;
        List<Action> highlightChangeCallbacks;
        
        /// <summary>
        /// Use this constructor if you want to make a multi column table where the number of elements is fixed
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="info">The tuples in this list are the entries. The entries of the second List should return an object on which ToString() will be called.</param>
        /// <param name="width"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        static public InfoTable Create(Transform parent, List<List<TextRef>> info, int width = 200, List<TextRef> headers = null, int fontSize = 12, TextRef title = null)
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
        static public InfoTable Create(Transform parent, Func<List<List<TextRef>>> script, int width = 200, List<TextRef> headers = null, int fontSize = 12, TextRef title = null)
        {
            return new MultiColumnActive(parent, script, width, fontSize, title, headers);
        }

        /// <summary>
        /// 
        /// Use this constructor if you want to make a multi column table where the number of elements is fixed
        /// and where the data in the table is remebered, for interactable tables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="dataList">This is the list with the data in the table</param>
        /// <param name="lineScript">This function transforms the data entries into a line of TextRefs</param>
        /// <param name="width"></param>
        /// <param name="headers"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static public InfoTable Create<T>(Transform parent, List<T> dataList, Func<T, List<TextRef>> lineScript, int width = 200, List<TextRef> headers = null, int fontSize = 12, TextRef title = null)
        {
            return new MultiColumnPassiveMemory<T>(parent, dataList, lineScript, width, fontSize, title, headers);
        }

        /// <summary>
        /// 
        /// Use this constructor if you want to make a multi column table where the number of elements is variable
        /// and where the data in the table is remebered, for interactable tables
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="listScript">This script creates the data in the table</param>
        /// <param name="lineScript">This function transforms the data entries into a line of TextRefs</param>
        /// <param name="width"></param>
        /// <param name="headers"></param>
        /// <param name="fontSize"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static public InfoTable Create<T>(Transform parent, Func<List<T>> listScript, Func<T, List<TextRef>> lineScript, int width = 200, List<TextRef> headers = null, int fontSize = 12, TextRef title = null)
        {
            return new MultiColumnActiveMemory<T>(parent, listScript, lineScript, width, fontSize, title, headers);
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
            if (highlightLines)
            {
                Button but = line.AddComponent<Button>();
                line.AddComponent<Image>().color = new Color(0, 0, 0, 0);
                but.transition = Selectable.Transition.None;
                but.onClick.AddListener(() => SetHighlight(line));
            }
            return line;
        }

        private void SetHighlight(GameObject line)
        {
            if (headers != null && line.transform.GetSiblingIndex() == 0)
                return;
            ColorLine(highlightedLine, false);
            highlightedLine = line;
            ColorLine(line, true);
            if (highlightChangeCallbacks != null)
                highlightChangeCallbacks.ForEach(hcc => hcc());
        }

        public void AddHighlightCallback(Action a)
        {
            if (highlightChangeCallbacks == null)
                highlightChangeCallbacks = new List<Action>();
            highlightChangeCallbacks.Add(a);
        }

        private void ColorLine(GameObject line, bool active)
        {
            if (line == null) return;
            foreach(TextBox.TextBoxScript tbs in line.transform.GetComponentsInChildren<TextBox.TextBoxScript>(true))
            {
                if (active)
                    tbs.parent.SetColor(Data.Graphics.Color_.activeText);
                else
                    tbs.parent.SetColor(Data.Graphics.Color_.text);
            }
        }

        public T RetrieveHighlight<T>()
        {
            if (GetType() == typeof(MultiColumnPassiveMemory<T>))
                return ((MultiColumnPassiveMemory<T>)this).RetrieveHighlight();
            else if (GetType() == typeof(MultiColumnActiveMemory<T>))
                return ((MultiColumnActiveMemory<T>)this).RetrieveHighlight();
            else
                throw new Exception("You can only retrieve the highlihgt from memory tables with the correct type. This is a " + GetType().ToString());
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

        protected void BaseRedraw(List<List<TextRef>> info, int numberOfCol)
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
        
        class MultiColumnPassive : InfoTable
        {
            List<List<TextRef>> info;
            int colms;

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
                BaseRedraw(info, colms);
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

        class MultiColumnPassiveMemory<T> : InfoTable
        {
            protected List<T> dataList;
            List<List<TextRef>> info;
            int colms;

            /// <summary>
            /// Use this constructor if you want to make a 2 column table where the number of elements is fixed
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="info">The tuples in this list are the entries. The second item is the ToString() of whatever object is returned by the second function.</param>
            /// <param name="width"></param>
            /// <param name="fontSize"></param>
            /// <param name="title"></param>
            public MultiColumnPassiveMemory(Transform parent, List<T> dataList, Func<T, List<TextRef>> lineScript, int width, int fontSize, TextRef title, List<TextRef> headers) :
            base(parent, width, fontSize, title)
            {
                if (dataList == null || dataList.Count == 0)
                    throw new ArgumentException("The info of this infotable is empty. Please don't do this to me.");
                colms = info[0].Count;
                this.dataList = dataList;
                info = dataList.ConvertAll(line => lineScript(line));
                this.headers = headers;
                AddHeaders();
                Redraw();
            }

            public override void Redraw()
            {
                BaseRedraw(info, colms);
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

            public T RetrieveHighlight()
            {
                return dataList[highlightedLine.transform.GetSiblingIndex() - (headers == null ? 0 : 1)];
            }
        }

        class MultiColumnActive : InfoTable
        {
            List<List<TextRef>> info;
            Func<List<List<TextRef>>> script;
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
                    BaseRedraw(info, colms);
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

        class MultiColumnActiveMemory<T> : InfoTable
        {
            List<T> dataList;
            List<List<TextRef>> info;
            Func<List<T>> listScript;
            Func<T, List<TextRef>> lineScript;
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
            public MultiColumnActiveMemory(Transform parent, Func<List<T>> listScript,Func<T, List<TextRef>> lineScript, int width, int fontSize, TextRef title, List<TextRef> headers) :
            base(parent, width, fontSize, title)
            {
                info = null;
                ActiveInfoTable ait = go.AddComponent<ActiveInfoTable>();
                ait.parent = this;
                this.listScript = listScript;
                this.lineScript = lineScript;
                this.headers = headers;
                Redraw();
            }

            public override void Redraw()
            {
                List<T> newList = listScript();
                if (newList == null || (newList.Count == 0 && headers == null))
                    throw new ArgumentException("The info of this infotable is empty. Please don't do this to me.");
                bool dataEqual = true;
                if (dataList == null || newList == null) dataEqual = false;
                else if (dataList.Count != newList.Count) dataEqual = false;

                if (dataEqual == false)
                {
                    dataList = new List<T>(newList);
                    info = newList.ConvertAll(line => lineScript(line));
                    colms = info.Count != 0 ? info[0].Count : colms;
                    AddHeaders();
                    if (colms == 0)
                        colms = info[0].Count;
                    BaseRedraw(info, colms);
                }
            }

            public override void ResetInfo()
            {
                info = new List<List<TextRef>>();
                listScript = null;
                lineScript = null;
            }

            private void AddHeaders()
            {
                if (headers != null && (headers.Count == colms || colms == 0))
                    info.Insert(0, headers);
                else if (headers != null)
                    throw new ArgumentException("The headers of this infotable do not have a valid count. Use 'number of colums' or 'number of colums + 1'");
            }

            public T RetrieveHighlight()
            {
                if (highlightLines == false)
                    throw new InvalidOperationException("This list does not have highlights.");
                if (highlightedLine != null)
                    return dataList[highlightedLine.transform.GetSiblingIndex() - (headers == null ? 0 : 1)];
                return default(T);
            }
        }
    }
}
