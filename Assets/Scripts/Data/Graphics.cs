using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    static class Graphics
    {
        static Dictionary<string, Texture2D> dict;

        public static void LoadGraphics()
        {
            string path = @"Mods\";
            string[] allPaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);
            dict = new Dictionary<string, Texture2D>();
            for (int i = 0; i < allPaths.Length; i++)
            {
                byte[] data = File.ReadAllBytes(allPaths[i]);
                Texture2D t = new Texture2D(32, 32, TextureFormat.ARGB32, false) {
                    name = Path.GetFileNameWithoutExtension(allPaths[i])
                };
                t.LoadImage(data);
                dict.Add(Path.GetFileNameWithoutExtension(allPaths[i]), t);
            }
            Debug.Log("Loaded " + allPaths.Length + " graphics.");
        }

        public static Font GetStandardFont()
        {
            return Resources.Load<Font>(@"Fonts\Orbitron\orbitron-light");
        }

        internal static Sprite GetSprite(string name)
        {
            if (dict.ContainsKey(name))
            {
                Texture2D t = dict[name];
                Sprite sp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), 
                    new Vector2(0.5f, 0.5f), 150, 1, SpriteMeshType.Tight,new Vector4(9,9,9,9));
                return sp;
            }
            Debug.Log("Failed loading graphic: " + name);
            return Resources.Load<Sprite>(@"Graphics\Small button");
        }

        public static class Color_
        {
            public static Color text { get { return new Color(1, 1, 0.8f); } }
            public static Color activeText { get { return new Color(.5f, .5f, 0); } }

            /// <summary>
            /// Transforms a temperature in K to its black body color
            /// </summary>
            /// <param name="temperature"></param>
            /// <returns></returns>
            internal static Color FromTemperature(int temperature)
            {
                float red = 0;
                if (temperature <= 6600)
                    red = 1;
                else
                    red = Mathf.Clamp01((float)(2.38774 * Math.Pow(temperature - 6000, 0.133205)));

                float green = 0;
                if (temperature <= 6600)
                    green = Mathf.Clamp01((float)(0.39008157 * Math.Log(temperature) - 2.4282335));
                else
                    green = Mathf.Clamp01((float)(1.5998 * Math.Pow(temperature, -0.0755148)));

                float blue = 0;
                if (temperature <= 1900)
                    blue = 0;
                else if (temperature <= 6600)
                    blue = Mathf.Clamp01((float)(0.5432067 * Math.Log(temperature - 1000) - 1.196254));
                else
                    blue = 1;
                return new Color(red, green, blue);
            }
        }
    }
}
