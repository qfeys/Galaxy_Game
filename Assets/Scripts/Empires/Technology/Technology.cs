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
        /// <summary>
        /// The amount of knowledge we can at most acuire on this subject.
        /// Also limits the understanding
        /// </summary>
        public double MaxKnowledge { get; private set; }
        public List<Prerequisite> Prerequisites { get; private set; }
        /// <summary>
        /// These are technologies that will increase in  understanding by using this tech
        /// </summary>
        public List<RootTech> Roots { get; private set; }
        bool isProject;

        static List<Technology> techTree;
        internal static List<Technology> TechTree { get { return techTree; } }

        internal enum Sector { PHYSICS, DRIVE }

        private Technology() { }

        public Technology(string name, Sector sector, List<Prerequisite> prerequisites,
            double maxKnowledge, List<RootTech> roots)
        {
            Name = name;this.sector = sector; Prerequisites = prerequisites; MaxKnowledge = maxKnowledge; Roots = roots;
        }

        public Technology(Technology clone)
        {
            Name = clone.Name; sector = clone.sector; Prerequisites = clone.Prerequisites; MaxKnowledge = clone.MaxKnowledge; Roots = clone.Roots;
        }

        internal static void LoadTechTree()
        {
            ModParser.Item modItem = ModParser.RetriveMasterItem("technology");
            techTree = modItem.GetChilderen().ConvertAll(i => Interpret(i));
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

        public override string ToString()
        {
            return "t_" + Name;
        }

        internal static Technology FindTech(string name)
        {
            return techTree.Find(t => t.Name == name);
        }

        internal static Technology Interpret(ModParser.Item i)
        {
            Technology t = new Technology() {
                Name = i.name,
                sector = i.GetEnum<Sector>("sector"),
                MaxKnowledge = i.GetNumber("max_progress"),
                Prerequisites = new List<Prerequisite>(),
                Roots = new List<RootTech>(),
                isProject = i.GetBool("is_project")
            };
            ModParser.Item prerqs = i.GetItem("prerequisites");
            if (prerqs != null)
            {
                prerqs.GetChilderen().ForEach(p =>
                    t.Prerequisites.Add(new Prerequisite(p.name, p.GetNumber("min"), p.GetNumber("max")))
                    );
            }
            ModParser.Item underst = i.GetItem("understanding");
            if (underst != null)
            {
                underst.GetChilderen().ForEach(r =>
                    t.Roots.Add(new RootTech(r.name, r.GetNumber()))
                );
            }
            return t;
        }

        /// <summary>
        /// Container class for a prerequisite technology.
        /// </summary>
        public class Prerequisite
        {
            /// <summary>
            /// The name of the prerequisite technology
            /// </summary>
            public readonly string name;
            /// <summary>
            /// The minimum value required to aquire the new technology
            /// </summary>
            public readonly double min;
            /// <summary>
            /// The maximum value where the development of the new tech will be hindered
            /// </summary>
            public readonly double max;

            public Technology Tech { get { return techTree.Find(t => t.Name == name); } }

            public Prerequisite(string name, double min, double max)
            {
                this.name = name; this.min = min; this.max = max;
            }
        }

        /// <summary>
        /// Container class for a root technology.
        /// </summary>
        public class RootTech
        {
            /// <summary>
            /// The name of the root technology
            /// </summary>
            public readonly string name;
            /// <summary>
            /// The amount of understanding the root recieves for every point of understanding
            /// this tech gains. Recommended to be below 1.
            /// </summary>
            public readonly double affinity;

            public Technology Tech { get { return techTree.Find(t => t.Name == name); } }

            public RootTech(string name, double affinity)
            {
                this.name = name; this.affinity = affinity;
            }
        }
    }

    class TechnologyInstance
    {
        Technology parent;
        public string Name { get { return parent.Name; }  }
        public Technology.Sector? sector { get { return parent.sector; } }
        public List<Technology.Prerequisite> Prerequisites { get { return parent.Prerequisites; } }
        /// <summary>
        /// The amount of knowledge we can at most acuire on this subject.
        /// Also limits the understanding
        /// </summary>
        public double MaxKnowledge { get { return parent.MaxKnowledge; } }
        /// <summary>
        /// These are technologies that will increase in  understanding by using this tech
        /// </summary>
        public List<Technology.RootTech> Roots { get { return parent.Roots; } }

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
