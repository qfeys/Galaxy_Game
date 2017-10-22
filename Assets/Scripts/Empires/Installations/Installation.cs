using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Data;

namespace Assets.Scripts.Empires.Installations
{
    /// <summary>
    /// All installations in population centers or in space. NOTE: for installations that are free floating, use a child of this.
    /// </summary>
    class Installation
    {
        public string name;
        public double costWork;
        public Dictionary<Production.Stockpile.ResourceType, double> costResources;
        public Dictionary<Technology.Technology, Tuple<double, double>> Prerequisites { get; private set; }

        public List<Modifier> Modefiers { get; private set; }

        static List<Installation> installationList;

        public Installation(string name, double costWork,
            Dictionary<Production.Stockpile.ResourceType, double> costResources, List<Modifier> modifiers)
        {
            this.name = name;
            this.costWork = costWork;
            this.costResources = costResources;
            this.Modefiers = modifiers;
        }

        Installation() { }

#region parsing

        static public void SetInstallationList(List<ModParser.Item> itemList)
        {
            if (installationList == null)
            {
                UnityEngine.Debug.Log("Installation List set with " + itemList.Count + " installations.");
            }
            else
            {
                UnityEngine.Debug.LogError("Installation List reset with " + itemList.Count + " installations.");
            }
            installationList = itemList.ConvertAll(i => Interpret(i));
        }

        internal static List<ModParser.Signature> Signature
        {
            get
            {
                List<ModParser.Signature> ret = new List<ModParser.Signature> {
                new ModParser.Signature("cost_work", ModParser.SignatueType.floating),
                new ModParser.Signature("cost_resources", ModParser.SignatueType.list,
                new List<ModParser.Signature>() { new ModParser.Signature(null, ModParser.SignatueType.floating) }),
                // new ModParser.Signature("modifiers", ModParser.SignatueType.words, Enum.GetNames(typeof(Modifier.Name)).ToList()),
                new ModParser.Signature("modifiers", ModParser.SignatueType.list,
                    new List<ModParser.Signature>(){ new ModParser.Signature(null,ModParser.SignatueType.floating) }),
                new ModParser.Signature("prerequisites", ModParser.SignatueType.list,
                new List<ModParser.Signature>() { new ModParser.Signature(null, ModParser.SignatueType.list,
                    new List<ModParser.Signature>() { new ModParser.Signature("min", ModParser.SignatueType.floating),
                                                    new ModParser.Signature("max", ModParser.SignatueType.floating)})
                })
            };
                return ret;
            }
        }

        private static Installation Interpret(ModParser.Item i)
        {
            Installation inst = new Installation() {
                name = i.name,
                costWork = (double)i.entries.Find(e => e.Item1.id == "cost_work").Item2,
                costResources = new Dictionary<Production.Stockpile.ResourceType, double>()
            };
            if (i.entries.Find(e => e.Item1.id == "cost_resources").Item2 != null)
            {
                List<Tuple<string, double>> mods = (i.entries.Find(e => e.Item1.id == "cost_resources").Item2 as ModParser.Item).entries.
                    ConvertAll(e => e.Item2 as Tuple<string, object>).ConvertAll(e => new Tuple<string, double>(e.Item1, (double)e.Item2));
                mods.ForEach(r => inst.costResources.Add(Production.Stockpile.ResourceType.Get(r.Item1), r.Item2));
            }

            inst.Modefiers = new List<Modifier>();
            if (i.entries.Find(e => e.Item1.id == "modifiers").Item2 != null)
            {
                List<Tuple<string, double>> res = (i.entries.Find(e => e.Item1.id == "modifiers").Item2 as ModParser.Item).entries.
                    ConvertAll(e => e.Item2 as Tuple<string, object>).ConvertAll(e => new Tuple<string, double>(e.Item1, (double)e.Item2));
                res.ForEach(r => inst.Modefiers.Add(new Modifier(r.Item1, r.Item2)));
            }

            List<ModParser.Item> prerqs = i.entries.Find(e => e.Item1.id == "prerequisites").Item2 as List<ModParser.Item>;
            inst.Prerequisites = new Dictionary<Technology.Technology, Tuple<double, double>>();
            if (prerqs != null)
            {
                prerqs.ForEach(p =>
                    inst.Prerequisites.Add(Technology.Technology.FindTech(p.name),
                                        new Tuple<double, double>((double)p.entries.Find(e => e.Item1.id == "min").Item2,
                                                                  (double)p.entries.Find(e => e.Item1.id == "max").Item2)
                                       )
                               );
            }
            return inst;
        }

#endregion

        public static Installation GetCopy(string name)
        {
            if(installationList.Any(i=> i.name == name))
            {
                return installationList.Find(i => i.name == name);
            }
            throw new ArgumentException("The installation " + name + " is not in the list.");
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return name.Equals(((Installation)obj).name);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
