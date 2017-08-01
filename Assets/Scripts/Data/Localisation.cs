using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assets.Scripts.Data
{
    static class Localisation
    {
        static Dictionary<string, string> eng = new Dictionary<string, string>();

        public static void Load()
        {
            string path = @"Mods\";
            List<string> mods = Directory.EnumerateDirectories(path).ToList();
            List<string> allPaths = mods.Select(m => Directory.GetFiles(m + @"\Localisation\", "*.txt", SearchOption.AllDirectories)).SelectMany(p => p.ToList()).ToList();
            for (int i = 0; i < allPaths.Count; i++)
            {
                Dictionary<string, string> dict;
                switch (allPaths[i].Substring(allPaths[i].Length - 7))
                {
                case "eng.txt":
                    dict = eng;
                    break;
                default:
                    throw new FileLoadException(allPaths[i] + " is not a valic localisation file. Trail it with a valid 3 letter language");
                }
                try
                {
                    string[] lines = File.ReadAllLines(allPaths[i]);
                    for (int j = 0; j < lines.Length; j++)
                    {
                        if (lines[j].First() == '#')
                            continue;
                        string key = new string(lines[j].TakeWhile(c => c != ':').ToArray());
                        int index1 = lines[j].IndexOf('"');
                        int index2 = lines[j].IndexOf('"', index1 + 1);
                        string value = new string(lines[j].ToList().GetRange(index1 + 1, index2 - index1 - 1).ToArray());
                        dict.Add(key, value);
                    }
                }catch(IOException e)
                {
                    Simulation.God.Log("IO Error in localisation: " + e);
                }
            }
        }

        public static string GetText(string textID)
        {
            if (eng.ContainsKey(textID))
            {
                return eng[textID];
            }
            // Simulation.God.Log("Localisation missing: " + textID);
            return textID;
        }
    }
}
