using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Bodies;
using System.Linq;

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
        }

        // Update is called once per frame
        void Update()
        {
            if(OverviewWindow.activeSelf == true)
                RedrawOverviewWindow();
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
            if(OverviewWindow.transform.GetChild(1).GetChild(0).gameObject.activeSelf == true)  // Empire tab active
            {
                Transform win = OverviewWindow.transform.GetChild(1).GetChild(0);
                InfoTable table = win.Find("Stats").GetComponent<InfoTable>();
                table.SetInfo(new Tuple<string, string>("Population", God.PlayerEmpire.population.ToString()));
                table.AddInfo(new Tuple<string, string>("Wealth", God.PlayerEmpire.wealth.ToString()));
                table.Redraw();
            }
        }
    }
}
