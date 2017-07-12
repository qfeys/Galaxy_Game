﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Technology
{
    class Academy
    {

        static List<Technology> techTree;
        // internal enum Sector { fundPhysics, applPhysics, chemistry, biology, biomedics, engineering, sociology, psycology, linguistics }
        const double STANDARD_DEVELOPMENT_TIME = 5; // Years

        public List<Technology> unlocks { get; private set; }
        public Dictionary<Technology.Sector, double> funding { get; private set; }

        public Academy()
        {
            funding = Enum.GetValues(typeof(Technology.Sector)).Cast<Technology.Sector>().ToDictionary<Technology.Sector, Technology.Sector, double>(d => d, d => 100);
            unlocks = new List<Technology>();
            CheckUnlocks();
        }

        internal static void SetTechTree(List<ModParser.Item> itemList)
        {
            if (techTree == null)
            {
                UnityEngine.Debug.Log("Techtree set with " + itemList.Count + " technologies.");
            }
            else
            {
                UnityEngine.Debug.LogError("Techtree reset with " + itemList.Count + " technologies.");
            }
            List<Technology> tt = itemList.ConvertAll(i => Technology.Interpret(i));
            Technology.PointPrerequisitesAndRoots(tt);
            techTree = new List<Technology>();
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
