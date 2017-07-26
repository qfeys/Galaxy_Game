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

        Dictionary<GameObject, Orbital> displayedBodies;
        Dictionary<GameObject, LineRenderer> displayedOrbits;   // The gameobject is the orbital, not the line
        Dictionary<Type, GameObject> prototypes;

        public float zoom = 10.8f; // log scale - high values are zoomed in

        public void InstantiatePrototypes(GameObject star, GameObject giant, GameObject rock)
        {
            prototypes = new Dictionary<Type, GameObject> {
                { typeof(Star), star },
                { typeof(Giant), giant },
                { typeof(Rock), rock }
            };
        }

        internal void SetSystem(StarSystem syst)
        {
            displayedBodies = new Dictionary<GameObject, Orbital>();
            displayedOrbits = new Dictionary<GameObject, LineRenderer>();
            GameObject star = null;
            syst.Childeren.ForEach(b =>
            {
                GameObject go = Instantiate(prototypes[b.GetType()]);
                go.name = b.ToString();
                go.tag = "Orbital";
                if (star == null && b.GetType() == typeof(Star)) star = go;
                displayedBodies.Add(go, b);
                if (b.GetType() != typeof(Star))
                {
                    go.transform.SetParent(star.transform);

                    GameObject orb = new GameObject("Orbit of " + b);
                    orb.transform.SetParent(star.transform);
                    LineRenderer lr = orb.AddComponent<LineRenderer>();
                    lr.positionCount = VERTICES_PER_ORBIT;
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.2f;
                    lr.material = DisplayManager.TheOne.lineMaterial;
                    displayedOrbits.Add(go, lr);
                }
            });
        }

        public void Render()
        {
            GameObject star = null;
            foreach(var b in displayedBodies)
            {
                if (star == null && b.Value.GetType() == typeof(Star)) star = b.Key;
                VectorS posS = b.Value.Elements.GetPositionSphere(Simulation.God.Time);
                float scale = Mathf.Pow(10, -zoom);
                Vector3 v = (Vector3)posS * scale;
                b.Key.transform.position = v;
                if (displayedOrbits.ContainsKey(b.Key))
                {
                    displayedOrbits[b.Key].SetPositions(FindPointsOnOrbit(b.Value.Elements, VERTICES_PER_ORBIT));
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
            if (displayedBodies.ContainsKey(obj))
            {
                return displayedBodies[obj];
            }
            throw new ArgumentException(obj.ToString() + " could not be found in the dictionary 'DisplayedBodies'.");
        }

        const int VERTICES_PER_ORBIT = 40;
    }
}
