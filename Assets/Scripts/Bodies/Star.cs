using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Star : Orbital
    {

        public const double SOLAR_MASS = 1.98855e30;

        public enum Classification { O, B, A, F, G, K ,M };

        Classification classification;

        public Star(Orbital parent, double mass, OrbitalElements elements) : base(parent, mass, elements)
        {
            if (Mass > 16 * SOLAR_MASS) classification = Classification.O;
            else if (Mass > 2.1 * SOLAR_MASS) classification = Classification.B;
            else if (Mass > 1.4 * SOLAR_MASS) classification = Classification.A;
            else if (Mass > 1.04 * SOLAR_MASS) classification = Classification.F;
            else if (Mass > 0.8 * SOLAR_MASS) classification = Classification.G;
            else if (Mass > 0.45 * SOLAR_MASS) classification = Classification.K;
            else if (Mass > 0.08 * SOLAR_MASS) classification = Classification.M;
            else throw new Exception("Star not valid, to small");
        }

        public override void Generate(double mass, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
