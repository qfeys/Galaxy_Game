using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    class InputManager : MonoBehaviour
    {
        public float scrollSensitivity = 0.05f;

        Vector3 lastMousePos;
        Vector2 camRotSys = Vector2.zero;

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Camera theCamera = Camera.main;
                Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray, out hitInfo))
                {
                    GameObject objectWeHit = hitInfo.collider.gameObject;

                    if(objectWeHit.tag == "Inspectable")
                    {
                        if (objectWeHit.GetComponent<SystemRenderer.PlanetScript>() != null)
                        {
                            Inspector.DisplayPlanet(objectWeHit.GetComponent<SystemRenderer.PlanetScript>().parent);
                            SystemRenderer.SetCenter(objectWeHit.GetComponent<SystemRenderer.PlanetScript>().parent);
                        }
                        if (objectWeHit.GetComponent<SystemRenderer.StarScript>() != null)
                            Inspector.DisplayStar(objectWeHit.GetComponent<SystemRenderer.StarScript>().parent);
                    }
                }
                lastMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                SystemRenderer.MoveCenter(lastMousePos - Input.mousePosition);
                lastMousePos = Input.mousePosition;
            }

            // camera rotation
            if (Input.GetMouseButtonDown(1))
                lastMousePos = Input.mousePosition;

            if (Input.GetMouseButton(1) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                SystemRenderer.CamRot += (Vector2)(lastMousePos - Input.mousePosition) * 0.01f;
                SystemRenderer.PlaceSystemCamera();
                lastMousePos = Input.mousePosition;
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                DisplayManager.TheOne.ChangeZoom(-Input.mouseScrollDelta.y * scrollSensitivity);
            }

        }
    }
}
