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
        public double wealth { get; private set; } // GDP in Milion USD
        double poverty; // part of people living in poverty
        double inequality; // gini-index - lower is better
        double happiness; // negative is bad
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
            wealth = count * 0.05;
            poverty = 0.1;
            inequality = 0.4;
            happiness = 0;
            assets = new List<Assets.Asset>();
            stockpile = new Production.Stockpile();
        }
    }
}
