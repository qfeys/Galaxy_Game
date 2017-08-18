﻿using UnityEngine;
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
        
        public Material lineMaterial;

        public void Awake()
        {
            if (TheOne != null) throw new Exception("A second display manager is created");
            TheOne = this;
            inputManager = gameObject.AddComponent<InputManager>();
        }

        // Use this for initialization
        void Start()
        {
        }

        internal void Init()
        {
            OverviewWindow.Create(GameObject.FindWithTag("MainCanvas"));
            Inspector.Create(GameObject.FindWithTag("MainCanvas"));
            Clock.Create(GameObject.FindWithTag("MainCanvas"));
            SwitchBoard.Create(GameObject.FindWithTag("MainCanvas"));
            MouseOver.Create();
        }

        // Update is called once per frame
        void Update()
        {
            SystemRenderer.Render();
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
