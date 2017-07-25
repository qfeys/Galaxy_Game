using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    abstract class Orbital
    {
        Orbital parent;
        public List<Orbital> Childeren { get; private set; }
        public double Mass { get; private set; }
        public OrbitalElements Elements { get; private set; }
        ulong id;
        static ulong idCounter = 0;
        List<Empires.Population> populations;

        public Orbital(Orbital parent, double mass, OrbitalElements elements)
        {
            this.parent = parent; Mass = mass; Elements = elements;
            Childeren = new List<Orbital>();
            id = idCounter;
            idCounter++;
            if(parent != null)
                this.parent.Childeren.Add(this);
            populations = new List<Empires.Population>();
        }

        public abstract void Generate(double mass, Random rand);

        public override string ToString()
        {
            return this.GetType().Name + id;
        }

        public string Information()
        {
            string info = string.Join(";", new[] { "mass", Mass.ToString("e3"), "SMA", Elements.SMA.ToString("e3")});
            if(populations != null)
            {
                info = string.Join(";", new[] { info, "Population", TotalPopulation.ToString() });
            }
            return info;
        }

        public void AddPopulation(Empires.Population p)
        {
            if (populations.Contains(p))
                throw new ArgumentException("" + this.ToString() + " already has population " + p.ToString());
            populations.Add(p);
        }

        public float TotalPopulation { get { return populations.Sum(p => (float)p.Count); } }
    }

}
