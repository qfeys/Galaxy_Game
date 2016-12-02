using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Population
    {

        Bodies.Orbital planet;
        ulong count;
        double wealth; // GDP in USD
        ulong poverty; // amount of people livning in poverty
        double inequality; // gini-index
        double contentment; // negative is bad
        Leaders.Leader governer;
        // species
        // culture
        // List<policies>
        List<Assets.Asset> assets;
        Production.Stockpile stockpile;

        public ulong population { get { return count; } }
    }
}
