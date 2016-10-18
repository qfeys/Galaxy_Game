using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Technology
    {
        string name;
        double knowledge;   // theoretical
        double understanding; // practical
        Academy.Department field;
        Dictionary<Technology, double> prerequisites; // use the max of knowledge and understanding


    }
}
