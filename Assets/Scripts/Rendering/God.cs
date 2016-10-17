using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Rendering
{
    public class God : MonoBehaviour
    {
        static God TheOne;
        public static List<string> log;

        public static long Time { get; internal set; }

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            log = new List<string>();
            Time = 0;

            Bodies.Core.Create(1, 22);
            log.ForEach(l => Debug.Log(l));
            DisplayManager.TheOne.DisplaySystem((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
