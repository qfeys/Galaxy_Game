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
            TextBox title = new TextBox(go.transform, TextRef.Create("empire_window_title"), 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);
            InfoTable table = InfoTable.Create(go.transform, new List<Tuple<TextRef, TextRef>>() {
                    new Tuple<TextRef,TextRef>(TextRef.Create("population"),    TextRef.Create(()=>Simulation.God.PlayerEmpire.Population)),
                    new Tuple<TextRef,TextRef>(TextRef.Create("Wealth"),        TextRef.Create(() => Simulation.God.PlayerEmpire.Wealth))
                }, 200, 12, "Populations");
            Center(table.transform, new Vector2(100, -100));
            return go;
        }

        static GameObject PopulationsWindow()
        {
            GameObject go = new GameObject("Populations Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, TextRef.Create("populations_window_title"), 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);

            InfoTable tablePops = InfoTable.Create(go.transform, () =>
            {
                List<List<TextRef>> list = new List<List<TextRef>>();
                list.Add(new List<TextRef>() {
                    TextRef.Create("Population"),
                    TextRef.Create("Inhabitants"),
                    TextRef.Create("Wealth"),
                    TextRef.Create("Details")
                });
                foreach (Empires.Population pop in Simulation.God.PlayerEmpire.Populations)
                {
                    list.Add(new List<TextRef>()
                    {
                        TextRef.Create(pop.ToString()),
                        TextRef.Create(() => pop.Count),
                        TextRef.Create(() => pop.Wealth),
                        TextRef.Create("show").AddLink(()=>PopDetails.OpenDetails(pop))
                    });
                }
                return list;
            }, 200);
            Center(tablePops.transform, new Vector2(-100, -100));

            return go;
        }

        static GameObject TechnologyWindow()
        {
            GameObject go = new GameObject("Technology Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, TextRef.Create("technology_window_title"), 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);

            InfoTable tableSectors = InfoTable.Create(go.transform, () =>
            {
                List<Tuple<TextRef, TextRef>> list = new List<Tuple<TextRef, TextRef>>();
                foreach (KeyValuePair<Empires.Technology.Technology.Sector, double> kvp in Simulation.God.PlayerEmpire.Academy.Funding)
                {
                    list.Add(new Tuple<TextRef, TextRef>(TextRef.Create(kvp.Key.ToString()), TextRef.Create(() => kvp.Value)));
                }
                return list;
            }, 200);
            Center(tableSectors.transform, new Vector2(-100, -100));


            InfoTable tableTechs = InfoTable.Create(go.transform, () =>
            {
                List<Tuple<TextRef, TextRef>> list = new List<Tuple<TextRef, TextRef>>();
                foreach (var tech in Simulation.God.PlayerEmpire.Academy.Unlocks)
                {
                    list.Add(new Tuple<TextRef, TextRef>(TextRef.Create(tech.Name), TextRef.Create(() => tech.Knowledge.ToString() + "/" + tech.Understanding)));
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
