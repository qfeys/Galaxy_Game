using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Technology
    {
        string name;
        Academy.Sector sector;
        Dictionary<Technology, Tuple<double,double>> prerequisites; // use the max of knowledge and understanding
        double maxKnowledge;
        List<Technology> Roots;             // These techs will increase in understanding by using this tech.

        bool unlocked = false;
        double knowledge;   // theoretical
        double understanding; // practical


    }
}
