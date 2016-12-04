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
        }

        public void DisplayOrbital(Bodies.Orbital o)
        {
            transform.GetChild(0).GetComponent<Text>().text = o.ToString();
            string[] info = string.Join(";", new[] { o.Information(), "####", "####" }).Split(';');
            int i;
            for(i = 0; i<info.Length/2; i++)
            {
                if (transform.GetChild(1).childCount <= i)
                {
                    Transform tr = Instantiate(transform.GetChild(1).GetChild(0).gameObject).transform;
                    tr.SetParent(transform.GetChild(1));
                    tr.SetAsLastSibling();
                }
                transform.GetChild(1).GetChild(i).GetChild(0).GetComponent<Text>().text = info[2 * i];
                transform.GetChild(1).GetChild(i).GetChild(1).GetComponent<Text>().text = info[2 * i + 1];
                transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
            }
            while(transform.GetChild(1).childCount < i)
            {
                transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
                i++;
            }
            
            gameObject.SetActive(true);
        }
    }
}
