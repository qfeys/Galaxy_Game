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
                Debug.Log("Button");
                Camera theCamera = Camera.main;
                Ray ray = theCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray, out hitInfo))
                {
                    Debug.Log("Ray started");
                    GameObject objectWeHit = hitInfo.collider.gameObject;

                    if(objectWeHit.tag == "Orbital")
                    {
                        Debug.Log("succesful ray");
                        DisplayManager.TheOne.SetInspector(objectWeHit);
                    }
                }
            }

            if(Input.mouseScrollDelta.y != 0)
            {
                DisplayManager.TheOne.ChangeZoom(Input.mouseScrollDelta.y * scrollSensitivity);
            }

        }
    }
}
