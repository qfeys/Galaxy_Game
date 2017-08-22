using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class StarMap
    {
        static GameObject master;
        static List<MapStar> displayedSystems;
        static GameObject ecliptica;
        static bool isActive = false;

        public static Theater theater { get; private set; }

        public static void Init()
        {
            master = new GameObject("StarMap");
            displayedSystems = new List<MapStar>();
            CreateEcliptica();
            theater = new Theater(Render, true) {
                zoom = -0.5f,
                CamRot = new Vector2(0, 30) * Mathf.Deg2Rad
            };
            theater.SetCenter(Vector3.zero);
            foreach (Bodies.Galaxy.SystemContainer sys in Bodies.Galaxy.systems)
            {
                displayedSystems.Add(new MapStar(sys));
            }
            Render();
            master.SetActive(false);
        }

        public static void Render()
        {
            foreach (var mapStar in displayedSystems)
            {
                mapStar.Update();
            }
        }

        public static void Disable()
        {
            Debug.Log("disable");
            if (isActive == false)
                return;
            master.SetActive(false);
            isActive = false;
        }

        public static void Enable()
        {
            Debug.Log("enable");
            if (isActive == true)
                return;
            master.SetActive(true);
            isActive = true;
        }

        static void CreateEcliptica()
        {
            ecliptica = new GameObject("EclipticaMap");
            ecliptica.transform.SetParent(master.transform);
            Canvas c = ecliptica.AddComponent<Canvas>();
            c.renderMode = RenderMode.WorldSpace;
            ((RectTransform)c.transform).sizeDelta = new Vector2(60, 60);
            CanvasScaler cs = ecliptica.AddComponent<CanvasScaler>();
            cs.referenceResolution = new Vector2(400, 400);
            cs.referencePixelsPerUnit = 15;
            Image im = ecliptica.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("ecliptica");
            im.color = new Color(1, 1, 1, 0.5f);
        }

        public class SystemScript : MonoBehaviour
        {
            public Bodies.StarSystem parent;

            public static implicit operator Vector3(SystemScript ss)
            {
                return displayedSystems.First(ms => (Bodies.StarSystem)ms.System == ss.parent).System;
            }
        }

        class MapStar
        {
            GameObject star;
            GameObject marker;
            LineRenderer line;
            public Bodies.Galaxy.SystemContainer System { get; private set; }

            public MapStar(Bodies.Galaxy.SystemContainer system)
            {
                this.System = system;
                star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                star.name = system.ToString();
                star.transform.SetParent(master.transform);
                star.transform.position = system;
                SystemScript ss = star.AddComponent<SystemScript>();
                ss.parent = system;
                Material mat = star.GetComponent<MeshRenderer>().material;
                mat.EnableKeyword("_EMISSION");
                Color color = Data.Graphics.Color_.FromTemperature(((Bodies.StarSystem)system).Primary.Temperature);
                mat.SetColor("_Color", color);
                mat.SetColor("_EmissionColor", color);
                star.tag = "Inspectable";
                
                marker = new GameObject("Marker", typeof(RectTransform));
                marker.transform.SetParent(ecliptica.transform);
                RectTransform rt = marker.transform as RectTransform;
                rt.sizeDelta = new Vector2(1.6f, 1.6f);
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = (Vector3)system * theater.Scale;
                Image im = marker.AddComponent<Image>();
                im.sprite = Data.Graphics.GetSprite("marker");

                line = star.AddComponent<LineRenderer>();
                line.positionCount = 2;
                line.startWidth = 0.2f;
                line.endWidth = 0.2f;
                line.material = DisplayManager.TheOne.lineMaterial;
                line.material.EnableKeyword("_EMISSION");
                line.material.SetColor("_EmissionColor", color * Mathf.LinearToGammaSpace(0.1f));
                line.SetPosition(0, Vector3.zero);
            }

            public void Update()
            {
                Vector3 pos = (System - theater.Center) * theater.Scale;
                star.transform.position = pos;
                ((RectTransform)marker.transform).anchoredPosition = pos;
                line.SetPosition(0, pos);
                line.SetPosition(1, new Vector3(pos.x, pos.y, 0));
                if (pos.magnitude < 25)
                {
                    star.SetActive(true);
                    marker.SetActive(true);
                }
                else
                {
                    star.SetActive(false);
                    marker.SetActive(false);
                }
            }
        }
    }
}
