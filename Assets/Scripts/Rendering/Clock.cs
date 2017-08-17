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
            tr.sizeDelta = new Vector2(200, 50);
            tr.anchorMin = new Vector2(1, 1);
            tr.anchorMax = new Vector2(1, 1);
            tr.pivot = new Vector2(1, 1);
            tr.anchoredPosition = new Vector2(0, -3);
            Image im = go.AddComponent<Image>();
            im.sprite = Data.Graphics.GetSprite("overview_window_bg");
            im.type = Image.Type.Sliced;

            TextBox text = new TextBox(go.transform, () => Simulation.God.Time.ToString("yyyy.MM.dd HH:mm:ss"), null, 16, TextAnchor.UpperCenter, Data.Graphics.Color_.text);

            Dictionary<string, TimeSpan> timeSteps = Simulation.God.timeSteps;

            int i = 0;
            foreach (var step in timeSteps)
            {
                GameObject Step = new GameObject("Tab", typeof(RectTransform));
                Step.transform.SetParent(go.transform);
                RectTransform trst = (RectTransform)Step.transform;
                trst.sizeDelta = new Vector2(30, 13);
                trst.anchorMin = new Vector2(0, 0);
                trst.anchorMax = new Vector2(0, 0);
                trst.pivot = new Vector2(0, 0);
                float x = 195 - (i % (timeSteps.Count / 2) + 1) * 190 / (timeSteps.Count / 2);
                float y = (i < timeSteps.Count / 2 ? 0 : 13) + 1;
                trst.anchoredPosition = new Vector2(x, y);
                Image img = Step.AddComponent<Image>();
                img.sprite = Data.Graphics.GetSprite("tab_image_low");
                img.raycastTarget = true;
                img.type = Image.Type.Sliced;
                img.fillCenter = true;

                TextBox textStep = new TextBox(Step.transform, step.Key, null, 8, TextAnchor.MiddleCenter);

                Step.AddComponent<Button>().onClick.AddListener(() => Simulation.God.deltaTime = step.Value);
                i++;
            }

        }
    }
}
