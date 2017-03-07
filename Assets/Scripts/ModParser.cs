using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Empires.Technology;

namespace Assets.Scripts
{
    class ModParser
    {
        public static List<Technology> readTechnology()
        {
            string text = System.IO.File.ReadAllText(@"\Mods\Core\Technology.txt");
            Stack<object> stack = new Stack<object>();
            string currentWord = null;
            ProtoTechnology newTech = null;
            int currentLine = 1;
            for (int i = 0; i < text.Count(); i++)
            {
                char c = text[i];
                if (c == '\n') currentLine++;
                if (currentWord == null)
                {
                    if (c == ' ' || c == '\n' || c == '\t' || c == '\r' || c == '=' || c == '}')
                    {

                    }
                    else if (c == '{')
                    {
                        throw new FormatException("Bad bracket at line " + currentLine);
                    }
                    else
                    {
                        stack.Push(c.ToString());
                    }
                }
                else 
                {
                    if (c == ' ' || c == '\n' || c == '\t' || c == '\r' || c == '=' || c == '}')
                    {
                        if(newTech == null)
                        {
                            newTech = new ProtoTechnology();
                            newTech.name = currentWord;
                            currentWord = null;
                        }
                        else
                        {

                        }
                        // Consolidate text into info
                    }
                    else if (c == '{')
                    {
                        throw new FormatException("Bad bracket at line " + currentLine);
                    }
                    else
                    {
                        currentWord += c;
                    }
                }
            }

            throw new NotImplementedException();
        }

        struct ProtoTechnology
        {
            public string name;

        }
    }
}
