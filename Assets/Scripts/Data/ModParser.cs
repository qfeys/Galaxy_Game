using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Assets.Scripts;

namespace Assets.Scripts.Data
{
    static class ModParser
    {
        static public void ParseAllFiles()
        {
            Dictionary<string, List<Signature>> signatures = CollectSignatures();
            //string path = @"Mods\Core\Technology.txt";
            string path = @"Mods\";
            List<string> mods = Directory.EnumerateDirectories(path).ToList();
            List<string> allPaths = mods.Select(m => Directory.GetFiles(m + @"\Common\", "*.txt", SearchOption.AllDirectories)).SelectMany(p => p.ToList()).ToList();
            try
            {
                // parse all the paths and put the list of strings that is the result of it into this array
                List<List<string>> words = allPaths.ConvertAll(p => Parse(p));
                // Convert every stringlist in that array into a datatree and put it in this new array
                List<List<Tuple<string, object>>> dataTree = words.ConvertAll(w => ExtractData(w, 0));
                // Put all the datatrees in a single collection
                List<Tuple<string, object>> allTrees = dataTree.SelectMany(t => t).ToList();
                foreach (Tuple<string, object> tree in allTrees.FindAll(t => t.Item1 == "technology"))
                {
                    List<Item> itemList = ConvertToItems(tree.Item2 as List<Tuple<string, object>>, signatures["technology"]);
                    Empires.Technology.Technology.SetTechTree(itemList);
                }
                foreach (Tuple<string, object> tree in allTrees.FindAll(t => t.Item1 == "installations"))
                {
                    List<Item> itemList = ConvertToItems(tree.Item2 as List<Tuple<string, object>>, signatures["installations"]);
                    Empires.Installations.Installation.SetInstallationList(itemList);
                }
            }
            catch (FormatException e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        static Dictionary<string, List<Signature>> CollectSignatures()
        {
            Dictionary<string, List<Signature>> signatures = new Dictionary<string, List<Signature>> {
                { "technology", Empires.Technology.Technology.Signature},
                { "installations", Empires.Installations.Installation.Signature}
            };
            return signatures;
        }

        static List<string> Parse(string path)
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
                    " times while '}' is found " + words.Count(s => s == "}") + " times.");
            }

            return words;
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
                        throw new FormatException("The given position (" + (currentPos - 1) + " - " + words[currentPos - 1] + ") is not a word");
                    if (words[currentPos] != "=")
                        throw new FormatException("The given position (" + currentPos + " - " + words[currentPos] + ") is not '='");
                    if (words[currentPos + 2] == "=")
                        throw new FormatException("The word after the given position (" + currentPos + " - " + words[currentPos + 1] + ") is invalid");
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
                    // Check if this was part of a valid statement, aka there is a '=' in front of it
                    if (words[currentPos - 1] != "=")
                        throw new FormatException("An open bracket ('{') was not precluded by an equal sign ('=').");
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
                else    // This is a normal word
                {       // We test if it is part of a correct statement, aka flanked by 1 '=' sign.
                    if(currentPos != 0 && currentPos != words.Count-1 && ((words[currentPos - 1] == "=") ^ (words[currentPos + 1] == "=" ))== false)
                    {
                        throw new FormatException("The word " + words[currentPos] + " is not part of a valid statement.");
                    }
                }

                currentPos++;
            }

            return data;
        }

        static List<Item> ConvertToItems(List<Tuple<string, object>> dataTree, List<Signature> signatures)
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < dataTree.Count; i++)
            {
                Item newItem = new Item(signatures) {
                    name = dataTree[i].Item1
                };
                List<Tuple<string, object>> info = dataTree[i].Item2 as List<Tuple<string, object>>;
                newItem = ConvertToItem(info, signatures);
                newItem.name = dataTree[i].Item1;
                items.Add(newItem);
            }
            return items;
        }

        static Item ConvertToItem(List<Tuple<string,object>> dataTree, List<Signature> signatures)
        {
            Item newItem = new Item(signatures);
            bool varName = signatures.Any(s => s.id == null) ? true : false;
            if(varName && signatures.Count != 1)
                throw new Exception("There are multiple null named signatures in " + newItem.name);
            if (varName)
                newItem = new Item(new List<Signature>());

            foreach (Tuple<string, object> entry in dataTree)
            {
                Signature signtr = null;
                if (varName)
                {
                    signtr = signatures[0];
                }
                else
                {
                    signtr = signatures.Find(s => s.id == entry.Item1);
                }
                object date = null;
                switch (signtr.type)
                {
                case SignatueType.boolean:
                    date = bool.Parse(entry.Item2 as string);
                    break;
                case SignatueType.integer:
                    date = int.Parse(entry.Item2 as string);
                    break;
                case SignatueType.floating:
                    date = double.Parse(entry.Item2 as string);
                    break;
                case SignatueType.words:
                    if (signtr.wordOptions != null)
                    {
                        if (signtr.wordOptions.Contains(entry.Item2 as string) == false)
                            throw new FormatException(entry.Item2 + " is not a valid option for " + entry.Item1);
                    }
                    date = entry.Item2 as string;
                    break;
                case SignatueType.list:
                    if (signtr.listSignatures == null)
                    {
                        throw new Exception(entry.Item1 + " does not have a valid signature. The sublist signatures are not given.");
                    }
                    date = ConvertToItem(entry.Item2 as List<Tuple<string, object>>, signtr.listSignatures);
                    break;
                }
                if (varName)  // Variable name entries - we must pass the name
                    newItem.entries.Add(new Tuple<Signature, object>(signtr, new Tuple<string, object>(entry.Item1, date)));
                else
                    newItem.SetEntry(signtr, date);
            }


            return newItem;
        }

        public enum SignatueType { boolean, words, integer, floating, list}

        public class Signature
        {
            public string id;
            public SignatueType type;
            public List<string> wordOptions;
            public List<Signature> listSignatures;

            public Signature(string id, SignatueType type)
            {
                if (type == SignatueType.words || type == SignatueType.list)
                    throw new ArgumentException("You used the wrong constructor for the signature " + id);
                this.id = id; this.type = type;
                wordOptions = null; listSignatures = null;
            }

            public Signature(string id, SignatueType type, List<string> wordOptions)
            {
                if (type != SignatueType.words)
                    throw new ArgumentException("You used the wrong constructor for the signature " + id);
                this.id = id; this.type = type;
                this.wordOptions = wordOptions; listSignatures = null;
            }

            public Signature(string id, SignatueType type, List<Signature> listSignatures)
            {
                if (type != SignatueType.list)
                    throw new ArgumentException("You used the wrong constructor for the signature " + id);
                this.id = id; this.type = type;
                wordOptions = null; this.listSignatures = listSignatures;
            }
        }

        public class Item
        {
            public string name;
            public List<Tuple<Signature, object>> entries;

            public Item(List<Signature> signatures)
            {
                entries = new List<Tuple<Signature, object>>();
                signatures.ForEach(s => entries.Add(new Tuple<Signature, object>(s, null)));
            }

            public void SetEntry(Signature signtr, object data)
            {
                if (entries.Any(tpl => tpl.Item1 == signtr) == false)
                    throw new FormatException("The item '" + name + "' should not have the entry '" + signtr.id + "'.");
                entries.Find(e => e.Item1 == signtr).Item2 = data;
            }
        }
    }
}
