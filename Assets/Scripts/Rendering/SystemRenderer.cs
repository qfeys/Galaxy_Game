using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Bodies;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    static class SystemRenderer
    {

        static Dictionary<GameObject, Planet> displayedPlanets;
        static Dictionary<GameObject, LineRenderer> displayedOrbits;   // The gameobject is the orbital, not the line
        static Dictionary<Type, GameObject> prototypes;

        public static float zoom = 0.0f; // log scale - high values are zoomed in

        public static void InstantiatePrototypes(GameObject star, GameObject giant, GameObject rock)
        {
            prototypes = new Dictionary<Type, GameObject> {
                { typeof(Star), star }
            };
        }

        internal static void SetSystem(StarSystem syst)
        {
            // TODO: needs fixing because transition orbitals to planets
            displayedPlanets = new Dictionary<GameObject, Planet>();
            displayedOrbits = new Dictionary<GameObject, LineRenderer>();
            GameObject star = null;
            syst.Planets.ForEach(p =>
            {
                GameObject go = CreatePlanet(p);
                go.tag = "Orbital";
                if (star == null && p.GetType() == typeof(Star)) star = go;
                displayedPlanets.Add(go, p);
                if (p.GetType() != typeof(Star))
                {
                    GameObject orb = new GameObject("Orbit of " + p);
                    LineRenderer lr = orb.AddComponent<LineRenderer>();
                    lr.positionCount = VERTICES_PER_ORBIT;
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.2f;
                    lr.material = DisplayManager.TheOne.lineMaterial;
                    displayedOrbits.Add(go, lr);
                }
            });
        }

        public static void Render()
        {
            GameObject star = null;
            foreach(var p in displayedPlanets)
            {
                if (star == null && p.Value.GetType() == typeof(Star)) star = p.Key;
                VectorS posS = p.Value.OrbElements.GetPositionSphere(Simulation.God.Time);
                Vector3 posPar = p.Value.ParentPlanet == null ?
                    p.Value.Parent.OrbElements.GetPosition(Simulation.God.Time) : p.Value.ParentPlanet.OrbElements.GetPosition(Simulation.God.Time);
                Vector3 posTrue = (Vector3)posS + posPar;
                float scale = Mathf.Pow(10, -zoom);
                Vector3 v = posTrue * scale;
                p.Key.transform.position = v;
                float size = p.Value.Radius * scale / StarSystem.AU;
                p.Key.transform.localScale = Vector3.one * (size > MIN_SIZE ? size : MIN_SIZE);
                if (displayedOrbits.ContainsKey(p.Key))
                {
                    Vector3[] points = FindPointsOnOrbit(p.Value.OrbElements, VERTICES_PER_ORBIT);
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] += posPar;
                        points[i] *= scale;
                    }
                    displayedOrbits[p.Key].SetPositions(points);
                }
            }
        }

        private static Vector3[] FindPointsOnOrbit(OrbitalElements elements, int number)
        {
            //elements.FindPointsOnOrbit(20).ToList().ForEach(vs => Debug.Log(vs));
            return elements.FindPointsOnOrbit(number, Simulation.God.Time).Select(vs => (Vector3)vs).ToArray();

        }

        internal static Planet FindOrbital(GameObject obj)
        {
            if (displayedPlanets.ContainsKey(obj))
            {
                return displayedPlanets[obj];
            }
            throw new ArgumentException(obj.ToString() + " could not be found in the dictionary 'DisplayedBodies'.");
        }

        static GameObject CreatePlanet(Planet pl)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            PlanetScript ps = go.AddComponent<PlanetScript>();
            ps.parent = pl;
            go.name = pl.ToString();

            return go;
        }

        const int VERTICES_PER_ORBIT = 40;
        const float MIN_SIZE = 1.0f;

        class PlanetScript : MonoBehaviour
        {
            public Planet parent;

        }
    }
}
