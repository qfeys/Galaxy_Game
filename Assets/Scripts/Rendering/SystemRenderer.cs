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
        static Vector3 center;
        public static Vector2 camRot = Vector2.zero;

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
                p.moons.ForEach(m =>
                {
                    GameObject gom = CreatePlanet(m);
                    displayedPlanets.Add(gom, m);
                    CreateOrbit(m.OrbElements, gom);
                });
            });
            center = Vector3.zero;
        }

        public static void Render()
        {
            foreach (var s in displayedStars)
            {
                VectorS posS = s.Value.OrbElements.GetPositionSphere(Simulation.God.Time);
                Vector3 posPar = Vector3.zero;  // TODO: correct position of tertiary planets
                Vector3 posTrue = (Vector3)posS + posPar;
                float scale = Mathf.Pow(10, -zoom);
                s.Key.transform.position = (posTrue - center) * scale;
                float size = (float)s.Value.Radius * scale / StarSystem.AU; // TODO correct scale radius
                s.Key.transform.localScale = Vector3.one * (size > MIN_SIZE ? size : MIN_SIZE);
                if (displayedOrbits.ContainsKey(s.Key))
                {
                    Vector3[] points = FindPointsOnOrbit(s.Value.OrbElements, VERTICES_PER_ORBIT);
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] += posPar - center;
                        points[i] *= scale;
                    }
                    displayedOrbits[s.Key].SetPositions(points);
                    if (Vector3.Distance(points[0], points[VERTICES_PER_ORBIT / 2]) < MIN_SIZE) // The orbit is smaller than the minimum object size, so do not display it
                    {
                        s.Key.SetActive(false);
                        displayedOrbits[s.Key].gameObject.SetActive(false);
                    }
                    else
                    {
                        s.Key.SetActive(true);
                        displayedOrbits[s.Key].gameObject.SetActive(true);
                    }
                }
            }
            foreach (var p in displayedPlanets)
            {
                VectorS posS = p.Value.OrbElements.GetPositionSphere(Simulation.God.Time);
                Vector3 posPar = p.Value.ParentPlanet == null ?
                    p.Value.Parent.OrbElements.GetPosition(Simulation.God.Time) : p.Value.ParentPlanet.OrbElements.GetPosition(Simulation.God.Time);
                Vector3 posTrue = (Vector3)posS + posPar;
                float scale = Mathf.Pow(10, -zoom);
                p.Key.transform.position = (posTrue - center) * scale;
                float size = p.Value.Radius * scale / StarSystem.AU;
                p.Key.transform.localScale = Vector3.one * (size > MIN_SIZE ? size : MIN_SIZE);
                if (displayedOrbits.ContainsKey(p.Key))
                {
                    Vector3[] points = FindPointsOnOrbit(p.Value.OrbElements, VERTICES_PER_ORBIT);
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] += posPar - center;
                        points[i] *= scale;
                    }
                    displayedOrbits[p.Key].SetPositions(points);
                    if (Vector3.Distance(points[0], points[VERTICES_PER_ORBIT / 2]) < MIN_SIZE) // The orbit is smaller than the minimum object size, so do not display it
                    {
                        p.Key.SetActive(false);
                        displayedOrbits[p.Key].gameObject.SetActive(false);
                    }
                    else
                    {
                        p.Key.SetActive(true);
                        displayedOrbits[p.Key].gameObject.SetActive(true);
                    }
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
            go.tag = "Inspectable";
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
            go.tag = "Inspectable";
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

        public static void SetCenter(Vector3 c) { center = c; }
        public static void SetCenter(Planet p) {
            VectorS posS = p.OrbElements.GetPositionSphere(Simulation.God.Time);
            Vector3 posPar = p.ParentPlanet == null ?
                p.Parent.OrbElements.GetPosition(Simulation.God.Time) : p.ParentPlanet.OrbElements.GetPosition(Simulation.God.Time);
            Vector3 posTrue = (Vector3)posS + posPar;
            SetCenter(posTrue);
        }

        public static void MoveCenter(Vector2 v) { center += (Vector3)v / Mathf.Pow(10, -zoom) * 0.1f; }

        public static void ResetView() { zoom = 0; center = Vector2.zero; camRot = Vector2.zero; PlaceSystemCamera(); }

        public static void PlaceSystemCamera()
        {
            float x = 40 * Mathf.Sin(camRot.x);
            float y = 40 * Mathf.Sin(camRot.y);
            float z = -40 * Mathf.Cos(camRot.x) * Mathf.Cos(camRot.y);
            Camera.main.transform.position = new Vector3(x, y, z);
            Camera.main.transform.rotation = Quaternion.Euler(camRot.y * Mathf.Rad2Deg, -camRot.x * Mathf.Rad2Deg, 0);
        }

        const int VERTICES_PER_ORBIT = 40;
        const float MIN_SIZE = 1.0f;

        public class PlanetScript : MonoBehaviour
        {
            public Planet parent;

        }

        public class StarScript : MonoBehaviour
        {
            public Star parent;

        }
    }
}
