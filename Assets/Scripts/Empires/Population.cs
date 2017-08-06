using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Bodies;

namespace Assets.Scripts.Empires
{
    class Population
    {

        public string Name { get; private set; }
        public Planet Location { get; private set; }
        public long Count { get; private set; }
        public double Wealth { get; private set; } // GDP in Milion USD
        double poverty; // part of people living in poverty
        double inequality; // gini-index - lower is better
        double happiness; // negative is bad
        Leaders.Leader governer;
        // species
        // culture
        // List<policies>
        Dictionary<Installations.Installation, int> installations;
        Production.Stockpile stockpile;
        
        public Population(Planet location, long initPop, string name = null)
        {
            Name = name ?? location.ToString();
            Location = location;
            Count = initPop;
            location.AddPopulation(this);
            Wealth = Count * 0.05;
            poverty = 0.1;
            inequality = 0.4;
            happiness = 0;
            installations = new Dictionary<Installations.Installation,int>();
            stockpile = new Production.Stockpile();
        }

        Population() { }

        internal static Population InitCapital(Planet capital)
        {
            var p = new Population() {
                Name = "Capital",
                Location = capital,
                Count = (long)5e9,
                Wealth = 5e9 * 0.05,
                poverty = 0.1,
                inequality = 0.4,
                happiness = 0,
                installations = new Dictionary<Installations.Installation, int>(),
                stockpile = new Production.Stockpile(1000)
            };
            capital.AddPopulation(p);
            p.installations.Add(Installations.Installation.GetCopy("general_industry"), 10);
            p.installations.Add(Installations.Installation.GetCopy("construction_factory"), 10);
            p.installations.Add(Installations.Installation.GetCopy("mining_equipment"), 10);
            return p;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
