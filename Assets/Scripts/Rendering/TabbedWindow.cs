using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    class TabbedWindow
    {
        bool canBeMinimised;

        List<Tuple<GameObject,GameObject>> windows;
        bool isMinimised = false;
        Vector2 size;
        int tabFontSize;

        GameObject go;
        public GameObject gameobject { get { return go; } }
        public RectTransform transform { get { return go.transform as RectTransform; } }

        public TabbedWindow(Transform parent, Vector2 size, List<Tuple<string,GameObject>> tabs, int tabFontSize = 12, bool canBeMinimised = true)
        {
            go = new GameObject("TabWindow", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            this.size = size;
            this.canBeMinimised = canBeMinimised;
            this.tabFontSize = tabFontSize;
            ((RectTransform)go.transform).sizeDelta = size;
            var VLayGr = go.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
            GameObject buttonLine = new GameObject("Tab Line", typeof(RectTransform));
            buttonLine.transform.SetParent(go.transform, false);
            var LayEl = buttonLine.AddComponent<LayoutElement>();
            LayEl.minHeight = tabFontSize * 3 / 2 + 5;
            LayEl.flexibleHeight = 0;
            LayEl.flexibleWidth = 1;
            var HLayGr = buttonLine.AddComponent<HorizontalLayoutGroup>();
            HLayGr.childForceExpandHeight = false;
            HLayGr.childForceExpandWidth = false;
            HLayGr.padding = new RectOffset(5, 0, 3, 2);
            GameObject mainWindow = new GameObject("Main Window", typeof(RectTransform));
            mainWindow.transform.SetParent(go.transform, false);
            mainWindow.AddComponent<LayoutElement>().flexibleHeight = 1;
            mainWindow.AddComponent<HorizontalLayoutGroup>();   // used to strech the underlying windows

            windows = new List<Tuple<GameObject, GameObject>>();
            for (int i = 0; i < tabs.Count; i++)
            {
                GameObject tab = new GameObject("Tab", typeof(RectTransform));
                tab.transform.SetParent(buttonLine.transform);
                Image img = tab.AddComponent<Image>();
                img.sprite = Data.Graphics.GetSprite("tab_image_low");
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                TextBox text = new TextBox(tab.transform, tabs[i].Item1,null,tabFontSize);
                text.transform.anchoredPosition += new Vector2(5, 0);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = text.Width + tabFontSize;

                int j = i;
                if (canBeMinimised)
                {
                    tab.AddComponent<Button>().onClick.AddListener(() => { MaximiseWindow(); SetTab(j); });
                }
                else
                {
                    tab.AddComponent<Button>().onClick.AddListener(() => SetTab(j));
                }

                GameObject window = tabs[i].Item2;
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(new Tuple<GameObject,GameObject>(tab,window));
            }
            if (canBeMinimised)
            {
                GameObject tab = new GameObject("Tab", typeof(RectTransform));
                tab.transform.SetParent(buttonLine.transform);
                Image img = tab.AddComponent<Image>();
                img.sprite = Data.Graphics.GetSprite("tab_image_low");
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                TextBox text = new TextBox(tab.transform, "X", null, tabFontSize);
                ((RectTransform)text.gameObject.transform).anchoredPosition += new Vector2(5, 0);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = text.gameObject.GetComponent<Text>().preferredWidth + tabFontSize;
                int a = windows.Count;
                tab.AddComponent<Button>().onClick.AddListener(() => { SetTab(a); MinimiseWindow(); });

                GameObject window = new GameObject("Null Window", typeof(RectTransform));
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(new Tuple<GameObject, GameObject>(tab, window));
            }
            if (canBeMinimised) MinimiseWindow();
            else SetTab(0);
        }

        /// <summary>
        /// Sets the tab of the window on the page with the given rank. If the window is minimised, 
        /// this will also maximise the window.
        /// </summary>
        /// <param name="n"></param>
        public void SetTab(int n)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if(i != n)
                {
                    if (windows[i].Item2.activeSelf)
                    {
                        windows[i].Item2.SetActive(false);
                        windows[i].Item1.GetComponent<Image>().sprite = Data.Graphics.GetSprite("tab_image_low");
                    }
                }
            }
            if (windows[n].Item2.activeSelf == false)
            {
                windows[n].Item2.SetActive(true);
                windows[n].Item1.GetComponent<Image>().sprite = Data.Graphics.GetSprite("tab_image_high");
            }

        }

        /// <summary>
        /// This will minimise the window
        /// </summary>
        public void MinimiseWindow()
        {
            if(isMinimised == false)
            {
                ((RectTransform)go.transform).sizeDelta = new Vector2(
                    ((RectTransform)go.transform).rect.width,
                     tabFontSize * 3 / 2.0f + 6);
                isMinimised = true;
                SetTab(windows.Count - 1);
            }
        }

        private void MaximiseWindow()
        {
            if (isMinimised == true)
            {
                ((RectTransform)go.transform).sizeDelta = size;
                isMinimised = false;
            }
        }
    }
}
