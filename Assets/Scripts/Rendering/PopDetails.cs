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
            go.AddComponent<Dragable>();
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
                trTtl.anchoredPosition = new Vector2(0, -10);
                TextBox text = new TextBox(trTtl, TextRef.Create(() => activePopulation.Name), 24, TextAnchor.UpperCenter);
            }

            // List
            // TODO

            // Tabs
            {
                List<Tuple<TextRef, GameObject>> tabsList = new List<Tuple<TextRef, GameObject>> {
                    new Tuple<TextRef, GameObject>(TextRef.Create("Overview"), OverviewTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Construction"), ConstructionTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Mining"), MiningTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Stockpile"), StockpileTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Enviroment"), EnviromentTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Demographics"), DemographicsTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Economy"), EconomyTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Research"), ResearchTab())
                };
                TabbedWindow tabs = new TabbedWindow(go.transform, new Vector2(600, 550), tabsList, 12, false);
                tabs.transform.pivot = new Vector2(0, 1);
                tabs.transform.anchorMin = new Vector2(0, 1);
                tabs.transform.anchorMax = new Vector2(0, 1);
                tabs.transform.anchoredPosition = new Vector2(140, -40);
            }
        }

        private static GameObject OverviewTab()
        {
            return new GameObject();
        }

        private static GameObject ConstructionTab()
        {
            return new GameObject();
        }

        private static GameObject MiningTab()
        {
            return new GameObject();
        }

        private static GameObject StockpileTab()
        {
            GameObject go = new GameObject("Stockpile", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, TextRef.Create("StockpileTab_title"), 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);
            var a = activePopulation.stockpile;
            InfoTable tablePops = InfoTable.Create(go.transform, ()=> {
                List<List<TextRef>> list = new List<List<TextRef>>();
                for (int i = 0; i < Empires.Production.Stockpile.ResourceType.ResourceTypes.Count; i++)
                {
                    Empires.Production.Stockpile.ResourceType current = Empires.Production.Stockpile.ResourceType.ResourceTypes[i];
                    list.Add(new List<TextRef>() {
                        TextRef.Create(current.ToString()),
                        TextRef.Create(() => activePopulation.stockpile.pile.ContainsKey(current) ? activePopulation.stockpile.pile[current] : 0),
                        TextRef.Create("Add").AddLink(()=>activePopulation.stockpile.Add(current,100))
                    });
                }
                return list;
            }, 200);
            Center(tablePops.transform, new Vector2(-100, -100));

            return go;
        }

        private static GameObject EnviromentTab()
        {
            return new GameObject();
        }

        private static GameObject DemographicsTab()
        {
            return new GameObject();
        }

        private static GameObject EconomyTab()
        {
            return new GameObject();
        }

        private static GameObject ResearchTab()
        {
            GameObject go = new GameObject("Research", typeof(RectTransform));
            TextBox title = new TextBox(go.transform, TextRef.Create("ResearchTab_title"), 24, TextAnchor.MiddleCenter);
            Center(title.transform);
            title.transform.sizeDelta = new Vector2(200, 36);
            Empires.Technology.Academy a = Simulation.God.PlayerEmpire.Academy;
            InfoTable tableLabs = InfoTable.Create(go.transform, () => {
                List<List<TextRef>> list = new List<List<TextRef>>();
                List<Empires.Technology.Academy.Laboratory> labs = a.GetLabsAt(activePopulation);
                list.Add(new List<TextRef>() { "id", "leader", "eff", "project", "eff" });
                foreach (var lab in labs)
                {
                    list.Add(new List<TextRef>() {
                        TextRef.Create(lab.id.ToString("001")),
                        TextRef.Create(() => lab.leader),
                        TextRef.Create(() => lab.leaderEfficiency),
                        TextRef.Create(() => lab.currentProject),
                        TextRef.Create(() => lab.projectEfficiency)
                    });
                }
                return list;
            }, 200);
            Center(tableLabs.transform, new Vector2(-100, -100));

            return go;
        }

        static void Center(RectTransform tr, Vector2? offset = null)
        {
            tr.anchorMin = new Vector2(0.5f, 1);
            tr.anchorMax = new Vector2(0.5f, 1);
            tr.pivot = new Vector2(0.5f, 1);
            tr.anchoredPosition = offset ?? Vector2.zero;
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
