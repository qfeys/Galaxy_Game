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
            for (int i = 0; i < text.Count(); i++)
            {
                char c = text[i];
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

        public static List<Technology> readTechnology()
        {
            List < Tuple < string,object>> data = Parse(@"\Mods\Core\Technology.txt");
            

            throw new NotImplementedException();
        }

        struct ProtoTechnology
        {
            public string name;

        }
    }
}
