using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Academy : Simulation.IUpdatable
    {

        // internal enum Sector { fundPhysics, applPhysics, chemistry, biology, biomedics, engineering, sociology, psycology, linguistics }
        const double STANDARD_DEVELOPMENT_TIME = 5; // Years

        public List<TechnologyInstance> Unlocks { get; private set; }
        List<Laboratory> labs;

        public DateTime LastUpdate { get; private set; }

        public DateTime NextMandatoryUpdate { get { return _nextMandatoryUpdate; } private set { _nextMandatoryUpdate = value; } }
        DateTime _nextMandatoryUpdate;

        public bool NextUpdateHasPriority { get; private set; }

        public Academy()
        {
            Unlocks = new List<TechnologyInstance>();
            labs = new List<Laboratory>();
            CheckUnlocks(Simulation.God.Time);
            NextMandatoryUpdate = Simulation.God.Time;
            Simulation.EventSchedule.Add(this);
        }

        public void CheckUnlocks(DateTime date)
        {
            foreach(Technology tech in Technology.TechTree)
            {
                if (Unlocks.Any(ti => ti.Name == tech.Name))
                    continue;
                double chance = 1;
                foreach(var prereq in tech.Prerequisites)
                {
                    if (Unlocks.Any(ti => ti.Name == prereq.Key.Name))
                    {
                        double level = Unlocks.Find(t => t.Equals(prereq.Key)).CurrentLevel();
                        if (level > prereq.Value.Item2)
                            continue;   // Check approved
                        else if (level > prereq.Value.Item1)
                        {
                            double progress = (level - prereq.Value.Item1) / (prereq.Value.Item2 - prereq.Value.Item1);
                            chance *= progress;
                        }
                        else
                        {
                            chance = 0;
                            break;
                        }
                    }
                    else
                    {
                        chance = 0;
                        break;
                    }
                }
                if (chance > 0)
                {
                    TimeSpan mtth = TimeSpan.FromDays(-STANDARD_DEVELOPMENT_TIME * 365.25 * Math.Log(chance, 2));
                    TimeSpan timeLeft =  RNG.NextOccurence(mtth);
                    if (date - LastUpdate <= timeLeft)
                    {
                        Unlocks.Add(new TechnologyInstance(tech));
                    }
                    else if(date+timeLeft < NextMandatoryUpdate)
                    {
                        NextMandatoryUpdate = date + timeLeft;      // if it's about time to unlock, update more frequently.
                    }
                }
            }
        }

        public void Update(DateTime date)
        {
            NextMandatoryUpdate = date.AddDays(5);
            CheckUnlocks(date);
            LastUpdate = date;
        }

        public List<Laboratory> GetLabsAt(Population pop)
        {
            var ret = new List<Laboratory>();
            foreach (Laboratory  lab in labs)
            {
                if (lab.pop == pop) ret.Add(lab);
            }
            return ret;
        }

        public class Laboratory
        {
            public readonly Population pop;
            public readonly int id;

            public Leaders.Leader leader { get; private set; }

            bool sector;
            Technology.Sector sectorResearch;
            Technology.Project projectResearch;
            public string currentProject { get { return sector ? sectorResearch.ToString() : projectResearch.ToString(); } }

            /// <summary>
            /// TODO: make these integratable values
            /// </summary>
            public float leaderEfficiency = 0;
            public float projectEfficiency = 0;

            public Laboratory(Population pop, int id)
            {
                this.pop = pop;
                this.id = id;
            }
        }
    }
}
