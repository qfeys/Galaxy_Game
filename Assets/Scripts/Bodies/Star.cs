using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Star : Orbital
    {

        public const double SolarMass = 1.98855e30;

        public enum Classification { O, B, A, F, G, K ,M };

        Classification classification;

        public Star(Orbital parent, double mass, OrbitalElements elements) : base(parent, mass, elements)
        {
            if (Mass > 16 * SolarMass) classification = Classification.O;
            else if (Mass > 2.1 * SolarMass) classification = Classification.B;
            else if (Mass > 1.4 * SolarMass) classification = Classification.A;
            else if (Mass > 1.04 * SolarMass) classification = Classification.F;
            else if (Mass > 0.8 * SolarMass) classification = Classification.G;
            else if (Mass > 0.45 * SolarMass) classification = Classification.K;
            else if (Mass > 0.08 * SolarMass) classification = Classification.M;
            else throw new Exception("Star not valid, to small");
        }

        public override void Generate(double mass, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
