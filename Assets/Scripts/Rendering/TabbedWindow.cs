using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    [ExecuteInEditMode]
    class TabbedWindow : MonoBehaviour
    {
        // must contain a button, a layoutElement and a text component
        public Sprite tabImageLow;
        public Sprite tabImageHigh;
        public GameObject exampleText;
        public List<string> tabNames;
        public List<GameObject> tabContent;
        List<GameObject> buttons;
        List<GameObject> windows;

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
                tab.AddComponent<Button>().onClick.AddListener(() => SetTab(j));
                buttons.Add(tab);

                tab.AddComponent<LayoutElement>().flexibleHeight = 1;
                tab.GetComponent<LayoutElement>().preferredWidth = width;

                GameObject window = Instantiate(tabContent[i]);
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
    }
}
