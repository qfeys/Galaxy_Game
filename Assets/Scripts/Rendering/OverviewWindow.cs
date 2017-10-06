using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class OverviewWindow
    {
        static TabbedWindow window;

        internal static void Create(GameObject canvas)
        {
            window = new TabbedWindow(canvas.transform, new Vector2(600, 400),
                new List<Tuple<string, GameObject>>() {
                    new Tuple<string, GameObject>("Empire", EmpireWindow()),
                    new Tuple<string, GameObject>("Populations", PopulationsWindow()),
                    new Tuple<string, GameObject>("Technology", TechnologyWindow())
                });
            window.transform.pivot = new Vector2(0, 1);
            window.transform.anchorMin = new Vector2(0, 1);
            window.transform.anchorMax = new Vector2(0, 1);
            window.transform.anchoredPosition = new Vector2(0, 0);
            Image im = window.gameobject.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;
        }

        static GameObject EmpireWindow()
        {
            GameObject go = new GameObject("Empire Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, "empire_window_title", null, 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);
            InfoTable table = InfoTable.Create(go.transform, new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("population", ()=>Simulation.God.PlayerEmpire.Population),
                    new Tuple<string, Func<object>>("Wealth", () => Simulation.God.PlayerEmpire.Wealth)
                }, 200, 12, "Populations");
            Center(table.transform, new Vector2(100, -100));
            return go;
        }

        static GameObject PopulationsWindow()
        {
            GameObject go = new GameObject("Populations Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, "populations_window_title", null, 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);

            InfoTable tablePops = InfoTable.Create(go.transform, () =>
            {
                List<Tuple<string, List<Func<object>>>> list = new List<Tuple<string, List<Func<object>>>>();
                list.Add(new Tuple<string, List<Func<object>>>("Population", new List<Func<object>>() { () => "Inhabitants", () => "Wealth" }));
                foreach (Empires.Population pop in Simulation.God.PlayerEmpire.Populations)
                {
                    list.Add(new Tuple<string, List<Func<object>>>(pop.ToString(), new List<Func<object>>() { () => pop.Count, () => pop.Wealth }));
                }
                return list;
            }, 200);
            Center(tablePops.transform, new Vector2(-100, -100));

            return go;
        }

        static GameObject TechnologyWindow()
        {
            GameObject go = new GameObject("Technology Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, "technology_window_title", null, 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);

            InfoTable tableSectors = InfoTable.Create(go.transform, () =>
            {
                List<Tuple<string, Func<object>>> list = new List<Tuple<string, Func<object>>>();
                foreach (KeyValuePair<Empires.Technology.Technology.Sector, double> kvp in Simulation.God.PlayerEmpire.Academy.Funding)
                {
                    list.Add(new Tuple<string, Func<object>>(kvp.Key.ToString(), () => kvp.Value));
                }
                return list;
            }, 200);
            Center(tableSectors.transform, new Vector2(-100, -100));


            InfoTable tableTechs = InfoTable.Create(go.transform, () =>
            {
                List<Tuple<string, Func<object>>> list = new List<Tuple<string, Func<object>>>();
                foreach (var tech in Simulation.God.PlayerEmpire.Academy.Unlocks)
                {
                    list.Add(new Tuple<string, Func<object>>(tech.Name, () => tech.Knowledge.ToString() + "/" + tech.Understanding));
                }
                return list;
            }, 200);
            Center(tableTechs.transform, new Vector2(100, -100));
            return go;
        }

        static void Center(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0.5f, 1);
            tr.anchorMax = new Vector2(0.5f, 1);
            tr.pivot = new Vector2(0.5f, 1);
            tr.anchoredPosition = offset ?? Vector2.zero;
        }

    }
}
