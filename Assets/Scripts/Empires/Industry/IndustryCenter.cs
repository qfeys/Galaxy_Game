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
        public readonly Stockpile stockpile;

        public Dictionary<Installation, int> installations { get; private set; }

        public List<Job> constructionQueue { get; private set; }
        public List<Job> productionQueue { get; private set; }

        public double maxComponentProduction { get; private set; }  // TODO: We'll have to find this from somwhere else
        public double activeCapacityComponentProduction { get; private set; }
        public double maxElectronicsProduction { get; private set; }  // TODO: We'll have to find this from somwhere else
        public double activeCapacityElectronicsProduction { get; private set; }

        public IndustryCenter()
        {
            stockpile = new Stockpile();
            constructionQueue = new List<Job>();
            productionQueue = new List<Job>();
        }

        public void BuildInstallation(Installation instl, int amount)
        {
            Job j = new Job(instl, amount);
        }

        public class Job
        {
            public readonly Installation instl;
            /// <summary>
            /// Keeps track of the amount of work that is still to be done
            /// </summary>
            public double work { get; private set; }
            /// <summary>
            /// Keeps track of the amount of resources that still need to be done
            /// </summary>
            public Stockpile.ResourceBill bill { get; private set; }
            /// <summary>
            /// The amount of objects that still have to build as part of this job
            /// </summary>
            public int amount { get; private set; }

            /// <summary>
            /// The part of total capacity that is used for this job
            /// </summary>
            public double capacity;

            public Job(Installation instl, int amount)
            {
                this.instl = instl;
                this.amount = amount;
                work = instl.costWork * amount;
                bill = instl.costResources * amount;
            }
        }
    }
}
