using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Star : Orbital
    {

        public const double SolarMass = 1.98855e30;

        public Star(Orbital parent, double mass, OrbitalElements elements) : base(parent, mass, elements)
        {
        }

        public override void Generate(double mass, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
