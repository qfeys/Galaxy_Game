using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Bodies;
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

            TextBox text = new TextBox(close.transform, "X", null, 8,TextAnchor.MiddleCenter);
            
            close.AddComponent<Button>().onClick.AddListener(() => { CloseInspector(); });
            CloseInspector();
        }

        private static void OpenInspector()
        {
            go.SetActive(true);
        }

        private static void CloseInspector()
        {
            go.SetActive(false);
        }

        private static void Clear()
        {
            for (int i = 1; i < go.transform.childCount; i++)
            {
                GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        public static void DisplayPlanet(Bodies.Planet p)
        {
            Clear();
            OpenInspector();
            TextBox title = new TextBox(go.transform, () => p, null, 20, TextAnchor.UpperLeft, Data.Graphics.Color_.text);
            title.transform.anchoredPosition = new Vector2(20, -20);

            InfoTable info = new InfoTable(go.transform, new List<Tuple<string, Func<object>>>());
            if (p.type == Bodies.Planet.Type.Chunk || p.type == Bodies.Planet.Type.Terrestial_planet)
                info.SetInfo(new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("Mass", ()=>p.Mass),
                    new Tuple<string, Func<object>>("Temperature",()=>p.SurfaceTemperature),
                    new Tuple<string, Func<object>>("Pressure",()=>p.PressureAtSeaLevel),
                    new Tuple<string, Func<object>>("Day (hours)",()=>p.SolarDay),
                    new Tuple<string, Func<object>>("Year (24h days)",()=>p.OrbElements.T.TotalDays),
                    new Tuple<string, Func<object>>("Semi-Major Axis",()=>p.OrbElements.SMA)
                });
            else if (p.type == Bodies.Planet.Type.Gas_Giant || p.type == Bodies.Planet.Type.Superjovian)
                info.SetInfo(new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("Mass", ()=>p.Mass),
                    new Tuple<string, Func<object>>("Temperature",()=>p.BaseTemperature),
                    new Tuple<string, Func<object>>("Day (hours)",()=>p.SolarDay),
                    new Tuple<string, Func<object>>("Year (24h days)",()=>p.OrbElements.T.TotalDays),
                    new Tuple<string, Func<object>>("Semi-Major Axis",()=>p.OrbElements.SMA)
                });
            info.Redraw();
        }

        public static void DisplayStar(Star s)
        {
            Clear();
            OpenInspector();
            TextBox title = new TextBox(go.transform, () => s, null, 20, TextAnchor.UpperLeft, Data.Graphics.Color_.text);
            title.transform.anchoredPosition = new Vector2(20, -20);

            InfoTable info = new InfoTable(go.transform, new List<Tuple<string, Func<object>>>());
                info.SetInfo(new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("Mass", ()=>s.Mass),
                    new Tuple<string, Func<object>>("Temperature",()=>s.Temperature),
                    new Tuple<string, Func<object>>("Radius",()=>s.Radius),
                    new Tuple<string, Func<object>>("Luminocity",()=>s.Luminosity),
                    new Tuple<string, Func<object>>("Orbit (24h days)",()=>s.OrbElements.T.TotalDays),
                    new Tuple<string, Func<object>>("Semi-Major Axis",()=>s.OrbElements.SMA)
                });
            info.Redraw();
        }

        internal static void DisplaySystem(StarSystem sys)
        {
            Clear();
            OpenInspector();
            TextBox title = new TextBox(go.transform, () => sys, null, 20, TextAnchor.UpperLeft, Data.Graphics.Color_.text);
            title.transform.anchoredPosition = new Vector2(20, -20);
            
            GameObject buttonView = new GameObject("View System", typeof(RectTransform));
            buttonView.transform.SetParent(go.transform);
            RectTransform trcl = (RectTransform)buttonView.transform;
            trcl.sizeDelta = new Vector2(60, 30);
            trcl.anchorMin = new Vector2(0, 1);
            trcl.anchorMax = new Vector2(0, 1);
            trcl.pivot = new Vector2(0, 1);
            trcl.anchoredPosition = new Vector2(20, -60);
            Image img = buttonView.AddComponent<Image>();
            img.sprite = Data.Graphics.GetSprite("tab_image_low");
            img.raycastTarget = true;
            img.type = Image.Type.Sliced;
            img.fillCenter = true;
            TextBox text = new TextBox(buttonView.transform, "Go To", null, 8, TextAnchor.MiddleCenter);
            buttonView.AddComponent<Button>().onClick.AddListener(() =>
            {
                DisplayManager.TheOne.DisplaySystem(sys);
                DisplayManager.TheOne.SetView(true);
                CloseInspector();
            });

            InfoTable info = new InfoTable(go.transform, new List<Tuple<string, Func<object>>>());
            info.SetInfo(new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("Primary", ()=>sys.Primary),
                    new Tuple<string, Func<object>>("# Stars",()=>sys.Tertiary == null? sys.Secondary == null? 1:2:3),
                    new Tuple<string, Func<object>>("# planets",()=>sys.Planets.Count),
                    new Tuple<string, Func<object>>("# moons",()=>sys.Planets.Sum(p => p.moons.Count))
            });
            info.Redraw();
        }
    }
}
