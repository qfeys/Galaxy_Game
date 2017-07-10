using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Empires.Technology;
using System.IO;

namespace Assets.Scripts
{
    static class ModParser
    {
        static public void ParseAllFiles()
        {
            try
            {
                Academy.SetTechTree(readTechnology());
            }
            catch (ArgumentException e)
            {
                UnityEngine.Debug.LogException(e);
                Academy.SetTechTree(new List<Technology>());
            }
        }

        static List<Tuple<string,object>> Parse(string path)
        {
            string text = System.IO.File.ReadAllText(path);

            /// Step 1: subdivide the text into strings of either: 
            ///         -one word 
            ///         -open bracket '{' 
            ///         -close bracket '}' or 
            ///         -equality'='
            List<string> words = new List<string>();
            string currentWord = null;
            bool commentRunning = false;
            for (int i = 0; i < text.Count(); i++)
            {
                char c = text[i];
                if (c == '#')
                    commentRunning = true;
                if (c == '\n')
                    commentRunning = false;
                if (commentRunning)
                    continue;
                if (c == ' ' || c == '\n' || c == '\t' || c == '\r')
                {
                    if (currentWord != null)
                    {
                        words.Add(currentWord);
                        currentWord = null;
                    }
                }else if(c == '=' || c == '}' || c == '{')
                {
                    if (currentWord != null)
                    {
                        words.Add(currentWord);
                        currentWord = null;
                    }
                    words.Add(c.ToString());
                }
                else
                {
                    if (currentWord == null)
                    {
                        currentWord = c.ToString();
                    }
                    else
                    {
                        currentWord += c;
                    }
                }
            }
            if(currentWord != null)
            {
                words.Add(currentWord);
                currentWord = null;
            }

            /// Step 2: Put the list of strings into a data structure
            /// This structure is a list of Tuples
            /// The first object is always a string
            /// The second object is either a string or an similar list of Tuples

            // Check if bracket count is right
            if(words.Count(s=> s == "{") != words.Count(s => s == "}"))
            {
                throw new FormatException("The brackets in the file: " + path + " are not balanced. '{' is found " + words.Count(s => s == "{") +
                    "times while '}' is found " + words.Count(s => s == "}") + "times.");
            }

            return ExtractData(words, 0);

            throw new NotImplementedException();
        }

        static List<Tuple<string, object>> ExtractData(List<string> words, int startPoint)
        {
            List<Tuple<string, object>> data = new List<Tuple<string, object>>();
            int currentPos = startPoint;
            while (currentPos<words.Count)
            {
                if (words[currentPos] == "=")
                {
                    string prime = words[currentPos - 1];
                    if (prime == "=" || prime == "}" || prime == "{")
                        throw new ArgumentException("The given position (" + (currentPos - 1) + ") is not a word");
                    if (words[currentPos] != "=")
                        throw new ArgumentException("The given position (" + currentPos + ") is not '='");
                    if ((new[] { "=", "}" }).Contains(words[startPoint + 2]))
                        throw new ArgumentException("The word after the given position (" + currentPos + ") is invalid");
                    if (words[currentPos + 1] == "{")
                    {
                        data.Add(new Tuple<string, object>(prime, ExtractData(words, currentPos + 2)));
                    }
                    else
                    {
                        data.Add(new Tuple<string, object>(prime, words[currentPos + 1]));
                    }
                }
                else if (words[currentPos] == "{")
                {
                    // Skip over this bracket
                    int openBrackets = 1;
                    while (openBrackets > 0)
                    {
                        currentPos++;
                        if (words[currentPos] == "{")
                            openBrackets++;
                        else if (words[currentPos] == "}")
                            openBrackets--;
                    }
                }
                else if (words[currentPos] == "}")
                    break;

                currentPos++;
            }

            return data;
        }

        static List<Technology> readTechnology()
        {
            List<Tuple<string, object>> data = Parse(@"Mods\Core\Technology.txt");
            List<Technology> techs = new List<Technology>();
            for (int i = 0; i < data.Count; i++)
            {
                ProtoTechnology newTech = new ProtoTechnology();
                newTech.name = data[i].Item1;
                List<Tuple<string, object>> info = data[i].Item2 as List<Tuple<string, object>>;
                for (int j = 0; j < info.Count; j++)
                {
                    switch (info[j].Item1)
                    {
                    case "starting_tech":
                        break;
                    case "sector":
                        newTech.sector = (Academy.Sector)Enum.Parse(typeof(Academy.Sector), info[j].Item2 as string);
                        break;
                    case "prerequisites":
                        newTech.prerequisites = new Dictionary<Technology, Tuple<double, double>>();
                        List<Tuple<string, object>> preqs = info[j].Item2 as List<Tuple<string, object>>;
                        for (int k = 0; k < preqs.Count; k++)
                        {
                            Technology preq = techs.Find(t => t.name == preqs[k].Item1);
                            var numbers = preqs[k].Item2 as List<Tuple<string, object>>;
                            double min = double.Parse(numbers.Find(t => t.Item1 == "min").Item2 as string);
                            double max = double.Parse(numbers.Find(t => t.Item1 == "max").Item2 as string);
                            newTech.prerequisites.Add(preq, new Tuple<double, double>(min, max));
                        }
                        break;
                    case "max_progress":
                        newTech.maxKnowledge = double.Parse(info[j].Item2 as string);
                        break;
                    case "understanding":
                        newTech.roots = new Dictionary<Technology, double>();
                        List<Tuple<string, object>> roots = info[j].Item2 as List<Tuple<string, object>>;
                        for (int k = 0; k < roots.Count; k++)
                        {
                            Technology root = techs.Find(t => t.name == roots[k].Item1);
                            newTech.roots.Add(root, double.Parse(roots[k].Item2 as string));
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid field in the technology file: " + info[j].Item1);
                    }
                }
                techs.Add(newTech.ToTech());
            }
            return techs;
        }

        struct ProtoTechnology
        {
            public string name;
            public Academy.Sector sector;
            public Dictionary<Technology, Tuple<double, double>> prerequisites;
            public double maxKnowledge;
            public Dictionary<Technology, double> roots;
            public Technology ToTech()
            {
                if (prerequisites == null)
                    prerequisites = new Dictionary<Technology, Tuple<double, double>>();
                if (roots == null)
                    roots = new Dictionary<Technology, double>();
                return new Technology(name, sector, prerequisites, maxKnowledge, roots);
            }
        }
    }
}
