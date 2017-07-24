using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Bodies;

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
        Dictionary<Installations.Installation, int> installations;
        Production.Stockpile stockpile;
        
        public Population(Orbital location, long initPop)
        {
            this.location = location;
            count = initPop;
            location.addPopulation(this);
            wealth = count * 0.05;
            poverty = 0.1;
            inequality = 0.4;
            happiness = 0;
            installations = new Dictionary<Installations.Installation,int>();
            stockpile = new Production.Stockpile();
        }

        Population() { }

        internal static Population InitCapital(Orbital capital)
        {
            var p = new Population() {
                location = capital,
                count = (long)5e9,
                wealth = 5e9 * 0.05,
                poverty = 0.1,
                inequality = 0.4,
                happiness = 0,
                installations = new Dictionary<Installations.Installation, int>(),
                stockpile = new Production.Stockpile(1000)
            };
            capital.addPopulation(p);
            p.installations.Add(Installations.Installation.GetCopy("general_industry"), 10);
            p.installations.Add(Installations.Installation.GetCopy("construction_factory"), 10);
            p.installations.Add(Installations.Installation.GetCopy("mining_equipment"), 10);
            return p;
        }
    }
}
