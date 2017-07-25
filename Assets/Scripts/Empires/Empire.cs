﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Simulation;

namespace Assets.Scripts.Empires
{
    class Empire : IUpdatable
    {

        string name;
        List<Population> populations;
        public List<Leaders.Leader> Leaders { get; private set; }
        public Production.TradeCenter TradeCenter { get; private set; }
        public Technology.Academy Academy { get; private set; }
        List<Installations.Installation> freeInstallations;  // in contrast to assets bound to populations
        List<Mobiles.Mobile> mobiles;

        public long Population { get { return populations.Sum(p => p.Count); } }
        public double Wealth { get { return populations.Sum(p => p.Wealth); } }

        public DateTime LastUpdate { get; private set; }

        public DateTime NextMandatoryUpdate { get; private set; }

        public bool NextUpdateHasPriority { get; private set; }

        public Empire(string name, Bodies.Orbital capital)
        {
            this.name = name;
            populations = new List<Population>();
            Leaders = new List<Leaders.Leader>();
            TradeCenter = new Production.TradeCenter();
            Academy = new Technology.Academy();
            freeInstallations = new List<Installations.Installation>();
            mobiles = new List<Mobiles.Mobile>();

            populations.Add(Empires.Population.InitCapital(capital));

            NextMandatoryUpdate = God.Time.AddDays(DAYS_BETWEEN_UPDATES);
            NextUpdateHasPriority = false;
            EventSchedule.Add(this);
        }

        public void Update(DateTime date)
        {
            LastUpdate = date;
            NextMandatoryUpdate = date.AddDays(DAYS_BETWEEN_UPDATES);
            God.ExcicuteOnUnityThread(() => UnityEngine.Debug.Log("Empire update. Next update: "+NextMandatoryUpdate));
        }

        public override string ToString()
        {
            return "Empire: " + name;
        }

        const int DAYS_BETWEEN_UPDATES = 1;

    }
}
