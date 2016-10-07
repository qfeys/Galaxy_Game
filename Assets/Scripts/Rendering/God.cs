using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Rendering
{
    public class God : MonoBehaviour
    {
        static God TheOne;
        public static List<string> log;

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            log = new List<string>();

            Bodies.Core.Create(1, 22);
            log.ForEach(l => Debug.Log(l));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
