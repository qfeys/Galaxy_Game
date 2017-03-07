using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Academy
    {

        static List<Technology> techTree;
        internal enum Sector { fundPhysics, applPhysics, chemistry, biology, biomedics, engineering, sociology, psycology, linguistics}
        Dictionary<Sector, double> funding;

        public Academy()
        {
            funding = Enum.GetValues(typeof(Sector)).Cast<Sector>().ToDictionary<Sector, Sector, double>(d => d, d => 100);
        }

        public static void Init()
        {
            techTree = ModParser.readTechnology();
        }
    }
}
