using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Technology
    {
        public readonly string name;
        public readonly Academy.Sector sector;
        public readonly Dictionary<Technology, Tuple<double,double>> prerequisites; // use the max of knowledge and understanding
        public readonly double maxKnowledge;
        public readonly Dictionary<Technology, double> roots;             // These techs will increase in understanding by using this tech.
        
        public double knowledge { get; private set; }     // theoretical
        public double understanding { get; private set; } // practical

        public Technology(string name, Academy.Sector sector, Dictionary<Technology, Tuple<double, double>> prerequisites,
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

    }
}
