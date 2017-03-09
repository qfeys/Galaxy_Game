using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Academy
    {

        static List<Technology> techTree;
        internal enum Sector { fundPhysics, applPhysics, chemistry, biology, biomedics, engineering, sociology, psycology, linguistics}
        const double STANDARD_DEVELOPMENT_TIME = 5; // Years

        List<Technology> unlocks;
        Dictionary<Sector, double> funding;

        public Academy()
        {
            funding = Enum.GetValues(typeof(Sector)).Cast<Sector>().ToDictionary<Sector, Sector, double>(d => d, d => 100);
        }

        public static void Init()
        {
            techTree = ModParser.readTechnology();
        }

        public void CheckUnlocks()
        {
            foreach(Technology tech in techTree)
            {
                if (unlocks.Contains(tech))
                    continue;
                double chance = 1;
                foreach(var prereq in tech.prerequisites)
                {
                    if (unlocks.Contains(prereq.Key))
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
                TimeSpan mtth = TimeSpan.FromDays(-STANDARD_DEVELOPMENT_TIME * 356 * Math.Log(chance, 2));
                Simulation.Event.Try(mtth, TimeSpan.FromDays(1), Simulation.Event.Interrupt.soft, () => { unlocks.Add(tech); });
            }
        }
    }
}
