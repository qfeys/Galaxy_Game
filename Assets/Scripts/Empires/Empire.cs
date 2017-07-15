using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Empire
    {

        string name;
        List<Population> populations;
        public List<Leaders.Leader> leaders { get; private set; }
        public Production.TradeCenter tradeCenter { get; private set; }
        public Technology.Academy academy { get; private set; }
        List<Installations.Installation> freeInstallations;  // in contrast to assets bound to populations
        List<Mobiles.Mobile> mobiles;

        Simulation.Event nextUpdate;

        public Empire(string name, Bodies.Orbital capital)
        {
            this.name = name;
            populations = new List<Population>();
            leaders = new List<Leaders.Leader>();
            tradeCenter = new Production.TradeCenter();
            academy = new Technology.Academy();
            freeInstallations = new List<Installations.Installation>();
            mobiles = new List<Mobiles.Mobile>();

            populations.Add(new Population(capital, (long)5e9));

            nextUpdate = new Simulation.Event(Simulation.God.Time + TimeSpan.FromDays(1), Simulation.Event.Interrupt.soft, Update);
        }

        public void Update()
        {
            nextUpdate = new Simulation.Event(Simulation.God.Time + TimeSpan.FromDays(1), Simulation.Event.Interrupt.soft, Update);
        }

        public long population { get { return populations.Sum(p => p.count); } }
        public double wealth { get { return populations.Sum(p => p.wealth); } }

    }
}
