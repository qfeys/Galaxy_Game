using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class UI_Window
    {
        public static void TopLeft(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0, 1);
            tr.anchorMax = new Vector2(0, 1);
            tr.pivot = new Vector2(0, 1);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void TopCenter(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0.5f, 1);
            tr.anchorMax = new Vector2(0.5f, 1);
            tr.pivot = new Vector2(0.5f, 1);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void TopRight(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(1, 1);
            tr.anchorMax = new Vector2(1, 1);
            tr.pivot = new Vector2(1, 1);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void TopStretch(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0, 1);
            tr.anchorMax = new Vector2(1, 1);
            tr.pivot = new Vector2(0.5f, 1);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void LeftCenter(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0, 0.5f);
            tr.anchorMax = new Vector2(0, 0.5f);
            tr.pivot = new Vector2(0, 0.5f);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void CenterCenter(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0.5f, 0.5f);
            tr.anchorMax = new Vector2(0.5f, 0.5f);
            tr.pivot = new Vector2(0.5f, 0.5f);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void RightCenter(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(1, 0.5f);
            tr.anchorMax = new Vector2(1, 0.5f);
            tr.pivot = new Vector2(1, 0.5f);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void BottomLeft(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0, 0);
            tr.anchorMax = new Vector2(0, 0);
            tr.pivot = new Vector2(0, 0);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void BottomCenter(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0.5f, 0);
            tr.anchorMax = new Vector2(0.5f, 0);
            tr.pivot = new Vector2(0.5f, 0);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }
        public static void BottomRight(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(1, 0);
            tr.anchorMax = new Vector2(1, 0);
            tr.pivot = new Vector2(1, 0);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }

        public static void CreateCloseButton(GameObject go, Action CloseAction)
        {
            GameObject close = new GameObject("Close", typeof(RectTransform));
            close.transform.SetParent(go.transform);
            RectTransform trcl = (RectTransform)close.transform;
            trcl.sizeDelta = new Vector2(15, 15);
            UI_Window.TopRight(trcl, new Vector2(-10, -10));
            Image img = close.AddComponent<Image>();
            img.sprite = Data.Graphics.GetSprite("tab_image_low");
            img.raycastTarget = true;
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            TextBox text = new TextBox(close.transform, TextRef.Create("X", false), 8, TextAnchor.MiddleCenter);
            close.AddComponent<Button>().onClick.AddListener(() => { CloseAction(); });
        }
    }
}
