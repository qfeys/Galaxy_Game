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
            tr.sizeDelta = new Vector2(200, 30);
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

            GameObject butRstVw = new GameObject("Reset View", typeof(RectTransform));
            butRstVw.transform.SetParent(go.transform);
            RectTransform tr_brv = (RectTransform)butRstVw.transform;
            tr_brv.sizeDelta = new Vector2(20, 20);
            tr_brv.anchorMin = new Vector2(1, 0);
            tr_brv.anchorMax = new Vector2(1, 0);
            tr_brv.pivot = new Vector2(1, 0);
            var lyel_brv = butRstVw.AddComponent<LayoutElement>();
            lyel_brv.preferredWidth = 20;
            Image img = butRstVw.AddComponent<Image>();
            img.sprite = Data.Graphics.GetSprite("tab_image_low");
            img.type = Image.Type.Sliced;
            Button but = butRstVw.AddComponent<Button>();
            but.onClick.AddListener(() => SystemRenderer.ResetView());

            TextBox t = new TextBox(butRstVw.transform, "rst", "rst_text", 8, TextAnchor.MiddleCenter);

        }
    }
}
