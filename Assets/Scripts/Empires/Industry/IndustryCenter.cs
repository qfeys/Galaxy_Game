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

        public double maxComponentProduction { get; private set; }  // TODO: We'll have to find this from somwhere else
        public double activeCapacityComponentProduction { get; private set; }
        public double maxElectronicsProduction { get; private set; }  // TODO: We'll have to find this from somwhere else
        public double activeCapacityElectronicsProduction { get; private set; }

        public IndustryCenter(Population parent)
        {
            this.parent = parent;
            stockpile = new Stockpile();
            constructionQueue = new List<Job>();
            productionQueue = new List<Job>();
        }

        public Changeling GetConstructionCapacity()
        {
            Changeling cap = Changeling.Create(0);
            installations.Keys.ToList().ForEach(k =>
            {
                if (k.Modefiers.Any(m => m.name == Modifier.Name.add_construction_capacity))
                    cap += k.Modefiers.Find(m => m.name == Modifier.Name.add_construction_capacity).value * installations[k];
            });
            cap += parent.demographic.FreeIndustrialPopulation * .01;
            // add pop modefiers
            // add sector modefiers
            // add empire modefiers
            return cap;
        }

        public void BuildInstallation(Installation instl, int amount, double capacity = 1)
        {
            Job j = new Job(instl, amount, Changeling.Create(capacity));    // TODO: This is fishy
            constructionQueue.Add(j);
        }

        public class Job
        {
            public readonly Installation instl;
            /// <summary>
            /// Keeps track of the amount of work that is still to be done
            /// </summary>
            public Changeling work { get; private set; }
            /// <summary>
            /// Keeps track of the amount of resources that still need to be done
            /// </summary>
            public Stockpile.ResourceBill bill { get; private set; }
            /// <summary>
            /// The amount of objects that still have to build as part of this job
            /// </summary>
            public int amount { get; private set; }

            public Changeling capacity;

            public Simulation.Event.Conditional nextItemDone;

            /// <summary>
            /// The part of total capacity that is used for this job
            /// </summary>
            public double capacityFraction;

            public Job(Installation instl, int amount, Changeling capacity, double capacityFraction = 1)
            {
                this.instl = instl;
                this.amount = amount;
                work = Changeling.Create(instl.costWork * amount, capacity, Simulation.God.Time);
                bill = instl.costResources * amount;
                this.capacity = capacity;
                this.capacityFraction = capacityFraction;
                //nextItemDone = new Simulation.Event.Conditional();// TODO CONTINUE HERE !!!!!!!!!!!!!!
            }
        }
    }
}
