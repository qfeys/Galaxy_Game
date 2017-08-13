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

                    if(objectWeHit.tag == "Orbital")
                    {
                        DisplayManager.TheOne.SetInspector(objectWeHit);
                    }
                }
            }

            if(Input.mouseScrollDelta.y != 0)
            {
                DisplayManager.TheOne.ChangeZoom(-Input.mouseScrollDelta.y * scrollSensitivity);
            }

        }
    }
}
