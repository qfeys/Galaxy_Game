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
        /// <summary>
        /// The planet where this population is
        /// </summary>
        public Planet Location { get; private set; }
        /// <summary>
        /// The number of people this population has (TODO: full demographics)
        /// </summary>
        public long Count { get; private set; }
        /// <summary>
        /// The GDP of this population in GDP
        /// </summary>
        public double Wealth { get; private set; }
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

        /// <summary>
        /// Creates a new population to function as a capital on the planet given.
        /// This does not check the validity of this planet.
        /// </summary>
        /// <param name="capital"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Use this only as a dummy.
        /// </summary>
        public static Population NullPop { get { return new Population(); } }
    }
}
