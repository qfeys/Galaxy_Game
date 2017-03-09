using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Technology
    {
        public readonly string name;
        Academy.Sector sector;
        Dictionary<Technology, Tuple<double,double>> prerequisites; // use the max of knowledge and understanding
        double maxKnowledge;
        Dictionary<Technology, double> roots;             // These techs will increase in understanding by using this tech.

        bool unlocked = false;
        double knowledge;   // theoretical
        double understanding; // practical

        public Technology(string name, Academy.Sector sector, Dictionary<Technology, Tuple<double, double>> prerequisites,
            double maxKnowledge, Dictionary<Technology, double> roots)
        {
            this.name = name;this.sector = sector;this.prerequisites = prerequisites;this.maxKnowledge = maxKnowledge;this.roots = roots;
        }

        public Technology(Technology clone)
        {
            name = clone.name;sector = clone.sector;prerequisites = clone.prerequisites;maxKnowledge = clone.maxKnowledge;roots = clone.roots;
        }
    }
}
