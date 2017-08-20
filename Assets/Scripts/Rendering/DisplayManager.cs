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
        bool systemViewActive = true;
        Theater activeTheater { get { return systemViewActive ? SystemRenderer.theater : StarMap.theater; } }

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
            SystemRenderer.Init();
            StarMap.Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (systemViewActive)
                SystemRenderer.Render();
            else
                StarMap.Render();
        }

        #region SystemDisplay

        internal void DisplaySystem(Bodies.StarSystem syst)
        {
            SystemRenderer.SetSystem(syst);
            SystemRenderer.Render();
        }

        internal void Rerender()
        {
            activeTheater.Render();
        }

        internal void ChangeZoom(float delta)
        {
            activeTheater.zoom += delta;
            activeTheater.Render();
        }

        internal void MoveCamera(Vector2 delta)
        {
            activeTheater.MoveCenter(delta);
            activeTheater.Render();
        }

        internal void TiltCamera(Vector2 delta)
        {
            activeTheater.CamRot += delta;
            activeTheater.PlaceCamera();
            activeTheater.Render();
        }

        internal void ToggleView()
        {
            if (systemViewActive)
            {
                SystemRenderer.Disable();
                StarMap.Enable();
                systemViewActive = false;
            }
            else
            {
                StarMap.Disable();
                SystemRenderer.Enable();
                systemViewActive = true;
            }
            activeTheater.PlaceCamera();
            activeTheater.Render();
        }

        #endregion

        internal void SetInspector(GameObject obj)
        {
            Planet orb = SystemRenderer.FindOrbital(obj);
            Inspector.DisplayPlanet(orb);
        }
    }
}
