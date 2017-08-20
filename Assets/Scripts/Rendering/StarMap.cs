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
        static Dictionary<GameObject, Bodies.Galaxy.SystemContainer> displayedSystems;
        static GameObject ecliptica;
        static bool isActive = false;

        public static Theater theater { get; private set; }

        public static void Init()
        {
            master = new GameObject("StarMap");
            displayedSystems = new Dictionary<GameObject, Bodies.Galaxy.SystemContainer>();
            foreach (Bodies.Galaxy.SystemContainer sys in Bodies.Galaxy.systems)
            {
                displayedSystems.Add(CreateSystem(sys, sys), sys);
            }
            CreateEcliptica();
            theater = new Theater(Render);
            theater.SetCenter(Vector3.zero);
            PlaceMarkers();
            master.SetActive(false);
        }

        public static void Render()
        {

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

        static GameObject CreateSystem(Bodies.StarSystem sys, Vector3 pos)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = sys.ToString();
            go.transform.SetParent(master.transform);
            go.transform.position = pos;
            SystemScript ss = go.AddComponent<SystemScript>();
            ss.parent = sys;
            Material mat = go.GetComponent<MeshRenderer>().material;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_Color", Data.Graphics.Color_.FromTemperature(sys.Primary.Temperature));
            mat.SetColor("_EmissionColor", Data.Graphics.Color_.FromTemperature(sys.Primary.Temperature));
            go.tag = "Inspectable";
            return go;
        }

        static void CreateEcliptica()
        {
            ecliptica = new GameObject("EclipticaMap");
            ecliptica.transform.SetParent(master.transform);
            Canvas c = ecliptica.AddComponent<Canvas>();
            c.renderMode = RenderMode.WorldSpace;
            ((RectTransform)c.transform).sizeDelta = new Vector2(40, 40);
            CanvasScaler cs = ecliptica.AddComponent<CanvasScaler>();
            cs.referenceResolution = new Vector2(400, 400);
            cs.referencePixelsPerUnit = 10;
            Image im = ecliptica.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("ecliptica");
            im.color = new Color(1, 1, 1, 0.5f);
        }

        static void PlaceMarkers()
        {
            foreach (Transform child in ecliptica.transform) GameObject.Destroy(child.gameObject);
            foreach (var sys in displayedSystems.Values)
            {
                Vector2 pos = (Vector2)((Vector3)sys - theater.center);
                if(pos.magnitude < 40)
                {
                    GameObject go = new GameObject("SwitchBoard", typeof(RectTransform));
                    go.transform.SetParent(ecliptica.transform);
                    RectTransform rt = go.transform as RectTransform;
                    rt.sizeDelta = new Vector2(1.6f, 1.6f);
                    rt.anchorMin = new Vector2(0.5f, 0.5f);
                    rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = pos;
                    Image im = go.AddComponent<Image>();
                    im.sprite = Data.Graphics.GetSprite("marker");
                }
            }
        }

        class SystemScript : MonoBehaviour
        {
            public Bodies.StarSystem parent;
        }
    }
}
