using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    class OverviewWindow
    {
        private TabbedWindow window;

        public OverviewWindow(GameObject canvas)
        {
            window = new TabbedWindow(canvas.transform, new Vector2(600, 400),
                new List<Tuple<string, GameObject>>() {
                    new Tuple<string, GameObject>("Empire", EmpireWindow),
                    new Tuple<string, GameObject>("Populations", PopulationsWindow),
                    new Tuple<string, GameObject>("Technology", TechnologyWindow)
                });
            RectTransform tr = window.gameobject.transform as RectTransform;
            tr.pivot = new Vector2(0, 1);
            tr.anchorMin = new Vector2(0, 1);
            tr.anchorMax = new Vector2(0, 1);
            tr.anchoredPosition = new Vector2(0, 0);
            Image im = window.gameobject.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("standard_window");
            im.type = Image.Type.Sliced;
        }

        GameObject EmpireWindow
        {
            get
            {
                GameObject go = new GameObject("Empire Window", typeof(RectTransform));
                TextBox title = new TextBox(go.transform, "empire_window_title", null, 24, TextAnchor.MiddleCenter);
                Center(title.gameObject);
                ((RectTransform)title.gameObject.transform).sizeDelta = new Vector2(200, 36);
                InfoTable table = new InfoTable(go.transform, new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("population", ()=>Simulation.God.PlayerEmpire.Population),
                    new Tuple<string, Func<object>>("Wealth", () => Simulation.God.PlayerEmpire.Wealth)
                }, 200);
                Center(table.gameObject, new Vector2(100, -100));
                return go;
            }
        }

        public GameObject PopulationsWindow
        {
            get
            {
                GameObject go = new GameObject("Populations Window", typeof(RectTransform));
                TextBox title = new TextBox(go.transform, "populations_window_title", null, 24, TextAnchor.MiddleCenter);
                Center(title.gameObject);
                ((RectTransform)title.gameObject.transform).sizeDelta = new Vector2(200, 36);

                InfoTable tablePops = new InfoTable(go.transform, () =>
                {
                    List<Tuple<string, Func<object>>> list = new List<Tuple<string, Func<object>>>();
                    foreach (Empires.Population pop in Simulation.God.PlayerEmpire.Populations)
                    {
                        list.Add(new Tuple<string, Func<object>>(pop.ToString(), () => pop.Count));
                    }
                    return list;
                }, 200);
                Center(tablePops.gameObject, new Vector2(-100, -100));

                return go;
            }
        }

        public GameObject TechnologyWindow
        {
            get
            {
                GameObject go = new GameObject("Technology Window", typeof(RectTransform));
                TextBox title = new TextBox(go.transform, "technology_window_title", null, 24, TextAnchor.MiddleCenter);
                Center(title.gameObject);
                ((RectTransform)title.gameObject.transform).sizeDelta = new Vector2(200, 36);
                
                InfoTable tableSectors = new InfoTable(go.transform, () =>
                {
                    List<Tuple<string, Func<object>>> list = new List<Tuple<string, Func<object>>>();
                    foreach (KeyValuePair<Empires.Technology.Technology.Sector, double> kvp in Simulation.God.PlayerEmpire.Academy.Funding)
                    {
                        list.Add(new Tuple<string, Func<object>>(kvp.Key.ToString(), () => kvp.Value));
                    }
                    return list;
                }, 200);
                Center(tableSectors.gameObject, new Vector2(-100, -100));


                InfoTable tableTechs = new InfoTable(go.transform, () =>
                {
                    List<Tuple<string, Func<object>>> list = new List<Tuple<string, Func<object>>>();
                    foreach (var tech in Simulation.God.PlayerEmpire.Academy.Unlocks)
                    {
                        list.Add(new Tuple<string, Func<object>>(tech.Name, () => tech.Knowledge.ToString() + "/" + tech.Understanding));
                    }
                    return list;
                }, 200);
                Center(tableTechs.gameObject, new Vector2(100, -100));
                return go;
            }
        }

        private static void Center(GameObject go, Vector2? offset = null)
        {
            ((RectTransform)go.transform).anchorMin = new Vector2(0.5f, 1);
            ((RectTransform)go.transform).anchorMax = new Vector2(0.5f, 1);
            ((RectTransform)go.transform).pivot = new Vector2(0.5f, 1);
            ((RectTransform)go.transform).anchoredPosition = offset ?? Vector2.zero;
        }

    }
}
