using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Bodies;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    class SystemRenderer : MonoBehaviour
    {

        Dictionary<GameObject, Bodies.Orbital> DisplayedBodies;
        Dictionary<GameObject, LineRenderer> DisplayedOrbits;   // The gameobject is the orbital, not the line
        Dictionary<Type, GameObject> prototypes;

        public float zoom = 10.8f; // log scale - high values are zoomed in

        public void Awake()
        {

        }

        public void InstantiatePrototypes(GameObject star, GameObject giant, GameObject rock)
        {
            prototypes = new Dictionary<Type, GameObject> {
                { typeof(Bodies.Star), star },
                { typeof(Bodies.Giant), giant },
                { typeof(Bodies.Rock), rock }
            };
        }

        internal void SetSystem(StarSystem syst)
        {
            DisplayedBodies = new Dictionary<GameObject, Bodies.Orbital>();
            DisplayedOrbits = new Dictionary<GameObject, LineRenderer>();
            GameObject star = null;
            syst.Childeren.ForEach(b =>
            {
                GameObject go = Instantiate(prototypes[b.GetType()]);
                go.name = b.ToString();
                go.tag = "Orbital";
                if (star == null && b.GetType() == typeof(Bodies.Star)) star = go;
                DisplayedBodies.Add(go, b);
                if (b.GetType() != typeof(Bodies.Star))
                {
                    go.transform.SetParent(star.transform);

                    GameObject orb = new GameObject("Orbit of " + b);
                    orb.transform.SetParent(star.transform);
                    LineRenderer lr = orb.AddComponent<LineRenderer>();
                    lr.SetVertexCount(40);
                    lr.SetWidth(0.1f, 0.2f);
                    lr.material = DisplayManager.TheOne.lineMaterial;
                    DisplayedOrbits.Add(go, lr);
                }
            });
        }

        public void Render()
        {
            GameObject star = null;
            foreach(var b in DisplayedBodies)
            {
                if (star == null && b.Value.GetType() == typeof(Bodies.Star)) star = b.Key;
                VectorS posS = b.Value.Elements.GetPositionSphere(Simulation.God.Time);
                float scale = Mathf.Pow(10, -zoom);
                Vector3 v = (Vector3)posS * scale;
                b.Key.transform.position = v;
                if (DisplayedOrbits.ContainsKey(b.Key))
                {
                    DisplayedOrbits[b.Key].SetPositions(FindPointsOnOrbit(b.Value.Elements, 40));
                }
            }
        }

        private Vector3[] FindPointsOnOrbit(OrbitalElements elements, int number)
        {
            //elements.FindPointsOnOrbit(20).ToList().ForEach(vs => Debug.Log(vs));
            return elements.FindPointsOnOrbit(number).Select(vs => (Vector3)vs * Mathf.Pow(10, -zoom)).ToArray();

        }

        internal Orbital FindOrbital(GameObject obj)
        {
            if (DisplayedBodies.ContainsKey(obj))
            {
                return DisplayedBodies[obj];
            }
            throw new ArgumentException(obj.ToString() + " could not be found in the dictionary 'DisplayedBodies'.");
        }
    }
}
