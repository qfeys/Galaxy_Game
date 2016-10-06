using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class StarSystem : Orbital
    {
        public StarSystem(Orbital parent, OrbitalElements elements) : base(parent, 0, elements)
        {
        }

        public override void Generate(double mass, Random rand)
        {
            Star s = new Star(this, mass * 0.995, new OrbitalElements());
            s.Generate(mass*0.995,rand);
            mass *= .005;
            while (mass > 1e25)
            {
                double gMass = rand.NextDouble() * 0.35 + 0.6;
                OrbitalElements gElem = new OrbitalElements(
                    rand.NextDouble()*2*Math.PI,
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 2 * Math.PI,
                    chooseNewSMA(rand.NextDouble(),gMass),
                    rand.NextDouble() * 0.01,
                    s);

                Giant g = new Giant(this, gMass, gElem);
            }
        }

        /// <summary>
        /// Using Newtons methode to find the SMA from a probability function
        /// </summary>
        /// <returns></returns>
        ulong chooseNewSMA(double P, double gMass)
        {
            //(4*R*R*e^(-(2*R)/m))/m^3                      Probability function
            // P = 1- exp(-2*R/m)*(m^2+2*m*R+2*R^2)/m^2     cumulative probability function, maps to [0,1]
            ulong m = (ulong)(gMass / Giant.JupiterMass * Giant.JupiterSMA);    // mean, the most likely radius
            ulong R = m;                                                        // Radius, an estimate of the sma
            double diff = P - 1 - Math.Exp(-2.0 * R / m) * (m * m + 2 * m * R + 2 * R * R) / (m * m);
            while (diff > 1e-10)
            {
                double slope = (4 * R * R * Math.Exp(-(2.0 * R) / m)) / (m * m * m);
                R -= (ulong)(diff / slope);

                diff = P - 1 - Math.Exp(-2.0 * R / m) * (m * m + 2 * m * R + 2 * R * R) / (m * m);
            }
            return R;
        }

        public const ulong AU = 149597870700;
        public const ulong SolSize = 40 * AU;
    }
}
