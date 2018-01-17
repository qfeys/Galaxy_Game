using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    internal static class MouseOver
    {

        static GameObject go;
        static Text tx;

        const float SIZE_X = 200;
        const float SIZE_Y = 44;

        internal static void Create()
        {
            go = new GameObject("MouseOverText", typeof(RectTransform));
            go.transform.SetParent(GameObject.FindGameObjectWithTag("PassiveCanvas").transform, false);
            go.layer = SortingLayer.GetLayerValueFromName("Ignore Raycast");
            RectTransform tr = go.transform as RectTransform;
            tr.sizeDelta = new Vector2(SIZE_X, SIZE_Y);
            tr.pivot = new Vector2(0, 1);
            tr.anchorMin = new Vector2(0, 0);
            tr.anchorMax = new Vector2(0, 0);
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("mouseover_window_bg");
            im.type = Image.Type.Sliced;
            GameObject go2 = new GameObject("text", typeof(RectTransform));
            go2.transform.SetParent(go.transform, false);
            RectTransform tr2 = go2.transform as RectTransform;
            tr2.anchorMin = new Vector2(0, 0);
            tr2.anchorMax = new Vector2(1, 1);
            tr2.offsetMin = new Vector2(6, 6);
            tr2.offsetMax = new Vector2(-6, -6);
            tr2.anchoredPosition = new Vector2(0, 0);
            tx = go2.AddComponent<Text>();
            tx.font = Data.Graphics.GetStandardFont();
            tx.fontSize = 8;
            tx.color = Data.Graphics.Color_.text;
            tx.alignment = TextAnchor.UpperLeft;

            go.AddComponent<MouseOverScript>();
            go.SetActive(false);
        }

        internal static void Activate(string v)
        {
            go.SetActive(true);
            tx.text = v;
        }

        internal static void Deactivate()
        {
            go.SetActive(false);
        }

        class MouseOverScript : MonoBehaviour
        {
            private void Update()
            {
                ((RectTransform)transform).anchoredPosition = Input.mousePosition + new Vector3(10, -5);
            }
        }
    }
}