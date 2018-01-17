using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Rendering
{
    /// <summary>
    /// A simple monobehaviour that allow you to drag a ui element (like a panel)
    /// </summary>
    public class Dragable : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            ((RectTransform)transform).anchoredPosition += eventData.delta;
        }
    }
}
