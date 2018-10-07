using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    class EventLogWindow
    {
        static GameObject go;

        internal static void Create(GameObject canvas)
        {
            go = new GameObject("Event Log", typeof(RectTransform));
            go.transform.SetParent(canvas.transform);
            RectTransform tr = (RectTransform)go.transform;
            tr.sizeDelta = new Vector2(400, 600);
            UI_Window.TopRight(tr, new Vector2(-100, -100));
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;
            go.AddComponent<Dragable>();
            go.SetActive(false);

            // close button
            UI_Window.CreateCloseButton(go, CloseLog);

            // Title
            {
                GameObject titleGo = new GameObject("Title", typeof(RectTransform));
                titleGo.transform.SetParent(go.transform);
                RectTransform trTtl = (RectTransform)titleGo.transform;
                trTtl.sizeDelta = new Vector2(0, 26);
                UI_Window.TopStretch(trTtl, new Vector2(0, -10));
                TextBox text = new TextBox(trTtl, "Event Log", 24, TextAnchor.UpperCenter);
            }

            // Table
            {
                InfoTable table = InfoTable.Create(go.transform, () => Simulation.Schedule.EventList,
                    ev => new List<TextRef>() { TextRef.Create(() => ev.moment), ev.ToString() },
                    350, new List<TextRef>() { "Moment", "text?" });
                UI_Window.TopCenter(table.transform, new Vector2(0, -30));
            }
        }

        public static void OpenLog()
        {
            go.SetActive(true);
        }

        public static void CloseLog()
        {
            go.SetActive(false);
        }

        public static void ToggleLog()
        {
            go.SetActive(!go.activeSelf);
        }
    }
}
