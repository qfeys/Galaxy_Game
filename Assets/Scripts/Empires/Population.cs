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
        /// The number of people this population has
        /// </summary>
        public long Count { get { return demographic.Count; } }
        /// <summary>
        /// The GDP of this population in GDP
        /// </summary>
        public double Wealth { get; private set; }
        Leaders.Leader governer;
        // List<policies>
        public Dictionary<Industry.Installation, int> installations { get; private set; }
        public Industry.Stockpile stockpile { get; private set; }
        public Demographics.Demographics demographic = new Demographics.Demographics();

        public Population(Planet location, long initPop, string name = null)
        {
            Name = name ?? location.ToString();
            Location = location;
            location.AddPopulation(this);
            Wealth = Count * 0.05;
            installations = new Dictionary<Industry.Installation,int>();
            stockpile = new Industry.Stockpile();
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
                //Count = (long)5e9,
                Wealth = 5e9 * 0.05,
                installations = new Dictionary<Industry.Installation, int>(),
                stockpile = new Industry.Stockpile(1000)
            };
            capital.AddPopulation(p);
            p.installations.Add(Industry.Installation.GetCopy("general_industry"), 10);
            p.installations.Add(Industry.Installation.GetCopy("construction_factory"), 10);
            p.installations.Add(Industry.Installation.GetCopy("mining_equipment"), 10);
            return p;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Use this only as a dummy.
        /// </summary>
        public static Population NullPop { get { return new Population() {stockpile = new Industry.Stockpile() }; } }
    }
}
