using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Technology
    {
        public string name { get; private set; }
        public Sector? sector { get; private set; }
        public Dictionary<Technology, Tuple<double,double>> prerequisites { get; private set; } // use the max of knowledge and understanding
        public double maxKnowledge { get; private set; }
        public Dictionary<Technology, double> roots { get; private set; }             // These techs will increase in understanding by using this tech.

        public double knowledge { get; private set; }     // theoretical
        public double understanding { get; private set; } // practical


        internal enum Sector { PHYSICS, DRIVE }

        private Technology()
        {

        }

        public Technology(string name, Sector sector, Dictionary<Technology, Tuple<double, double>> prerequisites,
            double maxKnowledge, Dictionary<Technology, double> roots)
        {
            this.name = name;this.sector = sector;this.prerequisites = prerequisites;this.maxKnowledge = maxKnowledge;this.roots = roots;
        }

        public Technology(Technology clone)
        {
            name = clone.name;sector = clone.sector;prerequisites = clone.prerequisites;maxKnowledge = clone.maxKnowledge;roots = clone.roots;
        }

        internal double CurrentLevel()
        {
            return Math.Max(knowledge, understanding);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Technology) && ((Technology)obj).name == name)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        internal static List<ModParser.Signature> Signature()
        {
            List<ModParser.Signature> ret = new List<ModParser.Signature>();
            ret.Add(new ModParser.Signature("starting_tech", ModParser.SignatueType.boolean));
            ret.Add(new ModParser.Signature("sector", ModParser.SignatueType.words, Enum.GetNames(typeof(Sector)).ToList()));
            ret.Add(new ModParser.Signature("max_progress", ModParser.SignatueType.integer));
            ret.Add(new ModParser.Signature("prerequisites", ModParser.SignatueType.list,
                new List<ModParser.Signature>() { new ModParser.Signature(null, ModParser.SignatueType.list,
                    new List<ModParser.Signature>() { new ModParser.Signature("min", ModParser.SignatueType.floating),
                                                    new ModParser.Signature("max", ModParser.SignatueType.floating)})
                }));
            ret.Add(new ModParser.Signature("understanding", ModParser.SignatueType.list,
                new List<ModParser.Signature>() { new ModParser.Signature(null, ModParser.SignatueType.floating)}
                ));

            return ret;
        }

        internal static Technology Interpret(ModParser.Item i)
        {
            Technology t = new Technology();
            t.name = i.name;
            t.sector = (Sector)Enum.Parse(typeof(Sector), i.entries.Find(e => e.Item1.id == "sector").Item2 as string);
            List<ModParser.Item> prerqs = i.entries.Find(e => e.Item1.id == "prerequisites").Item2 as List<ModParser.Item>;
            t.prerequisites = new Dictionary<Technology, Tuple<double, double>>();
            if (prerqs != null)
            {
                prerqs.ForEach(p =>
                    t.prerequisites.Add(new Technology() { name = p.name },
                                        new Tuple<double, double>((double)p.entries.Find(e => e.Item1.id == "min").Item2,
                                                                  (double)p.entries.Find(e => e.Item1.id == "max").Item2)
                                       )
                               );
            }
            t.roots = new Dictionary<Technology, double>();
            if (i.entries.Find(e => e.Item1.id == "understanding").Item2 != null)
            {
                List<Tuple<string, double>> rts = (i.entries.Find(e => e.Item1.id == "understanding").Item2 as ModParser.Item).entries.
                    ConvertAll(e => e.Item2 as Tuple<string, object>).ConvertAll(e => new Tuple<string, double>(e.Item1,(double)e.Item2));
                rts.ForEach(r => t.roots.Add(new Technology() { name = r.Item1 }, r.Item2));
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
            foreach(Technology t in techTree)
            {
                var preq = t.prerequisites.Keys.ToList();
                for (int i = 0; i < t.prerequisites.Count; i++)
                {
                    Technology p = preq[i];
                    if (p.sector == null)    // This means that this technology is badly defined
                    {
                        p = techTree.Find(tech => tech.name == p.name);
                    }
                }
                for(int i = 0; i< t.roots.Count; i++)
                {
                    Technology r = preq[i];
                    if (r.sector == null)    // This means that this technology is badly defined
                    {
                        r = techTree.Find(tech => tech.name == r.name);
                    }
                }
            }
        }

    }
}
