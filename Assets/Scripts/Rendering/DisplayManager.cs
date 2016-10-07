using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Rendering
{
    public class DisplayManager : MonoBehaviour
    {
        static public DisplayManager TheOne;
        Dictionary<GameObject, Bodies.Orbital> DisplayedBodies;
        public GameObject protoGiant;
        public float zoom = 12; // log scale - high values are zoomed in

        public void Awake()
        {
            if (TheOne != null) throw new Exception("A second display manager is created");
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
            DisplayedBodies = new Dictionary<GameObject, Bodies.Orbital>();
            syst.Childeren.ForEach(b =>
            {
                GameObject go = Instantiate(protoGiant);
                Bodies.VectorS posS = b.Elements.GetPositionSphere(God.Time);
                double scale = Math.Pow(10, -zoom);
                go.transform.position = new Vector3((float)(posS.r * Math.Cos(posS.u) * Math.Sin(posS.v) * scale),
                                                    (float)(posS.r * Math.Sin(posS.u) * Math.Sin(posS.v) * scale),
                                                    (float)(posS.r * Math.Cos(posS.v) * scale));
                DisplayedBodies.Add(go, b);

            });
        }
    }
}
