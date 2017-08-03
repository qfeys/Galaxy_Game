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

        public static class Color_ {
             public static Color text { get { return new Color(1, 1, 0.8f); } }
        }
    }
}
