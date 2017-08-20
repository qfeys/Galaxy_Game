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

        static GameObject master;
        static Dictionary<GameObject, Planet> displayedPlanets;
        static Dictionary<GameObject, Star> displayedStars;
        static Dictionary<GameObject, LineRenderer> displayedOrbits;   // The gameobject is the orbital, not the line
        static bool isActive = true;

        public static float zoom = 0.0f; // log scale - high values are zoomed in
        static Vector3 center;
        private static Vector2 camRot = Vector2.zero;
        public static Vector2 CamRot { get { return camRot; } set { camRot = new Vector2(Mathf.Clamp(value.x, -100 * Mathf.Deg2Rad, 100 * Mathf.Deg2Rad), Mathf.Clamp(value.y, -100 * Mathf.Deg2Rad, 100 * Mathf.Deg2Rad)); } }

        internal static void Init()
        {
            master = new GameObject("SolarSystem");
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
                if (go == null) return;
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
                Vector3 posPar = Vector3.zero;
                if (s.Value.starSystem.Tertiary != s.Value)
                    posPar = Vector3.zero;
                else
                {
                    switch (s.Value.starSystem.TertiaryPos)
                    {
                    case 0:
                        throw new Exception("Tertiary planet " + s.Value + " does not have a position assigned.");
                    case 1:
                        posPar = (Vector3)s.Value.starSystem.Primary.OrbElements.GetPositionSphere(Simulation.God.Time);
                        break;
                    case 2:
                        posPar = (Vector3)s.Value.starSystem.Secondary.OrbElements.GetPositionSphere(Simulation.God.Time);
                        break;
                    case 3:
                        posPar = Vector3.zero;
                        break;
                    }
                }
                Vector3 posTrue = (Vector3)posS + posPar;
                float scale = Mathf.Pow(10, -zoom);
                s.Key.transform.position = (posTrue - center) * scale;
                float size = (float)(s.Value.Radius * Star.SOLAR_RADIUS * 2 * scale / StarSystem.AU);
                s.Key.transform.localScale = Vector3.one * (size > MIN_SIZE ? size : MIN_SIZE);
                //s.Key.GetComponent<Light>().intensity = (float)s.Value.Luminosity * scale;
                s.Key.GetComponent<Light>().range = 100 * scale;
                if (displayedOrbits.ContainsKey(s.Key))
                {
                    Vector3[] points = FindPointsOnOrbit(s.Value.OrbElements, VERTICES_PER_ORBIT);
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] += posPar - center;
                        points[i] *= scale;
                    }
                    displayedOrbits[s.Key].SetPositions(points);
                    if (Vector3.Distance(points[0], points[VERTICES_PER_ORBIT / 2]) < MIN_SIZE && s.Value.starSystem.Primary != s.Value) // The orbit is smaller than the minimum object size, so do not display it
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
            if (pl.type == Planet.Type.Astroid_Belt) { Debug.Log("Try displaying an astroid belt. Aborting"); return null; }
            if (pl.type == Planet.Type.Double_Planet) { Debug.Log("Try displaying a double planet. Aborting"); return null; }
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.SetParent(master.transform);
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
            go.transform.SetParent(master.transform);
            StarScript ss = go.AddComponent<StarScript>();
            ss.parent = st;
            Light sl = go.AddComponent<Light>();
            sl.type = LightType.Point;
            sl.range = 100;
            sl.intensity = 1;
            sl.shadows = LightShadows.Hard;
            sl.color = Data.Graphics.Color_.FromTemperature(st.Temperature);
            sl.intensity = (float)st.Luminosity;
            Material mat = go.GetComponent<MeshRenderer>().material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_Color", sl.color);
            mat.SetColor("_EmissionColor", sl.color);
            go.tag = "Inspectable";
            return go;
        }

        private static void CreateOrbit(OrbitalElements el, GameObject go)
        {
            GameObject orb = new GameObject("Orbit of " + el);
            orb.transform.SetParent(master.transform);
            LineRenderer lr = orb.AddComponent<LineRenderer>();
            lr.positionCount = VERTICES_PER_ORBIT + 1;
            lr.startWidth = 0.03f;
            lr.endWidth = 0.2f;
            lr.material = DisplayManager.TheOne.lineMaterial;
            lr.material.EnableKeyword("_EMISSION");
            lr.material.SetColor("_EmissionColor", Color.green * Mathf.LinearToGammaSpace(0.1f));
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
            float x = 40 * Mathf.Sin(camRot.x) * Mathf.Cos(camRot.y);
            float y = 40 * Mathf.Sin(camRot.y);
            float z = -40 * Mathf.Cos(camRot.x) * Mathf.Cos(camRot.y);
            Camera.main.transform.position = new Vector3(x, y, z);
            Camera.main.transform.rotation = Quaternion.Euler(camRot.y * Mathf.Rad2Deg, -camRot.x * Mathf.Rad2Deg, 0);
        }

        public static void Disable()
        {
            if (isActive == false)
                return;
            master.SetActive(false);
            isActive = false;
        }

        public static void Enable()
        {
            if (isActive == true)
                return;
            master.SetActive(true);
            isActive = true;
        }

        const int VERTICES_PER_ORBIT = 128;
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
