using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    class Inspector : MonoBehaviour
    {
        public void Start()
        {
            gameObject.SetActive(false);
        }

        public void DisplayOrbital(Bodies.Orbital o)
        {
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = o.Mass.ToString();
            transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = o.Elements.SMA.ToString();
        }
    }
}
