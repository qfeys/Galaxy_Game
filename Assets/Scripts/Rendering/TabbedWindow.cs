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
        public bool canBeMinimised;

        List<Tuple<GameObject,GameObject>> windows;
        bool isMinimised = false;
        Vector2 size;

        GameObject go;
        public GameObject gameobject { get { return go; } }

        public TabbedWindow(Transform parent, Vector2 size, List<Tuple<string,GameObject>> tabs, int tabFontSize = 12)
        {
            go = new GameObject("TabWindow", typeof(RectTransform));
            go.transform.parent = parent;
            this.size = size;
            ((RectTransform)go.transform).sizeDelta = size;
            var VLayGr = go.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
            GameObject buttonLine = new GameObject("Tab Line", typeof(RectTransform));
            buttonLine.transform.parent = go.transform;
            var LayEl = buttonLine.AddComponent<LayoutElement>();
            LayEl.minHeight = tabFontSize * 3 / 2;
            LayEl.flexibleHeight = 0;
            LayEl.flexibleWidth = 1;
            var HLayGr = buttonLine.AddComponent<HorizontalLayoutGroup>();
            HLayGr.childForceExpandHeight = false;
            HLayGr.childForceExpandWidth = false;
            GameObject mainWindow = new GameObject("Main Window", typeof(RectTransform));
            mainWindow.transform.parent = go.transform;
            mainWindow.AddComponent<LayoutElement>().flexibleHeight = 1;
            mainWindow.AddComponent<HorizontalLayoutGroup>();

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

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = text.gameObject.GetComponent<Text>().preferredWidth + tabFontSize;

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
                img.sprite = Data.Graphics.GetSprite("tab_low");
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                TextBox text = new TextBox(tab.transform, "X", null, tabFontSize);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = text.gameObject.GetComponent<Text>().preferredWidth + tabFontSize;
                
                tab.AddComponent<Button>().onClick.AddListener(() => { SetTab(windows.Count); MinimiseWindow(); });

                GameObject window = new GameObject("Null Window", typeof(RectTransform));
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(new Tuple<GameObject, GameObject>(tab, window));
            }
            SetTab(0);
        }

        private void SetTab(int n)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                if(i != n)
                {
                    if (windows[i].Item2.activeSelf)
                    {
                        windows[i].Item2.SetActive(false);
                        windows[i].Item1.GetComponent<Image>().sprite = Data.Graphics.GetSprite("tab_low");
                    }
                }
            }
            if (windows[n].Item2.activeSelf == false)
            {
                windows[n].Item2.SetActive(true);
                windows[n].Item1.GetComponent<Image>().sprite = Data.Graphics.GetSprite("tab_high");
            }

        }

        private void MinimiseWindow()
        {
            if(isMinimised == false)
            {
                ((RectTransform)go.transform).sizeDelta = new Vector2(
                    ((RectTransform)go.transform).rect.width,
                    ((RectTransform)go.transform.GetChild(0).transform).rect.height);
                isMinimised = true;
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
