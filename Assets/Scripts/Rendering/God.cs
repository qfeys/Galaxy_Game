using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Rendering
{
    public class God : MonoBehaviour
    {
        static God TheOne;
        internal static Empires.Empire PlayerEmpire { get; private set; }

        public static long Time { get; internal set; }

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            Time = 0;

            Bodies.Core.Create(1, 22);
            PlayerEmpire = new Empires.Empire();
            DisplayManager.TheOne.DisplaySystem((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
