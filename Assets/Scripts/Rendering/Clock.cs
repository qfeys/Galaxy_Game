using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Rendering
{
    static class Clock
    {
        static GameObject go;

        internal static void Create(GameObject canvas)
        {
            go = new GameObject("Clock", typeof(RectTransform));
            go.transform.SetParent(canvas.transform);
            RectTransform tr = (RectTransform)go.transform;
            tr.sizeDelta = new Vector2(230, 50);
            UI_Window.TopRight(tr);
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;

            TextBox text = new TextBox(go.transform, TextRef.Create(() => Simulation.God.Time.ToString("yyyy.MM.dd HH:mm:ss")), 16, TextAnchor.UpperCenter, Data.Graphics.Color_.text);
            text.transform.anchoredPosition = new Vector2(0, -3);

            Dictionary<string, TimeSpan> timeSteps = Simulation.God.timeSteps;

            int i = 0;
            foreach (var step in timeSteps)
            {
                GameObject Step = new GameObject("Tab", typeof(RectTransform));
                Step.transform.SetParent(go.transform);
                RectTransform trst = (RectTransform)Step.transform;
                trst.sizeDelta = new Vector2(30, 13);
                UI_Window.BottomLeft(trst);
                float x = 195 - (i % (timeSteps.Count / 2) + 1) * 190 / (timeSteps.Count / 2);
                float y = (i < timeSteps.Count / 2 ? 0 : 14) + 1;
                trst.anchoredPosition = new Vector2(x, y);
                Image img = Step.AddComponent<Image>();
                img.sprite = Data.Graphics.GetSprite("tab_image_low");
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                TextBox textStep = new TextBox(Step.transform, TextRef.Create(step.Key, false), 8, TextAnchor.MiddleCenter);

                Step.AddComponent<Button>().onClick.AddListener(() => Simulation.God.deltaTime = step.Value);
                i++;
            }
            GameObject pause = new GameObject("Pause", typeof(RectTransform));
            pause.transform.SetParent(go.transform);
            RectTransform trps = (RectTransform)pause.transform;
            trps.sizeDelta = new Vector2(28, 24);
            UI_Window.BottomRight(trps, new Vector2(-5, 2));
            Image imP = pause.AddComponent<Image>();
            imP.sprite = Data.Graphics.GetSprite("tab_image_low");
            imP.raycastTarget = true;
            imP.type = Image.Type.Sliced;
            imP.fillCenter = true;

            TextBox textPause = new TextBox(pause.transform, TextRef.Create("||", false), 10, TextAnchor.MiddleCenter);

            pause.AddComponent<Button>().onClick.AddListener(() => Simulation.God.Pause());
        }
    }
}
