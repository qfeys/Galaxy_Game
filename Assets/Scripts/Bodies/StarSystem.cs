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
            Rendering.God.log.Add("Star with mass: " + mass * 0.995);
            //s.Generate(mass*0.995,rand);
            mass *= .005;
            int i = 0;
            while (mass > 1e25 || i>10)
            {
                double gMass = (rand.NextDouble() * 0.35 + 0.6) * mass;  // between 60% and 95% of leftover mass
                ulong sma = chooseNewSMA(rand.NextDouble(), gMass);
                OrbitalElements gElem = new OrbitalElements(
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 0.09 - 0.045,           // between -0.045 and 0.045 rad
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 2 * Math.PI,
                    sma,
                    rand.NextDouble() * 0.01,
                    s);

                Rendering.God.log.Add("Giant with mass: " + gMass +" and SMA: " + gElem.SMA);
                Giant g = new Giant(this, gMass, gElem);

                mass -= gMass;
                i++;
            }
        }

        /// <summary>
        /// Using Newtons methode to find the SMA from a probability function
        /// </summary>
        /// <returns></returns>
        ulong chooseNewSMA(double rand, double gMass)
        {
            //(4*R*R*e^(-(2*R)/m))/m^3                      Probability function
            // P = 1- exp(-2*R/m)*(m^2+2*m*R+2*R^2)/m^2     cumulative probability function, maps to [0,1]
            // ((log(10,x*10+1)*log(11,10)+1)/2             formulla most likly radius based on mass
            double massRatio = gMass / Giant.JupiterMass;
            ulong m = (ulong)((Math.Log(10 * massRatio + 1) / Math.Log(11) + 1) / 2 * Giant.JupiterSMA);    // mean, the most likely radius

            Func<ulong, double> P = r => (4.0 * r * r * Math.Exp(-2.0 * r / m)) / (1.0 * m * m * m);
            Func<ulong, ulong, double> Q = (r, k) => Math.Exp(-1.0 * k * k / 10 / Math.Pow(r - k, 2));
            List<Func<ulong, double>> Px = new List<Func<ulong, double>>();
            Px.Add(P);
            foreach (var g in Childeren.FindAll(c => c.GetType() == typeof(Giant)))
            {
                Px.Add(r =>Q(r, g.Elements.SMA));
            }
            List<Tuple<double, ulong>> C = NormRiemannSum(Px,m);

            // Find the last tuple where (rand-item1 > 0). This is the left border
            int lb = C.FindLastIndex(t => rand - t.Item1 > 0);
            // Preform and return an linear interpolation on C
            ulong R = C[lb].Item2 + (ulong)((rand - C[lb].Item1) / (C[lb + 1].Item1 - C[lb].Item1) * (C[lb + 1].Item2 - C[lb].Item2));

            UnityEngine.Debug.Log("massRatio: " + massRatio + " Mean location: " + m.ToString("e2") + " Actual location: " + R.ToString("e2") + " Rand: " + rand);
            return R;
        }

        /// <summary>
        /// Projects the riemann sum of function p(x) to (int(p,0,t),t) for t = 0 to 6*m
        /// Then it normilizes this so the maximum sum is 1
        /// </summary>
        private List<Tuple<double, ulong>> NormRiemannSum(List<Func<ulong, double>> p, ulong m)
        {
            List<Tuple<double, ulong>> ret = new List<Tuple<double, ulong>>();
            double s = 0;
            for (ulong i = 0; i < m * 6; i += m / 20)    // 120 iterations
            {
                double term = 1;
                foreach (var q in p)
                {
                    term *= q(i);
                }
                s += term * m / 20;         // Right riemann sum
                ret.Add(new Tuple<double, ulong>(s, i));
            }
            // Normilize all points so end point is 1
            ret.ForEach(t => t.Item1 *= 1 / s);
            return ret;
        }

        public const ulong AU = 149597870700;
        public const ulong SolSize = 40 * AU;
    }
}
