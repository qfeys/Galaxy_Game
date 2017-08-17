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
        InputManager inputManager;

        public GameObject protoStar;
        public GameObject protoGiant;
        public GameObject protoRock;
        public Material lineMaterial;
        
        public GameObject clock;

        public void Awake()
        {
            if (TheOne != null) throw new Exception("A second display manager is created");
            TheOne = this;
            SystemRenderer.InstantiatePrototypes(protoStar, protoGiant, protoRock);
            inputManager = gameObject.AddComponent<InputManager>();
        }

        // Use this for initialization
        void Start()
        {
        }

        internal void Init()
        {
            SetTimeControls();
            OverviewWindow.Create(GameObject.FindWithTag("MainCanvas"));
            Inspector.Create(GameObject.FindWithTag("MainCanvas"));
            Clock.Create(GameObject.FindWithTag("MainCanvas"));
            MouseOver.Create();
        }

        // Update is called once per frame
        void Update()
        {
            clock.transform.GetChild(0).GetComponent<Text>().text = God.Time.ToString("yyyy.MM.dd HH:mm:ss");
            SystemRenderer.Render();
        }

        private void SetTimeControls()
        {
            GameObject prefabControlButton = clock.transform.GetChild(1).GetChild(0).gameObject;
            foreach(var tc in God.timeSteps)
            {
                GameObject newControlButton = Instantiate(prefabControlButton);
                newControlButton.name = tc.Key;
                newControlButton.transform.SetParent(clock.transform.GetChild(1));
                newControlButton.transform.SetSiblingIndex(0);
                newControlButton.transform.GetChild(0).GetComponent<Text>().text = tc.Key;
                newControlButton.GetComponent<Button>().onClick.AddListener(()=> God.deltaTime = tc.Value);
            }
            Destroy(prefabControlButton);
        }

        #region SystemDisplay

        internal void DisplaySystem(Bodies.StarSystem syst)
        {
            SystemRenderer.SetSystem(syst);
            SystemRenderer.Render();
        }

        internal void RerenderSystem()
        {
            SystemRenderer.Render();
        }

        internal void ChangeZoom(float delta)
        {
            SystemRenderer.zoom += delta;
            SystemRenderer.Render();
        }

        #endregion

        internal void SetInspector(GameObject obj)
        {
            Planet orb = SystemRenderer.FindOrbital(obj);
            Inspector.DisplayPlanet(orb);
        }
    }
}
