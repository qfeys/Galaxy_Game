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
        static Dictionary<GameObject, Star> displayedStars;
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
            displayedPlanets = new Dictionary<GameObject, Planet>();
            displayedStars = new Dictionary<GameObject, Star>();
            displayedOrbits = new Dictionary<GameObject, LineRenderer>();
            GameObject prim = CreateStar(syst.Primary);
            displayedStars.Add(prim, syst.Primary);
            if (syst.Primary.OrbElements.SMA != 0) CreateOrbit(syst.Primary.OrbElements, prim);
            if (syst.Secondary != null)
            {
                GameObject sec = CreateStar(syst.Secondary);
                displayedStars.Add(sec, syst.Secondary);
                CreateOrbit(syst.Secondary.OrbElements, sec);
                if (syst.Tertiary != null)
                {
                    GameObject ter = CreateStar(syst.Tertiary);
                    displayedStars.Add(ter, syst.Tertiary);
                    CreateOrbit(syst.Tertiary.OrbElements, ter);
                }
            }

            syst.Planets.ForEach(p =>
            {
                GameObject go = CreatePlanet(p);
                displayedPlanets.Add(go, p);
                CreateOrbit(p.OrbElements, go);
            });
        }

        public static void Render()
        {
            foreach(var p in displayedPlanets)
            {
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
            go.tag = "Orbital";
            return go;
        }

        static GameObject CreateStar(Star st)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = st.ToString();
            StarScript ss = go.AddComponent<StarScript>();
            ss.parent = st;
            Light sl = go.AddComponent<Light>();
            sl.type = LightType.Point;
            sl.range = 100;
            sl.intensity = 1;
            sl.shadows = LightShadows.Hard;
            sl.color = Data.Graphics.Color_.FromTemperature(st.Temperature);
            go.tag = "Orbital";
            return go;
        }

        private static void CreateOrbit(OrbitalElements el, GameObject go)
        {
            GameObject orb = new GameObject("Orbit of " + el);
            LineRenderer lr = orb.AddComponent<LineRenderer>();
            lr.positionCount = VERTICES_PER_ORBIT + 1;
            lr.startWidth = 0.03f;
            lr.endWidth = 0.2f;
            lr.material = DisplayManager.TheOne.lineMaterial;
            displayedOrbits.Add(go, lr);
        }

        const int VERTICES_PER_ORBIT = 40;
        const float MIN_SIZE = 1.0f;

        class PlanetScript : MonoBehaviour
        {
            public Planet parent;

        }

        class StarScript : MonoBehaviour
        {
            public Star parent;

        }
    }
}
