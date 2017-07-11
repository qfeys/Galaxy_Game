using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Assets.Scripts.Bodies;
using System.Linq;
using Assets.Scripts.Simulation;

namespace Assets.Scripts.Rendering
{
    public class DisplayManager : MonoBehaviour
    {
        static public DisplayManager TheOne;
        SystemRenderer systemrenderer;
        Inspector inspector;
        InputManager inputManager;

        public GameObject protoStar;
        public GameObject protoGiant;
        public GameObject protoRock;
        public Material lineMaterial;

        public GameObject InspectorWindow;
        public GameObject OverviewWindow;
        public GameObject Clock;
        public float zoom = 12; // log scale - high values are zoomed in

        public void Awake()
        {
            if (TheOne != null) throw new Exception("A second display manager is created");
            TheOne = this;
            systemrenderer = gameObject.AddComponent<SystemRenderer>();
            systemrenderer.InstantiatePrototypes(protoStar, protoGiant, protoRock);
            inspector = InspectorWindow.GetComponent<Inspector>();
            inputManager = gameObject.AddComponent<InputManager>();
        }

        // Use this for initialization
        void Start()
        {
            SetTimeControls();
        }

        // Update is called once per frame
        void Update()
        {
            if(OverviewWindow.activeSelf == true)
                RedrawOverviewWindow();
            Clock.transform.GetChild(0).GetComponent<Text>().text = God.Time.ToString("yyyy.MM.dd HH:mm:ss");
        }

        private void SetTimeControls()
        {
            GameObject prefabControlButton = Clock.transform.GetChild(1).GetChild(0).gameObject;
            foreach(var tc in God.TimeSteps)
            {
                GameObject newControlButton = Instantiate(prefabControlButton);
                newControlButton.name = tc.Key;
                newControlButton.transform.SetParent(Clock.transform.GetChild(1));
                newControlButton.transform.SetSiblingIndex(0);
                newControlButton.transform.GetChild(0).GetComponent<Text>().text = tc.Key;
                newControlButton.GetComponent<Button>().onClick.AddListener(()=> God.DeltaTime = tc.Value);
            }
            Destroy(prefabControlButton);
        }

        #region SystemDisplay

        internal void DisplaySystem(Bodies.StarSystem syst)
        {
            systemrenderer.SetSystem(syst);
            systemrenderer.Render();
        }

        internal void RerenderSystem()
        {
            systemrenderer.Render();
        }

        internal void ChangeZoom(float delta)
        {
            systemrenderer.zoom += delta;
            systemrenderer.Render();
        }

        #endregion

        internal void SetInspector(GameObject obj)
        {
            Orbital orb = systemrenderer.FindOrbital(obj);
            inspector.DisplayOrbital(orb);
        }

        private void RedrawOverviewWindow()
        {
            if (OverviewWindow.transform.GetChild(1).GetChild(0).gameObject.activeSelf == true)  // Empire tab active
            {
                Transform win = OverviewWindow.transform.GetChild(1).GetChild(0);
                InfoTable table = win.Find("Stats").GetComponent<InfoTable>();
                table.SetInfo(new Tuple<string, string>("Population", God.PlayerEmpire.population.ToString()));
                table.AddInfo(new Tuple<string, string>("Wealth", God.PlayerEmpire.wealth.ToString()));
                table.Redraw();
            }
            if (OverviewWindow.transform.GetChild(1).GetChild(4).gameObject.activeSelf == true)  // Technology tab active
            {
                Transform win = OverviewWindow.transform.GetChild(1).GetChild(4);
                InfoTable tableSec = win.Find("Sectors").GetComponent<InfoTable>();
                tableSec.ResetInfo();
                foreach (KeyValuePair<Empires.Technology.Technology.Sector, double> kvp in God.PlayerEmpire.academy.funding)
                {
                    tableSec.AddInfo(new Tuple<string, string>(kvp.Key.ToString(), kvp.Value.ToString()));
                }
                tableSec.Redraw();

                InfoTable tableTech = win.Find("Techs").GetComponent<InfoTable>();
                tableTech.ResetInfo();
                foreach (var tech in God.PlayerEmpire.academy.unlocks)
                {
                    tableTech.AddInfo(new Tuple<string, string>(tech.name, tech.knowledge.ToString() +"/" + tech.understanding.ToString()));
                }
                tableTech.Redraw();
            }
        }
    }
}
