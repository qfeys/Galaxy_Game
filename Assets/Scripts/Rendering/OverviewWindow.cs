using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                    new Tuple<string, GameObject>("TEchnology", TechnologyWindow)
                });
            RectTransform tr = window.gameobject.transform as RectTransform;
            tr.pivot = new Vector2(0, 1);
            tr.anchorMin = new Vector2(0, 1);
            tr.anchorMax = new Vector2(0, 1);
            tr.anchoredPosition = new Vector2(0, 0);
        }

        GameObject EmpireWindow
        {
            get
            {
                GameObject go = new GameObject("Empire Window", typeof(RectTransform));

                InfoTable table = new InfoTable(go.transform, new List<Tuple<string, Func<object>>>() {
                    new Tuple<string, Func<object>>("population", ()=>Simulation.God.PlayerEmpire.Population),
                    new Tuple<string, Func<object>>("Wealth", () => Simulation.God.PlayerEmpire.Wealth)
                });
                return go;
            }
        }

        public GameObject TechnologyWindow
        {
            get
            {
                GameObject go = new GameObject("Technology Window", typeof(RectTransform));

                List<Tuple<string, Func<object>>> list = new List<Tuple<string, Func<object>>>();
                foreach (KeyValuePair<Empires.Technology.Technology.Sector, double> kvp in Simulation.God.PlayerEmpire.Academy.Funding)
                {
                    list.Add(new Tuple<string, Func<object>>(kvp.Key.ToString(), () => kvp.Value));
                }
                InfoTable tableSectors = new InfoTable(go.transform, list);

                list = new List<Tuple<string, Func<object>>>();
                foreach (var tech in Simulation.God.PlayerEmpire.Academy.Unlocks)
                {
                    list.Add(new Tuple<string, Func<object>>(tech.Name, () => tech.Knowledge.ToString() + "/" + tech.Understanding));
                }
                InfoTable tableTechs = new InfoTable(go.transform, list);
                return go;
            }
        }
        
    }
}
