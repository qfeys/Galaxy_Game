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
        public readonly Planet Location;
        /// <summary>
        /// The empire this population belongs to
        /// </summary>
        public readonly Empire Empire;
        /// <summary>
        /// The number of people this population has
        /// </summary>
        public double Count { get { return demographic.Count.Value(Simulation.God.Time); } }
        /// <summary>
        /// The GDP of this population in GDP
        /// </summary>
        public double Wealth { get; private set; }
        Leaders.Leader governer;
        // List<policies>
        public readonly Industry.IndustryCenter industryCenter;
        public readonly Demographics.Demographics demographic = new Demographics.Demographics();

        public Population(Planet location, long initPop, string name = null)
        {
            Name = name ?? location.ToString();
            Location = location;
            location.AddPopulation(this);
            Wealth = Count * 0.05;
            location.AddPopulation(this);
            industryCenter = new Industry.IndustryCenter(this);
        }

        Population() { industryCenter = new Industry.IndustryCenter(this); }

        /// <summary>
        /// Creates a new population to function as a capital on the planet given.
        /// This does not check the validity of this planet.
        /// </summary>
        /// <param name="capital"></param>
        /// <returns></returns>
        internal static Population InitCapital(Planet capital)
        {
            var p = new Population(capital,(long)5e9, "Capital");
            return p;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Use this only as a dummy.
        /// </summary>
        public static Population NullPop { get { return new Population() { }; } }
    }
}
