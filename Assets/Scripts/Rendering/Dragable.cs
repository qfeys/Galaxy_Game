using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Rendering
{
    class Dragable : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition += eventData.delta;
        }
    }
}
