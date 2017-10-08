using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Empires;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class PopDetails
    {
        static GameObject go;
        static Population activePopulation;

        internal static void Create(GameObject canvas)
        {
            go = new GameObject("PopDetails", typeof(RectTransform));
            go.transform.SetParent(canvas.transform);
            RectTransform tr = (RectTransform)go.transform;
            tr.sizeDelta = new Vector2(800, 600);
            tr.anchorMin = new Vector2(0, 1);
            tr.anchorMax = new Vector2(0, 1);
            tr.pivot = new Vector2(0, 1);
            tr.anchoredPosition = new Vector2(100, -100);
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;
            go.SetActive(false);

            activePopulation = Population.NullPop;

            // close button
            {
                GameObject close = new GameObject("Close", typeof(RectTransform));
                close.transform.SetParent(go.transform);
                RectTransform trcl = (RectTransform)close.transform;
                trcl.sizeDelta = new Vector2(15, 15);
                trcl.anchorMin = new Vector2(1, 1);
                trcl.anchorMax = new Vector2(1, 1);
                trcl.pivot = new Vector2(1, 1);
                trcl.anchoredPosition = new Vector2(-10, -10);
                Image img = close.AddComponent<Image>();
                img.sprite = Data.Graphics.GetSprite("tab_image_low");
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;
                TextBox text = new TextBox(close.transform, TextRef.Create("X", false), 8, TextAnchor.MiddleCenter);
                close.AddComponent<Button>().onClick.AddListener(() => { CloseDetails(); });
            }

            // Title
            {
                GameObject titleGo = new GameObject("Title", typeof(RectTransform));
                titleGo.transform.SetParent(go.transform);
                RectTransform trTtl = (RectTransform)titleGo.transform;
                trTtl.sizeDelta = new Vector2(0, 26);
                trTtl.anchorMin = new Vector2(0, 1);
                trTtl.anchorMax = new Vector2(1, 1);
                trTtl.pivot = new Vector2(0.5f, 1);
                trTtl.anchoredPosition = new Vector2(-10, -10);
                TextBox text = new TextBox(trTtl, TextRef.Create(() => activePopulation.Name), 24, TextAnchor.UpperCenter);
            }
        }

        private static void OpenDetails()
        {
            go.SetActive(true);
        }

        public static void OpenDetails(Population pop)
        {
            go.SetActive(true);
            activePopulation = pop;
        }

        private static void CloseDetails()
        {
            go.SetActive(false);
        }
    }
}
