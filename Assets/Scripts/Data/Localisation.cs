using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// This is the class that takes care of all Localisation files.
    /// At the moment English is the only supported language.
    /// Take care that all files are in a folder named "Localisation".
    /// Take care the name of the file ends with "eng.txt".
    /// </summary>
    static public class Localisation
    {
        static Dictionary<string, string> eng = new Dictionary<string, string>();
        static string path = @"Mods\";

        /// <summary>
        /// Loads in all the localisation files in the path (see also SetPath())
        /// </summary>
        public static void Load()
        {
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
                        if (lines[j].Count() == 0)
                            continue;
                        if (lines[j].First() == '#')
                            continue;
                        string key = new string(lines[j].TakeWhile(c => c != ':').ToArray());
                        int index1 = lines[j].IndexOf('"');
                        int index2 = lines[j].IndexOf('"', index1 + 1);
                        string value = new string(lines[j].ToList().GetRange(index1 + 1, index2 - index1 - 1).ToArray());
                        dict.Add(key, value);
                    }
                }
                catch (IOException e)
                {
                    UnityEngine.Debug.LogError("IO Error in localisation: " + e);
                }
            }
        }

        /// <summary>
        /// Sets the path of the localisation library.
        /// Defaults to @"Mods\"
        /// Will search for folders named Localisation in all underlying directories
        /// Call this before Load()
        /// </summary>
        /// <param name="p"></param>
        public static void SetPath(string p)
        {
            path = p;
        }

        /// <summary>
        /// Get a string from the localisation files, according to the string id
        /// </summary>
        /// <param name="textID"></param>
        /// <returns></returns>
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
