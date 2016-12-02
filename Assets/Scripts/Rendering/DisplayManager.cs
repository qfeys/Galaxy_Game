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

        public GameObject protoInspector;
        public float zoom = 12; // log scale - high values are zoomed in

        public void Awake()
        {
            if (TheOne != null) throw new Exception("A second display manager is created");
            TheOne = this;
            systemrenderer = gameObject.AddComponent<SystemRenderer>();
            systemrenderer.InstantiatePrototypes(protoStar, protoGiant, protoRock);
            var insp = Instantiate(protoInspector);
            inspector = insp.GetComponent<Inspector>();
            inputManager = gameObject.AddComponent<InputManager>();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void DisplaySystem(Bodies.StarSystem syst)
        {
            systemrenderer.SetSystem(syst);
            systemrenderer.Render();
        }

        internal void RerenderSystem()
        {
            systemrenderer.Render();
        }

        internal void SetInspector(GameObject obj)
        {
            Orbital orb = systemrenderer.FindOrbital(obj);
            inspector.DisplayOrbital(orb);
        }

        internal void ChangeZoom(float delta)
        {
            systemrenderer.zoom += delta;
            systemrenderer.Render();
        }
    }
}
