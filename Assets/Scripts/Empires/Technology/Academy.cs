using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Academy
    {

        static List<Technology> techTree;
        // internal enum Sector { fundPhysics, applPhysics, chemistry, biology, biomedics, engineering, sociology, psycology, linguistics }
        internal enum Sector { PHYSICS, DRIVE }
        const double STANDARD_DEVELOPMENT_TIME = 5; // Years

        public List<Technology> unlocks { get; private set; }
        public Dictionary<Sector, double> funding { get; private set; }

        public Academy()
        {
            funding = Enum.GetValues(typeof(Sector)).Cast<Sector>().ToDictionary<Sector, Sector, double>(d => d, d => 100);
            unlocks = new List<Technology>();
            CheckUnlocks();
        }

        public static void SetTechTree(List<Technology> tt)
        {
            if(techTree == null)
            {
                UnityEngine.Debug.Log("Techtree set with " + tt.Count + " technologies.");
            }else
            {
                UnityEngine.Debug.LogError("Techtree reset with " + tt.Count + " technologies.");
            }
            techTree = tt;
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
                if (chance > 0)
                {
                    TimeSpan mtth = TimeSpan.FromDays(-STANDARD_DEVELOPMENT_TIME * 365.25 * Math.Log(chance, 2));
                    Simulation.Event.Try(mtth, TimeSpan.FromDays(1), Simulation.Event.Interrupt.soft, () => { unlocks.Add(tech); });
                }
            }
        }
    }
}
