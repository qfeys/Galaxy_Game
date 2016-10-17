using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    abstract class Orbital
    {
        Orbital Parent;
        public List<Orbital> Childeren { get; private set; }
        public double Mass { get; private set; }
        public OrbitalElements Elements { get; private set; }
        ulong id;
        static ulong idCounter = 0;

        public Orbital(Orbital parent, double mass, OrbitalElements elements)
        {
            Parent = parent; Mass = mass; Elements = elements;
            Childeren = new List<Orbital>();
            id = idCounter;
            idCounter++;
            if(parent != null)
                Parent.Childeren.Add(this);
        }

        public abstract void Generate(double mass, Random rand);

        public override string ToString()
        {
            return this.GetType().ToString();
        }
    }

}
