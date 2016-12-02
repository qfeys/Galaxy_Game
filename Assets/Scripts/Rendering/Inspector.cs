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
            transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            gameObject.SetActive(false);
        }

        public void DisplayOrbital(Bodies.Orbital o)
        {
            transform.GetChild(0).GetComponent<Text>().text = o.ToString(); 
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = o.Mass.ToString("e3");
            transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = o.Elements.SMA.ToString("e3");
            gameObject.SetActive(true);
        }
    }
}
