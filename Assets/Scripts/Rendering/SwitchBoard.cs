using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class SwitchBoard
    {
        static GameObject go;

        internal static void Create(GameObject canvas)
        {
            go = new GameObject("SwitchBoard", typeof(RectTransform));
            go.transform.SetParent(canvas.transform);
            RectTransform tr = (RectTransform)go.transform;
            tr.sizeDelta = new Vector2(200, 50);
            tr.anchorMin = new Vector2(1, 0);
            tr.anchorMax = new Vector2(1, 0);
            tr.pivot = new Vector2(1, 0);
            tr.anchoredPosition = new Vector2(0, 0);
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;

            HorizontalLayoutGroup hzlg = go.AddComponent<HorizontalLayoutGroup>();
            hzlg.childForceExpandWidth = true;
            hzlg.childForceExpandHeight = true;
            hzlg.padding = new RectOffset(5, 5, 5, 5);
            hzlg.spacing = 5;
            hzlg.childAlignment = TextAnchor.LowerLeft;
            hzlg.childForceExpandWidth = false;
            AddButton("Reset View", "rst", () => DisplayManager.TheOne.ResetView());
            AddButton("change view", "sys", () => DisplayManager.TheOne.ToggleView());

        }

        private static void AddButton(string name, string text, Action act)
        {
            GameObject butGo = new GameObject(name, typeof(RectTransform));
            butGo.transform.SetParent(go.transform);
            RectTransform tr = (RectTransform)butGo.transform;
            tr.sizeDelta = new Vector2(50, 50);
            tr.anchorMin = new Vector2(1, 0);
            tr.anchorMax = new Vector2(1, 0);
            tr.pivot = new Vector2(1, 0);
            var lyel_brv = butGo.AddComponent<LayoutElement>();
            lyel_brv.preferredWidth = 50;
            Image img = butGo.AddComponent<Image>();
            img.sprite = Data.Graphics.GetSprite("tab_image_low");
            img.type = Image.Type.Sliced;
            Button but = butGo.AddComponent<Button>();
            but.onClick.AddListener(()=>act());

            TextBox t = new TextBox(butGo.transform, TextRef.Create(text, text + "_text"), 14, TextAnchor.MiddleCenter);
        }
    }
}
