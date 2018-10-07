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
        readonly Changeling workDoneOnCurrentConstruction;
        Changeling.Subscription nextConstructionEvent;
        public List<Job> productionQueue { get; private set; }
        readonly Changeling workDoneOnCurrentProduction;
        Changeling.Subscription nextProductionEvent;

        readonly public Changeling constructionCapacity;
        readonly public Changeling componentProduction;
        readonly public Changeling electronicsProduction;
        public double activeComponentProduction { get; private set; }   // Between 0 and 1
        public double activeElectronicsProduction { get; private set; } // Between 0 and 1

        public IndustryCenter(Population parent)
        {
            this.parent = parent;
            stockpile = new Stockpile();
            constructionQueue = new List<Job>();
            productionQueue = new List<Job>();
            workDoneOnCurrentConstruction = Changeling.ReserveComplex();
            nextConstructionEvent = workDoneOnCurrentConstruction.Subscribe(double.MinValue, ConstructionCompleted);
            workDoneOnCurrentProduction = Changeling.ReserveComplex();
            nextProductionEvent = workDoneOnCurrentProduction.Subscribe(double.MinValue, ProductionCompleted);
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
            Job j = new Job(instl, amount, capacity);
            constructionQueue.Add(j);
            RecalculateNextConstructionEvent();
        }

        private void ConstructionCompleted()
        {
            throw new NotImplementedException();
        }

        private void ProductionCompleted()
        {
            throw new NotImplementedException();
        }

        void RecalculateNextConstructionEvent()
        {
            if(constructionQueue.Count == 0)
            {
                nextConstructionEvent.ChangeTrigger(double.MinValue);
                return;
            }
            double workTillNext = constructionQueue[0].instl.costWork;
            nextConstructionEvent.ChangeTrigger(workTillNext);
        }

        public class Job
        {
            public readonly Installation instl;
            /// <summary>
            /// Keeps track of the amount of resources that still need to be consumed at Tick
            /// </summary>
            public Stockpile.ResourceBill BillAtTick { get; private set; }
            /// <summary>
            /// The amount of objects that still have to build as part of this job
            /// </summary>
            public int Amount { get; private set; }
            /// <summary>
            /// The relative capacity that is used for this job
            /// </summary>
            public double capacity;

            public double workLeftAtTick;
            public DateTime tick;

            public Job(Installation instl, int amount, double capacity)
            {
                this.instl = instl;
                this.Amount = amount;
                BillAtTick = instl.costResources * amount;
                this.capacity = capacity;
                workLeftAtTick = instl.costWork * amount;
                tick = Simulation.God.Time;
            }
        }
    }
}
