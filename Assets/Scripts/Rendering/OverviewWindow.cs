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
                new List<Tuple<TextRef, GameObject>>() {
                    new Tuple<TextRef, GameObject>(TextRef.Create("OW_empire"), EmpireWindow()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("OW_populations"), PopulationsWindow()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("OW_technology"), TechnologyWindow())
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
            TextBox title = new TextBox(go.transform, TextRef.Create("empire_window_title"), 24, TextAnchor.UpperCenter);
            InfoTable table = InfoTable.Create(go.transform, new List<List<TextRef>>() {
                    new List<TextRef>(){TextRef.Create("population"),    TextRef.Create(()=>Simulation.God.PlayerEmpire.Population) },
                    new List<TextRef>(){TextRef.Create("Wealth"),        TextRef.Create(() => Simulation.God.PlayerEmpire.Wealth) }
                }, 200, fontSize: 12, title: "Populations");
            Center(table.transform, new Vector2(100, -100));
            return go;
        }

        static GameObject PopulationsWindow()
        {
            GameObject go = new GameObject("Populations Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, TextRef.Create("populations_window_title"), 24, TextAnchor.UpperCenter);

            InfoTable tablePops = InfoTable.Create(go.transform, () => Simulation.God.PlayerEmpire.Populations,
                pop => new List<TextRef>() {
                        TextRef.Create(pop.ToString()),
                        TextRef.Create(() => pop.Count),
                        TextRef.Create(() => pop.Wealth),
                        TextRef.Create("show").AddLink(()=>PopDetails.OpenDetails(pop)) },
                300, new List<TextRef>() {
                    TextRef.Create("Population"),
                    TextRef.Create("Citizens"),
                    TextRef.Create("Wealth"),
                    TextRef.Create("Details")
                }, 12, null);
            Center(tablePops.transform, new Vector2(-100, -100));

            return go;
        }

        static GameObject TechnologyWindow()
        {
            GameObject go = new GameObject("Technology Window", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, TextRef.Create("technology_window_title"), 24, TextAnchor.UpperCenter);

            InfoTable tableSectors = InfoTable.Create(go.transform,
                () => Enum.GetValues(typeof(Empires.Technology.Technology.Sector)).Cast<Empires.Technology.Technology.Sector>().ToList(),
                sector => new List<TextRef>() { sector.ToString(), 0 },
                200, new List<TextRef>() { "tech_sector", "funding" }, 12, null);
            Center(tableSectors.transform, new Vector2(-100, -100));


            InfoTable tableTechs = InfoTable.Create(go.transform, () => Simulation.God.PlayerEmpire.Academy.Unlocks,
                tech => new List<TextRef>() { tech.Name, tech.Knowledge, tech.Understanding },
                200, new List<TextRef>() { "", "knowledge", "understanding" }, 12, null);
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
