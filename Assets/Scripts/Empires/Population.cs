using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Population
    {

        Bodies.Orbital location;
        public long count { get; private set; }
        public double wealth { get; private set; } // GDP in USD
        long poverty; // amount of people livning in poverty
        double inequality; // gini-index
        double contentment; // negative is bad
        Leaders.Leader governer;
        // species
        // culture
        // List<policies>
        List<Assets.Asset> assets;
        Production.Stockpile stockpile;
        
        public Population(Bodies.Orbital location, long initPop)
        {
            this.location = location;
            count = initPop;
            location.addPopulation(this);
        }
    }
}
