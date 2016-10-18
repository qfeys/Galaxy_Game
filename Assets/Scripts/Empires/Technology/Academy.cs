using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Academy
    {

        static List<Technology> techTree;
        internal enum Department { fundPhysics, applPhysics, chemistry, biology, biomedics, engineering, sociology, psycology, linguistics}
        Dictionary<Department, double> funding;

        public Academy()
        {
            funding = Enum.GetValues(typeof(Department)).Cast<Department>().ToDictionary<Department, Department, double>(d => d, d => 100);
        }
    }
}
