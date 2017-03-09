using UnityEngine;
using System.Collections.Generic;
using System;
using Assets.Scripts.Empires;

namespace Assets.Scripts.Simulation
{
    public class God : MonoBehaviour
    {
        static God TheOne;
        internal static Empires.Empire PlayerEmpire { get; private set; }

        public static DateTime Time { get; internal set; }

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            Time = new DateTime(2100, 1, 1);

            Init();

            Rendering.DisplayManager.TheOne.DisplaySystem((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void Init()
        {
            Empires.Technology.Academy.Init();
            Bodies.Core.Create(1, 22);


            PlayerEmpire = new Empire("TyroTech Empire", ((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]).RandLivableWorld());
        }
    }
}
