using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// This class handels loading graphics
    /// </summary>
    static class Graphics
    {
        static Dictionary<string, Texture2D> dict;
        static string path = @"Mods\";
        static Font standardFont;

        /// <summary>
        /// Sets the path of the graphics library.
        /// Defaults to @"Mods\"
        /// Will search for .png in all underlying directories
        /// Call this before LoadGraphics()
        /// </summary>
        /// <param name="p"></param>
        public static void SetPath(string p)
        {
            path = p;
        }

        public static void SetDefaultFont(Font font)
        {
            standardFont = font;
        }

        /// <summary>
        /// Loads in all the graphics files in the path (see also SetPath())
        /// </summary>
        public static void LoadGraphics()
        {
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
            Rendering.MouseOver.Create();
        }

        /// <summary>
        /// Get the default fond, see also SetDefaultFont()
        /// </summary>
        /// <returns></returns>
        public static Font GetStandardFont()
        {
            if (standardFont != null)
                return standardFont;
            return new Font("Arial");
        }

        /// <summary>
        /// Returns a sprite from a png in the graphics folder.
        /// </summary>
        /// <param name="name">The name of the file without the extension</param>
        /// <returns></returns>
        public static Sprite GetSprite(string name)
        {
            if (dict.ContainsKey(name))
            {
                Texture2D t = dict[name];
                Sprite sp = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f), 150, 1, SpriteMeshType.Tight, new Vector4(9, 9, 9, 9));
                return sp;
            }
            Debug.Log("Failed loading graphic: " + name);
            return Resources.Load<Sprite>(@"Graphics\Small button");
        }

        /// <summary>
        /// A custom class for color handeling
        /// </summary>
        public static class Color_
        {
            /// <summary>
            /// Default text color
            /// </summary>
            public static Color text { get; private set; }
            /// <summary>
            /// Default color for selected text
            /// </summary>
            public static Color activeText { get; private set; }

            static Color_()
            {
                text = new Color(1, 1, 0.8f);
                activeText = new Color(.5f, .5f, 0);
            }

            /// <summary>
            /// Sets the default text color
            /// </summary>
            /// <param name="col"></param>
            public static void SetTextColor(Color col)
            {
                text = col;
            }

            /// <summary>
            /// Sets the default color for selected text
            /// </summary>
            /// <param name="col"></param>
            public static void SetActiveTextColor(Color col)
            {
                activeText = col;
            }

            /// <summary>
            /// Transforms a temperature in K to its black body color
            /// </summary>
            /// <param name="temperature"></param>
            /// <returns></returns>
            public static Color FromTemperature(int temperature)
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
