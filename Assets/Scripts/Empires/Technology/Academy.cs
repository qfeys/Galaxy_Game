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

        public List<TechnologyInstance> unlocks { get; private set; }
        public Dictionary<Technology.Sector, double> funding { get; private set; }

        public DateTime LastUpdate { get; private set; }

        public DateTime NextMandatoryUpdate { get { return _nextMandatoryUpdate; } private set { _nextMandatoryUpdate = value;
                Simulation.God.ExcicuteOnUnityThread(() => UnityEngine.Debug.Log("NMU changed to: " + _nextMandatoryUpdate));
            } }
        DateTime _nextMandatoryUpdate;

        public bool NextUpdateHasPriority { get; private set; }

        public Academy()
        {
            funding = Enum.GetValues(typeof(Technology.Sector)).Cast<Technology.Sector>().ToDictionary<Technology.Sector, Technology.Sector, double>(d => d, d => 100);
            unlocks = new List<TechnologyInstance>();
            CheckUnlocks(Simulation.God.Time);
            NextMandatoryUpdate = Simulation.God.Time;
            Simulation.EventSchedule.Add(this);
        }

        public void CheckUnlocks(DateTime date)
        {
            foreach(Technology tech in Technology.TechTree)
            {
                if (unlocks.Any(ti => ti.name == tech.name))
                    continue;
                double chance = 1;
                foreach(var prereq in tech.prerequisites)
                {
                    if (unlocks.Any(ti => ti.name == prereq.Key.name))
                    {
                        double level = unlocks.Find(t => t.Equals(prereq.Key)).CurrentLevel();
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
                        unlocks.Add(new TechnologyInstance(tech));
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
    }
}
