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

            TextBox text = new TextBox(close.transform, TextRef.Create("X", false), 8,TextAnchor.MiddleCenter);
            
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
            TextBox title = new TextBox(go.transform, TextRef.Create(() => p), 20, TextAnchor.UpperLeft, Data.Graphics.Color_.text);
            title.transform.anchoredPosition = new Vector2(20, -20);

            List<List<TextRef>> infoData = new List<List<TextRef>>();
            if (p.type == Bodies.Planet.Type.Chunk || p.type == Bodies.Planet.Type.Terrestial_planet)
                if (p.IsPopulated == false)
                    infoData = new List<List<TextRef>>() {
                    new List<TextRef>(){ TextRef.Create("Mass"),            TextRef.Create(()=>p.Mass) },
                    new List<TextRef>(){ TextRef.Create("Temperature"),     TextRef.Create(()=>p.SurfaceTemperature) },
                    new List<TextRef>(){ TextRef.Create("Pressure"),        TextRef.Create(()=>p.PressureAtSeaLevel) },
                    new List<TextRef>(){ TextRef.Create("Day (hours)"),     TextRef.Create(()=>p.SolarDay) },
                    new List<TextRef>(){ TextRef.Create("Year (24h days)"), TextRef.Create(()=>p.OrbElements.T.TotalDays) },
                    new List<TextRef>(){ TextRef.Create("Semi-Major Axis"), TextRef.Create(()=>p.OrbElements.SMA) }
                    };
                else
                    infoData = new List<List<TextRef>>() {
                    new List<TextRef>(){ TextRef.Create("Mass"),            TextRef.Create(()=>p.Mass)},
                    new List<TextRef>(){ TextRef.Create("Temperature"),     TextRef.Create(()=>p.SurfaceTemperature)},
                    new List<TextRef>(){ TextRef.Create("Pressure"),        TextRef.Create(()=>p.PressureAtSeaLevel)},
                    new List<TextRef>(){ TextRef.Create("Day (hours)"),     TextRef.Create(()=>p.SolarDay)},
                    new List<TextRef>(){ TextRef.Create("Year (24h days)"), TextRef.Create(()=>p.OrbElements.T.TotalDays)},
                    new List<TextRef>(){ TextRef.Create("Semi-Major Axis"), TextRef.Create(()=>p.OrbElements.SMA)},
                    new List<TextRef>(){ TextRef.Create("Total population"),TextRef.Create(()=>p.PopulationCount) }
                    };
            else if (p.type == Bodies.Planet.Type.Gas_Giant || p.type == Bodies.Planet.Type.Superjovian)
                infoData = new List<List<TextRef>>() {
                    new List<TextRef>(){ TextRef.Create("Mass"),            TextRef.Create(()=>p.Mass)},
                    new List<TextRef>(){ TextRef.Create("Temperature"),     TextRef.Create(()=>p.BaseTemperature)},
                    new List<TextRef>(){ TextRef.Create("Day (hours)"),     TextRef.Create(()=>p.SolarDay)},
                    new List<TextRef>(){ TextRef.Create("Year (24h days)"), TextRef.Create(()=>p.OrbElements.T.TotalDays)},
                    new List<TextRef>(){ TextRef.Create("Semi-Major Axis"), TextRef.Create(()=>p.OrbElements.SMA)}
                };
            InfoTable info = InfoTable.Create(go.transform, infoData);
        }

        public static void DisplayStar(Star s)
        {
            Clear();
            OpenInspector();
            TextBox title = new TextBox(go.transform, TextRef.Create(() => s), 20, TextAnchor.UpperLeft, Data.Graphics.Color_.text);
            title.transform.anchoredPosition = new Vector2(20, -20);

            List<List<TextRef>> infoData = new List<List<TextRef>>() {
                    new List<TextRef>(){ TextRef.Create("Mass"),            TextRef.Create(()=>s.Mass)},
                    new List<TextRef>(){ TextRef.Create("Temperature"),     TextRef.Create(()=>s.Temperature)},
                    new List<TextRef>(){ TextRef.Create("Radius"),          TextRef.Create(()=>s.Radius)},
                    new List<TextRef>(){ TextRef.Create("Luminocity"),      TextRef.Create(()=>s.Luminosity)},
                    new List<TextRef>(){ TextRef.Create("Orbit (24h days)"),TextRef.Create(()=>s.OrbElements.T.TotalDays)},
                    new List<TextRef>(){ TextRef.Create("Semi-Major Axis"), TextRef.Create(()=>s.OrbElements.SMA) }
            };
            InfoTable info = InfoTable.Create(go.transform, infoData);
        }

        internal static void DisplaySystem(StarSystem sys)
        {
            Clear();
            OpenInspector();
            TextBox title = new TextBox(go.transform, TextRef.Create(() => sys), 20, TextAnchor.UpperLeft, Data.Graphics.Color_.text);
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
            TextBox text = new TextBox(buttonView.transform, TextRef.Create("Go To"), 8, TextAnchor.MiddleCenter);
            buttonView.AddComponent<Button>().onClick.AddListener(() =>
            {
                DisplayManager.TheOne.DisplaySystem(sys);
                DisplayManager.TheOne.SetView(true);
                CloseInspector();
            });

            List<List<TextRef>> infoData = new List<List<TextRef>>() {
                    new List<TextRef>(){ TextRef.Create("Primary"),     TextRef.Create(()=>sys.Primary)},
                    new List<TextRef>(){ TextRef.Create("# Stars"),     TextRef.Create(()=>sys.Tertiary == null? sys.Secondary == null? 1:2:3)},
                    new List<TextRef>(){ TextRef.Create("# planets"),   TextRef.Create(()=>sys.Planets.Count)},
                    new List<TextRef>(){ TextRef.Create("# moons"),     TextRef.Create(()=>sys.Planets.Sum(p => p.moons.Count))}
            };
            InfoTable info = InfoTable.Create(go.transform, infoData);
        }
    }
}
