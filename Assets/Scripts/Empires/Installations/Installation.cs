using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Dictionary<Technology.Technology, Tuple<double, double>> prerequisites { get; private set; }

        public List<Modifier> modefiers { get; private set; }

        public Installation(string name, double costWork,
            Dictionary<Production.Stockpile.ResourceType, double> costResources, List<Modifier> modefiers)
        {
            this.name = name;
            this.costWork = costWork;
            this.costResources = costResources;
            this.modefiers = modefiers;
        }

        Installation() { }

        static List<Installation> installationList;

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
                // new ModParser.Signature("modefiers", ModParser.SignatueType.words, Enum.GetNames(typeof(Modifier.Name)).ToList()),
                new ModParser.Signature("modefiers", ModParser.SignatueType.list,
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
            Installation inst = new Installation();
            inst.name = i.name;
            inst.costWork = (double)i.entries.Find(e => e.Item1.id == "cost_work").Item2;
            inst.costResources = new Dictionary<Production.Stockpile.ResourceType, double>();
            if (i.entries.Find(e => e.Item1.id == "cost_resources").Item2 != null)
            {
                List<Tuple<string, double>> mods = (i.entries.Find(e => e.Item1.id == "cost_resources").Item2 as ModParser.Item).entries.
                    ConvertAll(e => e.Item2 as Tuple<string, object>).ConvertAll(e => new Tuple<string, double>(e.Item1, (double)e.Item2));
                mods.ForEach(r => inst.costResources.Add(new Production.Stockpile.ResourceType(r.Item1), r.Item2));
            }

            inst.modefiers = new List<Modifier>();
            if (i.entries.Find(e => e.Item1.id == "modefiers").Item2 != null)
            {
                List<Tuple<string, double>> res = (i.entries.Find(e => e.Item1.id == "modefiers").Item2 as ModParser.Item).entries.
                    ConvertAll(e => e.Item2 as Tuple<string, object>).ConvertAll(e => new Tuple<string, double>(e.Item1, (double)e.Item2));
                res.ForEach(r => inst.modefiers.Add(new Modifier(r.Item1, r.Item2)));
            }

            List<ModParser.Item> prerqs = i.entries.Find(e => e.Item1.id == "prerequisites").Item2 as List<ModParser.Item>;
            inst.prerequisites = new Dictionary<Technology.Technology, Tuple<double, double>>();
            if (prerqs != null)
            {
                prerqs.ForEach(p =>
                    inst.prerequisites.Add(Technology.Technology.FindTech(p.name),
                                        new Tuple<double, double>((double)p.entries.Find(e => e.Item1.id == "min").Item2,
                                                                  (double)p.entries.Find(e => e.Item1.id == "max").Item2)
                                       )
                               );
            }
            return inst;
        }
    }

    class InstallationInstance
    {

    }
}
