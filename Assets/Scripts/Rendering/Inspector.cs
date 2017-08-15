using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class Inspector
    {
        static GameObject go;

        enum Modes { EmptyPlanet, PopulatedPlanet, Star}

        internal static void Create(GameObject canvas)
        {
            go = new GameObject("Inspector", typeof(RectTransform));
            go.transform.SetParent(canvas.transform);
            RectTransform tr = (RectTransform)go.transform;
            tr.sizeDelta = new Vector2(400, 400);
            tr.anchorMin = new Vector2(0, 0);
            tr.anchorMax = new Vector2(0, 0);
            tr.pivot = new Vector2(0, 0);
            tr.anchoredPosition = new Vector2(0, 0);
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;

            GameObject close = new GameObject("Tab", typeof(RectTransform));
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

            TextBox text = new TextBox(close.transform, "X", null, 8,TextAnchor.MiddleCenter);
            
            close.AddComponent<Button>().onClick.AddListener(() => { CloseInspector(); });

        }

        private static void OpenInspector()
        {
            go.SetActive(true);
        }

        private static void CloseInspector()
        {
            go.SetActive(false);
        }

        public static void DisplayPlanet(Bodies.Planet p) // TODO: needs fixing because of transition of orbitals to planets
        {
            TextBox title = new TextBox(go.transform, () => p, null, 24, TextAnchor.MiddleCenter);
            //transform.GetChild(0).GetComponent<Text>().text = o.ToString();
            //string[] info = string.Join(";", new[] { o.Information(), "####", "####" }).Split(';');
            //int i;
            //for(i = 0; i<info.Length/2; i++)
            //{
            //    if (transform.GetChild(1).childCount <= i)
            //    {
            //        Transform tr = Instantiate(transform.GetChild(1).GetChild(0).gameObject).transform;
            //        tr.SetParent(transform.GetChild(1));
            //        tr.SetAsLastSibling();
            //    }
            //    transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<Text>().text = info[2 * i];
            //    transform.GetChild(1).GetChild(i).GetChild(1).GetComponent<Text>().text = info[2 * i + 1];
            //    transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
            //}
            //while(transform.GetChild(1).childCount < i)
            //{
            //    transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
            //    i++;
            //}
            
            //gameObject.SetActive(true);
        }
    }
}
