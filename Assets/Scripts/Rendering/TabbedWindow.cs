using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    class TabbedWindow : MonoBehaviour
    {
        // must contain a button, a layoutElement and a text component
        public bool canBeMinimised;
        public Sprite tabImageLow;
        public Sprite tabImageHigh;
        public GameObject exampleText;
        public List<string> tabNames;
        public List<GameObject> tabContent;

        List<GameObject> buttons;
        List<GameObject> windows;
        float standardHeight;
        bool isMinimised = false;
        

        public void Awake()
        {
            if (tabNames.Count != tabContent.Count)
                throw new ArgumentException("You gave " + tabNames.Count + " tab names and " + tabContent.Count + " tab contents.");
            //if (tabPrefab.GetComponentInChildren<Text>() == null)
            //    throw new ArgumentException("You did not include a Text component in your tab prefab.");
            //if (tabPrefab.GetComponentInChildren<Button>() == null)
            //    throw new ArgumentException("You did not include a Button component in your tab prefab.");
            //if (tabPrefab.GetComponentInChildren<LayoutElement>() == null)
            //    throw new ArgumentException("You did not include a LayoutElement component in your tab prefab.");
        }

        public void Start()
        {
            standardHeight = ((RectTransform)gameObject.transform).rect.height;
            Debug.Log(standardHeight);
            var VLayGr = gameObject.GetComponent<VerticalLayoutGroup>();
            if(VLayGr == null)
                VLayGr = gameObject.AddComponent<VerticalLayoutGroup>();
            VLayGr.childForceExpandHeight = false;
            VLayGr.childForceExpandWidth = false;
            GameObject buttonLine = new GameObject("Tab Line");
            buttonLine.transform.parent = transform;
            var LOE = buttonLine.AddComponent<LayoutElement>();
            LOE.minHeight = exampleText.GetComponent<Text>().fontSize * 3 / 2;
            LOE.flexibleHeight = 0;
            LOE.flexibleWidth = 1;
            var HLayGr = buttonLine.AddComponent<HorizontalLayoutGroup>();
            HLayGr.childForceExpandHeight = false;
            HLayGr.childForceExpandWidth = false;
            GameObject mainWindow = new GameObject("Main Window");
            mainWindow.transform.parent = transform;
            mainWindow.AddComponent<LayoutElement>().flexibleHeight = 1;
            mainWindow.AddComponent<HorizontalLayoutGroup>();

            buttons = new List<GameObject>();
            windows = new List<GameObject>();
            for (int i = 0; i < tabNames.Count; i++)
            {
                GameObject tab = new GameObject("Tab");
                tab.transform.SetParent(buttonLine.transform);
                Image img = tab.AddComponent<Image>();
                img.sprite = tabImageLow;
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                GameObject text = Instantiate(exampleText);
                text.transform.SetParent(tab.transform,false);
                Text t = text.GetComponent<Text>();
                t.text = tabNames[i];
                float width = t.preferredWidth + t.fontSize;
                float height = t.fontSize * 3 / 2;

                int j = i;
                if (canBeMinimised)
                {
                    tab.AddComponent<Button>().onClick.AddListener(() => { MaximiseWindow(); SetTab(j); });
                }
                else
                {
                    tab.AddComponent<Button>().onClick.AddListener(() => SetTab(j));
                }
                buttons.Add(tab);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = width;

                GameObject window = Instantiate(tabContent[i]);
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(window);
            }
            if (canBeMinimised)
            {
                GameObject tab = new GameObject("Tab");
                tab.transform.SetParent(buttonLine.transform);
                Image img = tab.AddComponent<Image>();
                img.sprite = tabImageLow;
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                GameObject text = Instantiate(exampleText);
                text.transform.SetParent(tab.transform, false);
                Text t = text.GetComponent<Text>();
                t.text = "X";
                float width = t.preferredWidth + t.fontSize;
                float height = t.fontSize * 3 / 2;
                
                tab.AddComponent<Button>().onClick.AddListener(() => { SetTab(tabNames.Count); MinimiseWindow(); });
                buttons.Add(tab);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = width;

                GameObject window = new GameObject();
                window.transform.SetParent(mainWindow.transform);
                window.SetActive(false);
                windows.Add(window);
            }
            SetTab(0);
        }

        private void SetTab(int n)
        {
            for (int i = 0; i < tabNames.Count; i++)
            {
                if(i != n)
                {
                    if (windows[i].activeSelf)
                    {
                        windows[i].SetActive(false);
                        buttons[i].GetComponent<Image>().sprite = tabImageLow;
                    }
                }
            }
            if (windows[n].activeSelf == false)
            {
                windows[n].SetActive(true);
                buttons[n].GetComponent<Image>().sprite = tabImageHigh;
            }

        }

        private void MinimiseWindow()
        {
            if(isMinimised == false)
            {
                ((RectTransform)gameObject.transform).sizeDelta = new Vector2(
                    ((RectTransform)gameObject.transform).rect.width,
                    ((RectTransform)transform.GetChild(0).transform).rect.height);
                isMinimised = true;
            }
        }

        private void MaximiseWindow()
        {
            if (isMinimised == true)
            {
                ((RectTransform)gameObject.transform).sizeDelta = new Vector2(
                    ((RectTransform)gameObject.transform).rect.width,
                    standardHeight);
                isMinimised = false;
            }
        }
    }
}
