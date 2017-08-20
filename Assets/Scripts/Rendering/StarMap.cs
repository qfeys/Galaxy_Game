using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    static class StarMap
    {
        static GameObject master;
        static Dictionary<GameObject, Bodies.StarSystem> displayedSystems;
        static GameObject ecliptica;
        static bool isActive = false;

        public static void Init()
        {
            master = new GameObject("StarMap");
            displayedSystems = new Dictionary<GameObject, Bodies.StarSystem>();
            foreach (Bodies.Galaxy.SystemContainer sys in Bodies.Galaxy.systems)
            {
                displayedSystems.Add(CreateSystem(sys, sys), sys);
            }
            CreateEcliptica();
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
            ecliptica = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ecliptica.transform.SetParent(master.transform);
            ecliptica.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        class SystemScript : MonoBehaviour
        {
            public Bodies.StarSystem parent;
        }
    }
}
