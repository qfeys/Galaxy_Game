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
            tr.sizeDelta = new Vector2(1000, 600);
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

            // List of populations
            // TODO

            // Tabs
            {
                List<Tuple<TextRef, GameObject>> tabsList = new List<Tuple<TextRef, GameObject>> {
                    new Tuple<TextRef, GameObject>(TextRef.Create("Overview"), OverviewTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Construction"), ConstructionTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Production"), ProductionTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Mining"), MiningTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Stockpile"), StockpileTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Enviroment"), EnviromentTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Demographics"), DemographicsTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Economy"), EconomyTab()),
                    new Tuple<TextRef, GameObject>(TextRef.Create("Research"), ResearchTab())
                };
                TabbedWindow tabs = new TabbedWindow(go.transform, new Vector2(700, 550), tabsList, 12, false);
                tabs.transform.pivot = new Vector2(1, 1);
                tabs.transform.anchorMin = new Vector2(1, 1);
                tabs.transform.anchorMax = new Vector2(1, 1);
                tabs.transform.anchoredPosition = new Vector2(-60, -40);
            }
        }

        private static GameObject OverviewTab()
        {
            return new GameObject();
        }

        private static GameObject ConstructionTab()
        {
            GameObject go = new GameObject("Construction", typeof(RectTransform));

            List<Tuple<TextRef, GameObject>> tabs = new List<Tuple<TextRef, GameObject>>() {
                new Tuple<TextRef, GameObject>(TextRef.Create("installations"), InstallationsTabTab()),
                new Tuple<TextRef, GameObject>(TextRef.Create("orbital_habitats"), OrbHabTabTab()),
                new Tuple<TextRef, GameObject>(TextRef.Create("pdc"), PdcTabTab()),
                new Tuple<TextRef, GameObject>(TextRef.Create("special_projects"), SpecialProjectsTabTab())
            };

            TabbedWindow constructionTabs = new TabbedWindow(go.transform, new Vector2(700, 250), tabs, 12, false);
            constructionTabs.transform.anchorMin = new Vector2(0, 1);
            constructionTabs.transform.anchorMax = new Vector2(0, 1);
            constructionTabs.transform.pivot = new Vector2(0, 1);
            constructionTabs.transform.anchoredPosition = new Vector2(0, -10);

            InfoTable constructionQueue = InfoTable.Create(go.transform, () => activePopulation.industryCenter.constructionQueue,
                job => new List<TextRef>() { job.instl.name, job.amount, TextRef.Create(() => job.capacity * 100), job.instl.costWork, "NaN", "NaN" },
                700, new List<TextRef>() { "", "amount remaining", "% of capacity", "cost per item", "next item", "end of job" }, 12, null);
            constructionQueue.transform.anchorMin = new Vector2(0, 0.5f);
            constructionQueue.transform.anchorMax = new Vector2(0, 0.5f);
            constructionQueue.transform.pivot = new Vector2(0, 1);
            constructionQueue.transform.anchoredPosition = new Vector2(0, 0);

            GameObject constructionInterface = new GameObject("ConstructionInterface", typeof(RectTransform));
            {
                RectTransform tr = (RectTransform)constructionInterface.transform;
                tr.SetParent(go.transform);
                tr.sizeDelta = new Vector2(700, 50);
                tr.anchorMin = new Vector2(0, 1);
                tr.anchorMax = new Vector2(0, 1);
                tr.pivot = new Vector2(0, 1);
                tr.anchoredPosition = new Vector2(0, -230);
                TextBox tb1 = new TextBox(tr, "amount", 12, TextAnchor.MiddleLeft);
                tb1.transform.anchoredPosition = new Vector2(100, 0);
                TextBox.InputBox ib1 = new TextBox.InputBox(tr, "0", 12, 50, TextAnchor.MiddleRight, InputField.ContentType.IntegerNumber);
                ib1.transform.anchorMin = new Vector2(0, .5f);
                ib1.transform.anchorMax = new Vector2(0, .5f);
                ib1.transform.anchoredPosition = new Vector2(250, 0);
                TextBox tb2 = new TextBox(tr, "capacity", 12, TextAnchor.MiddleLeft);
                tb2.transform.anchoredPosition = new Vector2(300, 0);
                TextBox.InputBox ib2 = new TextBox.InputBox(tr, "100", 12, 50, TextAnchor.MiddleRight, InputField.ContentType.DecimalNumber);
                ib2.transform.anchorMin = new Vector2(0, .5f);
                ib2.transform.anchorMax = new Vector2(0, .5f);
                ib2.transform.anchoredPosition = new Vector2(450, 0);
                Action butAct = () =>
                {
                    GameObject at = constructionTabs.GetActiveTab();
                    InfoTable it = at.transform.GetChild(0).GetComponent<InfoTable.ActiveInfoTable>().parent;
                    Empires.Industry.Installation newInstall = it.RetrieveHighlight<Empires.Industry.Installation>();
                    if (newInstall != null)
                        activePopulation.industryCenter.BuildInstallation(newInstall, int.Parse(ib1.TakeValue()), double.Parse(ib2.TakeValue()) / 100);
                };
                TextBox but = new TextBox(tr, TextRef.Create("build").AddLink(butAct), 12, TextAnchor.MiddleLeft);
                but.transform.anchoredPosition = new Vector2(500, 0);
            }

            return go;
        }

        private static GameObject InstallationsTabTab()
        {
            GameObject go = new GameObject("Construction", typeof(RectTransform));

            InfoTable installationsList = InfoTable.Create(go.transform,
                () => Empires.Industry.Installation.installationList.FindAll(ins => ins.IsValid(activePopulation)),
                instl => new List<TextRef>() { instl.name, instl.costWork.ToString(),
                            instl.costResources["steel"],
                            instl.costResources["nonFerrous"],
                            instl.costResources["carbon"],
                            instl.costResources["silicates"],
                            instl.costResources["rareEarth"],
                            instl.costResources["components"],
                            instl.costResources["electronics"]}, 700, new List<TextRef>() { "", "work", "steel", "nonFerrous", "carbon", "silicates", "rareEarth", "components", "electronics" }, 12,
                null);

            installationsList.transform.sizeDelta = new Vector2(700, 250);
            installationsList.transform.anchorMin = new Vector2(1, 1);
            installationsList.transform.anchorMax = new Vector2(1, 1);
            installationsList.transform.pivot = new Vector2(1, 1);
            installationsList.transform.anchoredPosition = new Vector2(0, -10);

            return go;
        }

        private static GameObject OrbHabTabTab()
        {
            return new GameObject();
        }

        private static GameObject PdcTabTab()
        {
            return new GameObject();
        }

        private static GameObject SpecialProjectsTabTab()
        {
            return new GameObject();
        }

        private static GameObject ProductionTab()
        {
            GameObject go = new GameObject("Production", typeof(RectTransform));

            List<Tuple<TextRef, GameObject>> tabs = new List<Tuple<TextRef, GameObject>>() {
                new Tuple<TextRef, GameObject>(TextRef.Create("ship_components"), Ship_componentsTabTab()),
                new Tuple<TextRef, GameObject>(TextRef.Create("ammunition"), AmmunitionTabTab()),
                new Tuple<TextRef, GameObject>(TextRef.Create("fighters"), FightersTabTab())
            };

            TabbedWindow constructionTabs = new TabbedWindow(go.transform, new Vector2(700, 250), tabs, 12, false);
            constructionTabs.transform.anchorMin = new Vector2(0, 1);
            constructionTabs.transform.anchorMax = new Vector2(0, 1);
            constructionTabs.transform.pivot = new Vector2(0, 1);
            constructionTabs.transform.anchoredPosition = new Vector2(0, -10);

            InfoTable productionQueue = InfoTable.Create(go.transform, () =>
            {
                List<List<TextRef>> list = new List<List<TextRef>>();
                list.Add(new List<TextRef>() { "", "amount remaining", "% of capacity", "cost per item", "end of job" });
                foreach (Empires.Industry.IndustryCenter.Job job in activePopulation.industryCenter.productionQueue)
                {
                    list.Add(new List<TextRef>() { job.instl.name, job.amount, job.capacity, job.instl.costWork, "NaN" });
                }
                return list;
            }, 700);
            productionQueue.transform.anchorMin = new Vector2(0, 0.65f);
            productionQueue.transform.anchorMax = new Vector2(0, 0.65f);
            productionQueue.transform.pivot = new Vector2(0, 1);
            productionQueue.transform.anchoredPosition = new Vector2(0, 0);

            GameObject componentsPanel = new GameObject("Components", typeof(RectTransform));
            {
                RectTransform rt = (RectTransform)componentsPanel.transform;
                rt.SetParent(go.transform);
                rt.sizeDelta = new Vector2(300, 150);
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                rt.anchoredPosition = new Vector2(10, 10);
                TextBox title = new TextBox(rt, "component_production", 12, TextAnchor.UpperLeft);
                TextBox cap = new TextBox(rt, "component_capacity", 12, TextAnchor.UpperLeft);
                cap.transform.anchoredPosition = new Vector2(0, -30);
                TextBox cap2 = new TextBox(rt, activePopulation.industryCenter.maxComponentProduction, 12, TextAnchor.UpperRight);
                cap2.transform.anchoredPosition = new Vector2(0, -30);
                TextBox act = new TextBox(rt, "component_active_capacity", 12, TextAnchor.UpperLeft);
                act.transform.anchoredPosition = new Vector2(0, -60);
                TextBox act2 = new TextBox(rt, activePopulation.industryCenter.activeCapacityComponentProduction, 12, TextAnchor.UpperRight);
                act2.transform.anchoredPosition = new Vector2(0, -60);
            }
            GameObject electronicsPanel = new GameObject("Electronics", typeof(RectTransform));
            {
                RectTransform rt = (RectTransform)electronicsPanel.transform;
                rt.SetParent(go.transform);
                rt.sizeDelta = new Vector2(300, 150);
                rt.anchorMin = new Vector2(1, 0);
                rt.anchorMax = new Vector2(1, 0);
                rt.pivot = new Vector2(1, 0);
                rt.anchoredPosition = new Vector2(-10, 10);
                TextBox title = new TextBox(rt, "electronics_production", 12, TextAnchor.UpperRight);
                TextBox cap = new TextBox(rt, "electronics_capacity", 12, TextAnchor.UpperRight);
                cap.transform.anchoredPosition = new Vector2(0, -30);
                TextBox cap2 = new TextBox(rt, activePopulation.industryCenter.maxElectronicsProduction, 12, TextAnchor.UpperLeft);
                cap2.transform.anchoredPosition = new Vector2(0, -30);
                TextBox act = new TextBox(rt, "electronics_active_capacity", 12, TextAnchor.UpperRight);
                act.transform.anchoredPosition = new Vector2(0, -60);
                TextBox act2 = new TextBox(rt, activePopulation.industryCenter.activeCapacityElectronicsProduction, 12, TextAnchor.UpperLeft);
                act2.transform.anchoredPosition = new Vector2(0, -60);
            }

            return go;
        }

        private static GameObject Ship_componentsTabTab()
        {
            return new GameObject();
        }

        private static GameObject AmmunitionTabTab()
        {
            return new GameObject();
        }

        private static GameObject FightersTabTab()
        {
            return new GameObject();
        }

        private static GameObject MiningTab()
        {
            GameObject go = new GameObject("Mining", typeof(RectTransform));
            Bodies.MiningPile pile = 
                activePopulation != null && 
                activePopulation.Location != null && 
                activePopulation.Location.MiningPile != null ? activePopulation.Location.MiningPile : Bodies.MiningPile.Empty;
            InfoTable tablePops = InfoTable.Create(go.transform, () =>
            {
                List<List<TextRef>> list = new List<List<TextRef>>();
                for (int i = 0; i < pile.pile.Count; i++)
                {
                    int j = i;
                    list.Add(new List<TextRef>() {
                        TextRef.Create(() => pile.pile[j].Item1.ToString()),
                        TextRef.Create(() => pile.pile[j].Item2[0]),
                        TextRef.Create(() => pile.pile[j].Item2[1]),
                        TextRef.Create(() => pile.pile[j].Item2[2])
                    });
                }
                return list;
            }, 200);
            Center(tablePops.transform, new Vector2(-100, -100));

            return go;
        }

        private static GameObject StockpileTab()
        {
            GameObject go = new GameObject("Stockpile", typeof(RectTransform));
            var a = activePopulation.industryCenter.stockpile;
            InfoTable tablePops = InfoTable.Create(go.transform, () =>
            {
                List<List<TextRef>> list = new List<List<TextRef>>();
                for (int i = 0; i < Empires.Industry.Stockpile.ResourceType.ResourceTypes.Count; i++)
                {
                    Empires.Industry.Stockpile.ResourceType current = Empires.Industry.Stockpile.ResourceType.ResourceTypes[i];
                    list.Add(new List<TextRef>() {
                        TextRef.Create(current.ToString()),
                        TextRef.Create(() => activePopulation.industryCenter.stockpile.pile.ContainsKey(current) ? activePopulation.industryCenter.stockpile.pile[current] : 0),
                        TextRef.Create("Add").AddLink(()=>activePopulation.industryCenter.stockpile.Add(current,100))
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
            Empires.Technology.Academy a = Simulation.God.PlayerEmpire.Academy;
            InfoTable tableLabs = InfoTable.Create(go.transform, () =>
            {
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
