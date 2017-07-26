using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Data;

namespace Assets.Scripts.Empires.Technology
{
    class Technology
    {
        public string Name { get; private set; }
        public Sector? sector { get; private set; }
        public Dictionary<Technology, Tuple<double,double>> Prerequisites { get; private set; } // use the max of knowledge and understanding
        public double MaxKnowledge { get; private set; }
        public Dictionary<Technology, double> Roots { get; private set; }             // These techs will increase in understanding by using this tech.

        static List<Technology> techTree;
        internal static List<Technology> TechTree { get { return techTree; } }

        internal enum Sector { PHYSICS, DRIVE }

        private Technology() { }

        public Technology(string name, Sector sector, Dictionary<Technology, Tuple<double, double>> prerequisites,
            double maxKnowledge, Dictionary<Technology, double> roots)
        {
            Name = name;this.sector = sector; Prerequisites = prerequisites; MaxKnowledge = maxKnowledge; Roots = roots;
        }

        public Technology(Technology clone)
        {
            Name = clone.Name; sector = clone.sector; Prerequisites = clone.Prerequisites; MaxKnowledge = clone.MaxKnowledge; Roots = clone.Roots;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Technology) && ((Technology)obj).Name == Name)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        internal static void SetTechTree(List<ModParser.Item> itemList)
        {
            if (techTree == null)
            {
                UnityEngine.Debug.Log("Techtree set with " + itemList.Count + " technologies.");
            }
            else
            {
                UnityEngine.Debug.LogError("Techtree reset with " + itemList.Count + " technologies.");
            }
            List<Technology> tt = itemList.ConvertAll(i => Interpret(i));
            PointPrerequisitesAndRoots(tt);
            techTree = tt;
        }

        internal static Technology FindTech(string name)
        {
            return techTree.Find(t => t.Name == name);
        }

        internal static List<ModParser.Signature> Signature
        {
            get
            {
                List<ModParser.Signature> ret = new List<ModParser.Signature> {
                    new ModParser.Signature("starting_tech", ModParser.SignatueType.boolean),
                    new ModParser.Signature("sector", ModParser.SignatueType.words, Enum.GetNames(typeof(Sector)).ToList()),
                    new ModParser.Signature("max_progress", ModParser.SignatueType.floating),
                    new ModParser.Signature("prerequisites", ModParser.SignatueType.list,
                    new List<ModParser.Signature>() { new ModParser.Signature(null, ModParser.SignatueType.list,
                    new List<ModParser.Signature>() { new ModParser.Signature("min", ModParser.SignatueType.floating),
                                                    new ModParser.Signature("max", ModParser.SignatueType.floating)})
                    }),
                    new ModParser.Signature("understanding", ModParser.SignatueType.list,
                    new List<ModParser.Signature>() { new ModParser.Signature(null, ModParser.SignatueType.floating) }
                    )
                };
                return ret;
            }
        }

        internal static Technology Interpret(ModParser.Item i)
        {
            Technology t = new Technology() {
                Name = i.name,
                sector = (Sector)Enum.Parse(typeof(Sector), i.entries.Find(e => e.Item1.id == "sector").Item2 as string),
                MaxKnowledge = (double)i.entries.Find(e => e.Item1.id == "max_progress").Item2
            };
            ModParser.Item prerqs = (ModParser.Item)i.entries.Find(e => e.Item1.id == "prerequisites").Item2;
            t.Prerequisites = new Dictionary<Technology, Tuple<double, double>>();
            if (prerqs != null)
            {
                prerqs.entries.ConvertAll(e => {
                    var b = e.Item2 as Tuple<string, object>;
                    return new Tuple<string, ModParser.Item>(b.Item1, (ModParser.Item)b.Item2);
                }).ForEach(p =>
                    t.Prerequisites.Add(new Technology() { Name = p.Item1 },
                                        new Tuple<double, double>((double)p.Item2.entries.Find(e => e.Item1.id == "min").Item2,
                                                                  (double)p.Item2.entries.Find(e => e.Item1.id == "max").Item2)
                                       )
                               );
            }
            t.Roots = new Dictionary<Technology, double>();
            if (i.entries.Find(e => e.Item1.id == "understanding").Item2 != null)
            {
                List<Tuple<string, double>> rts = (i.entries.Find(e => e.Item1.id == "understanding").Item2 as ModParser.Item).entries.
                    ConvertAll(e => e.Item2 as Tuple<string, object>).ConvertAll(e => new Tuple<string, double>(e.Item1, (double)e.Item2));
                rts.ForEach(r => t.Roots.Add(new Technology() { Name = r.Item1 }, r.Item2));
            }
            return t;
        }

        /// <summary>
        /// This function makes sure that the prerequisite technologies of each tech is correctly pointed to the actual tech
        /// instead of a dummy tech as given in the 'Interpret' function
        /// </summary>
        /// <param name="techTree"></param>
        internal static void PointPrerequisitesAndRoots(List<Technology> techTree)
        {
            foreach (Technology t in techTree)
            {
                var preq = t.Prerequisites.Keys.ToList();
                for (int i = 0; i < t.Prerequisites.Count; i++)
                {
                    Technology p = preq[i];
                    if (p.sector == null)    // This means that this technology is badly defined
                    {
                        p = techTree.Find(tech => tech.Name == p.Name);
                    }
                }
                var rts = t.Roots.Keys.ToList();
                for (int i = 0; i < t.Roots.Count; i++)
                {
                    Technology r = rts[i];
                    if (r.sector == null)    // This means that this technology is badly defined
                    {
                        r = techTree.Find(tech => tech.Name == r.Name);
                    }
                }
            }
        }

    }

    class TechnologyInstance
    {
        Technology parent;
        public string Name { get { return parent.Name; }  }
        public Technology.Sector? sector { get { return parent.sector; } }
        public Dictionary<Technology, Tuple<double, double>> Prerequisites { get { return parent.Prerequisites; } } // use the max of knowledge and understanding
        public double MaxKnowledge { get { return parent.MaxKnowledge; } }
        public Dictionary<Technology, double> Roots { get { return parent.Roots; } }            // These techs will increase in understanding by using this tech.

        public double Knowledge { get; private set; }     // theoretical
        public double Understanding { get; private set; } // practical

        public TechnologyInstance(string name)
        {
            if (Technology.TechTree.Any(t => t.Name == name) == false)
                throw new ArgumentException("The tech tree does not contain the technology: " + name);
            parent = Technology.TechTree.Find(t => t.Name == name);
            Knowledge = 0;
            Understanding = 0;
        }

        public TechnologyInstance(Technology tech)
        {
            parent = tech;
            Knowledge = 0;
            Understanding = 0;
        }

        internal double CurrentLevel()
        {
            return Math.Max(Knowledge, Understanding);
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

            if (obj == null)
            {
                return false;
            }
            if(obj.GetType() == typeof(Technology))
            {
                return parent.Equals(obj);
            }
            if(obj.GetType() == typeof(TechnologyInstance))
            {
                return parent.Equals(((TechnologyInstance)obj).parent);
            }
            return false;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return parent.GetHashCode();
        }
    }
}
