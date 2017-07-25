using System;
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
        public List<Leaders.Leader> leaders { get; private set; }
        public Production.TradeCenter tradeCenter { get; private set; }
        public Technology.Academy academy { get; private set; }
        List<Installations.Installation> freeInstallations;  // in contrast to assets bound to populations
        List<Mobiles.Mobile> mobiles;

        public Empire(string name, Bodies.Orbital capital)
        {
            this.name = name;
            populations = new List<Population>();
            leaders = new List<Leaders.Leader>();
            tradeCenter = new Production.TradeCenter();
            academy = new Technology.Academy();
            freeInstallations = new List<Installations.Installation>();
            mobiles = new List<Mobiles.Mobile>();

            populations.Add(Population.InitCapital(capital));

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

        public long population { get { return populations.Sum(p => p.count); } }
        public double wealth { get { return populations.Sum(p => p.wealth); } }

        public DateTime LastUpdate { get; private set; }

        public DateTime NextMandatoryUpdate { get; private set; }

        public bool NextUpdateHasPriority { get; private set; }

        const int DAYS_BETWEEN_UPDATES = 1;

        public override string ToString()
        {
            return "Empire: " + name;
        }

    }
}
