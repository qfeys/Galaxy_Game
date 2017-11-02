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
        static List<Item> modData;

        static public void ParseAllFiles()
        {
            string path = @"Mods\";
            List<string> mods = Directory.EnumerateDirectories(path).ToList();
            List<string> allPaths = mods.Select(m => Directory.GetFiles(m + @"\Common\", "*.txt", SearchOption.AllDirectories)).SelectMany(p => p.ToList()).ToList();
            try
            {
                // parse all the paths and put the list of strings that is the result of it into this array
                Dictionary<List<StringAndLoc>, string> words = allPaths.ToDictionary(p => Parse(p));
                // Convert every stringlist into a single Item
                modData = words.ToList().ConvertAll(w => ExtractData(w.Key, w.Value));
            }
            catch (Item.BadModException e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        /// <summary>
        /// This function takes a file and extracts a list of all the words, comments not included,
        /// together with their line location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<StringAndLoc> Parse(string path)
        {
            List<string> lines = File.ReadLines(path).ToList();

            /// Step 1: subdivide the text into strings of either: 
            ///         -one word 
            ///         -open bracket '{' 
            ///         -close bracket '}' or 
            ///         -equality'='
            List<StringAndLoc> words = new List<StringAndLoc>();
            for(int i = 0; i < lines.Count; i++)
            {
                string currentWord = null;
                for (int j = 0; j < lines[i].Length; j++)
                {
                    char c = lines[i][j];
                    if (c == '#')
                        break;
                    if (c == ' ' || c == '\n' || c == '\t' || c == '\r')
                    {
                        if (currentWord != null)
                        {
                            words.Add(new StringAndLoc(currentWord, i + 1));
                            currentWord = null;
                        }
                    }
                    else if (c == '=' || c == '}' || c == '{')
                    {
                        if (currentWord != null)
                        {
                            words.Add(new StringAndLoc(currentWord, i + 1));
                            currentWord = null;
                        }
                        words.Add(new StringAndLoc(c.ToString(), i + 1));
                    }
                    else
                    {
                        if (currentWord == null)
                            currentWord = c.ToString();
                        else
                            currentWord += c;
                    }
                }

                if (currentWord != null)
                {
                    words.Add(new StringAndLoc(currentWord, i + 1));
                }

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

        /// <summary>
        /// Takes the list of words and converts it in an item, assuming the items starts
        /// at the word nr. startpoint.
        /// </summary>
        /// <param name="words"></param>
        /// <param name="path"></param>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        static Item ExtractData(List<StringAndLoc> words, string path, int startPoint = 0)
        {
            Item master = Item.ListItem(words[startPoint], path, words[startPoint].loc);
            if (words[startPoint + 1] != "=" || words[startPoint+2] != "{")
                throw new Item.BadModException(master);
            int currentPos = startPoint + 3;
            while(currentPos < words.Count)
            {
                if(words[currentPos] == "}") // We have reached the end of this Item
                {
                    return master;
                }
                else if (words[currentPos + 2] == "{") {     // This is a new list item
                    master.AddItem(ExtractData(words, path, currentPos));
                    currentPos += 3;
                    int openBrackets = 1;
                    while (openBrackets > 0)    // Find the end of this section
                    {
                        if (words[currentPos] == "{") openBrackets++;
                        else if (words[currentPos] == "}") openBrackets--;
                        currentPos++;
                    }
                }
                else // This is a new value item
                {
                    Item n = master.AddItem(words[currentPos], words[currentPos + 2], path, words[currentPos].loc);
                    if (words[currentPos + 1] != "=") throw new Item.BadModException(n);
                    currentPos += 3;
                }
            }
            throw new Item.BadModException(master); // The function should terminate within the loop
        }

        static public Item RetriveMasterItem(string name)
        {
            Item master = null;
            for (int i = 0; i < modData.Count; i++)
            {
                if (modData[i].name == name)
                    master = master == null ? modData[i] : master.Merge(modData[i]);
            }
            return master;
        }

        /// <summary>
        /// Container class for a combination of a string and an int.
        /// Is an implicite string
        /// </summary>
        class StringAndLoc
        {
            public readonly string s;
            public readonly int loc;
            public StringAndLoc(string s, int loc) { this.s = s; this.loc = loc; }
            public static implicit operator string(StringAndLoc sl) { return sl.s; }
        }

        /// <summary>
        /// An element in a mod file. It contains either a value or a list with Items
        /// </summary>
        public class Item
        {
            public readonly string name;
            string value;
            List<Item> list;
            bool isValue;
            public readonly Location location;

            Item(string name, string path, int line)
            {
                this.name = name;
                location = new Location() { path = path, line = line };
            }

            public static Item ValueItem(string name, string value, string path, int line)
            {
                return new Item(name, path, line) {
                    value = value,
                    isValue = true
                };
            }

            public static Item ListItem(string name, string path, int line)
            {
                return new Item(name, path, line) {
                    list = new List<Item>(),
                    isValue = false
                };
            }

            public Item AddItem(string name, string value, string path, int line)
            {
                if (isValue) throw new BadModException(this);
                Item ret = ValueItem(name, value, path, line);
                list.Add(ret);
                return ret;
            }

            public Item AddItem(Item item)
            {
                if (isValue) throw new BadModException(this);
                list.Add(item);
                return item;
            }

            internal Item Merge(Item item)
            {
                if (this == null) return item;
                if (item == null) return this;
                if (item.isValue || this.isValue)
                    throw new Exception("You are trying to merge value items. You can only merge list items");
                Item merge = new Item(this.name, null, 0) {
                    isValue = false,
                    list = new List<Item>(this.list)
                };
                foreach (Item it in item.list)
                {
                    merge.list.RemoveAll(i => i.name == it.name);
                    merge.list.Add(it);
                }
                return merge;
            }

            internal List<Item> GetChilderen()
            {
                if (isValue) throw new Exception("You cannot retrive the childeren of a value object");
                return list;
            }

            internal string GetString()
            {
                if (isValue == false) throw new Exception("You cannot retrive a value from a list item");
                return value;
            }

            internal double GetNumber()
            {
                if (isValue == false) throw new Exception("You cannot retrive a value from a list item");
                return double.Parse(value);
            }

            internal bool GetBool()
            {
                if (isValue == false) throw new Exception("You cannot retrive a value from a list item");
                return bool.Parse(value);
            }

            /// <summary>
            /// Returns the enum value of this item.
            /// BEWARE: this function will do no exception handeling
            /// </summary>
            /// <typeparam name="T">This is the type of the Enum</typeparam>
            /// <returns></returns>
            internal T GetEnum<T>()
            {
                if (isValue == false) throw new Exception("You cannot retrive a value from a list item");
                return (T)Enum.Parse(typeof(T),value);
            }

            internal Item GetItem(string valueName)
            {
                if (isValue) throw new Exception("This function must be used on a list item");
                return list.Find(i => i.name == valueName);
            }

            internal string GetString(string valueName)
            {
                if (isValue) throw new Exception("This function must be used on a list item");
                if (list.Any(i => i.name == valueName))
                    return list.Find(i => i.name == valueName).GetString();
                return "";
            }

            internal double GetNumber(string valueName)
            {
                if (isValue) throw new Exception("This function must be used on a list item");
                if (list.Any(i => i.name == valueName))
                    return list.Find(i => i.name == valueName).GetNumber();
                return 0;
            }

            internal bool GetBool(string valueName, bool _default = false)
            {
                if (isValue) throw new Exception("This function must be used on a list item");
                if (list.Any(i => i.name == valueName))
                    return list.Find(i => i.name == valueName).GetBool();
                return _default;
            }

            internal T GetEnum<T>(string valueName)
            {
                if (isValue) throw new Exception("This function must be used on a list item");
                if (list.Any(i => i.name == valueName))
                    return list.Find(i => i.name == valueName).GetEnum<T>();
                return default(T);
            }



            public struct Location
            {
                public string path;
                public int line;
            }


            [Serializable]
            public class BadModException : Exception
            {
                public BadModException(Item item) : base("There is a problem in file " + item.location.path + " around line "+ item.location.line + ".") { }
                protected BadModException(
                  System.Runtime.Serialization.SerializationInfo info,
                  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
            }
        }
    }
}
