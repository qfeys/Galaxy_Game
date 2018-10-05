using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Empires.Industry
{
    /// <summary>
    /// This class organises all the industry of a population
    /// </summary>
    class IndustryCenter
    {
        public readonly Population parent;

        public readonly Stockpile stockpile;

        /// <summary>
        /// All the constructed installations in this population
        /// </summary>
        public Dictionary<Installation, int> installations { get; private set; }

        public List<Job> constructionQueue { get; private set; }
        public List<Job> productionQueue { get; private set; }

        public Changeling constructionCapacity;
        public Changeling componentProduction;
        public Changeling electronicsProduction;
        public double activeComponentProduction { get; private set; }
        public double activeElectronicsProduction { get; private set; }

        public IndustryCenter(Population parent)
        {
            this.parent = parent;
            stockpile = new Stockpile();
            constructionQueue = new List<Job>();
            productionQueue = new List<Job>();
        }

        public void RecalculateConstructionCapacity()
        {
            double fromBuildings = 0;
            installations.Keys.ToList().ForEach(installation =>
            {
                if (installation.Effect.Any(m => m.name == Installation.InstallationEffect.Name.construction))
                    fromBuildings += installation.Effect.Find(m => m.name == Installation.InstallationEffect.Name.construction).value * installations[installation];
            });
            Changeling fromFreePops = parent.demographic.FreeIndustrialPopulation * .01;
            // add pop modefiers
            // add sector modefiers
            // add empire modefiers
            constructionCapacity.Modify(fromFreePops + fromBuildings);
        }

        /// <summary>
        /// Start a new construction job
        /// </summary>
        /// <param name="instl"></param>
        /// <param name="amount"></param>
        /// <param name="capacity">The fraction of the entire construction cap that should work on this.</param>
        public void BuildInstallation(Installation instl, int amount, double capacity = 1)
        {
            Job j = new Job(instl, amount, constructionCapacity * capacity);    // TODO: This is fishy
            constructionQueue.Add(j);
        }

        public class Job
        {
            public readonly Installation instl;
            /// <summary>
            /// Keeps track of the amount of work that is still to be done
            /// </summary>
            public Changeling WorkLeft { get; private set; }
            /// <summary>
            /// Keeps track of the amount of resources that still need to be consumed
            /// </summary>
            public Stockpile.ResourceBill Bill { get; private set; }
            /// <summary>
            /// The amount of objects that still have to build as part of this job
            /// </summary>
            public int Amount { get; private set; }
            /// <summary>
            /// The capacity that is used for this job
            /// </summary>
            public Changeling capacity;

            public Job(Installation instl, int amount, Changeling capacity)
            {
                this.instl = instl;
                this.Amount = amount;
                WorkLeft = Changeling.Create(instl.costWork * amount, capacity, Simulation.God.Time);
                Bill = instl.costResources * amount;
                this.capacity = capacity;

            }
        }
    }
}
