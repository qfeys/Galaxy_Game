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
        Dictionary<GameObject, Bodies.Orbital> DisplayedBodies;
        Dictionary<GameObject, LineRenderer> DisplayedOrbits;
        public GameObject protoStar;
        public GameObject protoGiant;
        public GameObject protoRock;
        Dictionary<Type, Func<GameObject>> protoInstantiation;
        public float zoom = 12; // log scale - high values are zoomed in

        public void Awake()
        {
            if (TheOne != null) throw new Exception("A second display manager is created");
            TheOne = this;
            protoInstantiation = new Dictionary<Type, Func<GameObject>> {
                { typeof(Star), () =>  Instantiate(protoStar) },
                { typeof(Giant), () => Instantiate(protoGiant) },
                { typeof(Rock), () => Instantiate(protoRock) }
            };
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
            DisplayedOrbits = new Dictionary<GameObject, LineRenderer>();
            GameObject star = null;
            syst.Childeren.ForEach(b =>
            {
                GameObject go = protoInstantiation[b.GetType()]();
                if (star == null && b.GetType() == typeof(Bodies.Star)) star = go;
                Bodies.VectorS posS = b.Elements.GetPositionSphere(God.Time);
                float scale = Mathf.Pow(10, -zoom);
                Vector3 v = (Vector3)posS * scale;
                go.transform.position = v;
                DisplayedBodies.Add(go, b);
                if (b.GetType() != typeof(Bodies.Star))
                {
                    go.transform.SetParent(star.transform);

                    GameObject orb = new GameObject("Orbit of " + b);
                    orb.transform.SetParent(star.transform);
                    LineRenderer lr = orb.AddComponent<LineRenderer>();
                    lr.SetVertexCount(40);
                    lr.SetWidth(0.1f, 0.2f);
                    lr.SetPositions(FindPointsOnOrbit(b.Elements, 40));
                    DisplayedOrbits.Add(go, lr);
                }
            });
        }

        private Vector3[] FindPointsOnOrbit(OrbitalElements elements, int number)
        {
            //elements.FindPointsOnOrbit(20).ToList().ForEach(vs => Debug.Log(vs));
            return elements.FindPointsOnOrbit(number).Select(vs => (Vector3)vs * Mathf.Pow(10, -zoom)).ToArray();
            
        }
    }
}
