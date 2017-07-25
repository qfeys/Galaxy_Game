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

        #region Generation code

        public override void Generate(double mass, Random rand)
        {
            Star s = new Star(this, mass * 0.995, new OrbitalElements());
            //s.Generate(mass*0.995,rand);
            mass *= .005;
            int i = 0;
            while (mass > 1e25 || i>10)
            {
                double gMass = (rand.NextDouble() * 0.35 + 0.6) * mass;  // between 60% and 95% of leftover mass
                ulong sma = ChooseNewGiantSMA(rand.NextDouble(), gMass);
                OrbitalElements gElem = new OrbitalElements(
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 0.09 - 0.045,           // between -0.045 and 0.045 rad
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 2 * Math.PI,
                    sma,
                    rand.NextDouble() * 0.01,
                    s);
                
                Giant g = new Giant(this, gMass, gElem);

                mass -= gMass;
                i++;
            }
            while(mass > 1e20 || i > 30)
            {
                double rMass = (rand.NextDouble() * 0.35 + 0.5) * mass;  // between 50% and 85% of leftover mass
                ulong sma = ChooseNewRockSMA(rand.NextDouble(), rMass);
                OrbitalElements gElem = new OrbitalElements(
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 0.18 - 0.09,           // between -0.09 and 0.09 rad
                    rand.NextDouble() * 2 * Math.PI,
                    rand.NextDouble() * 2 * Math.PI,
                    sma,
                    rand.NextDouble() * 0.01,
                    s);

                bool breathable = rand.NextDouble() > 0.2;
                Rock r = new Rock(this, rMass, gElem, 60*60*24, 0.1, breathable);

                mass -= rMass;
                i++;
            }
        }

        /// <summary>
        /// Using Newtons methode to find the SMA from a probability function
        /// </summary>
        /// <returns></returns>
        ulong ChooseNewGiantSMA(double rand, double gMass)
        {
            // (4*R*R*e^(-2*R/m))/m^3                       Probability function
            // e^(-k^2 / 10 / (r - k)^2)                     Probability modifier on presence other giant
            // ((log(10,x*10+1)*log(11,10)+1)/2             formulla most likly radius based on mass
            double massRatio = gMass / Giant.JUPITER_MASS;
            ulong m = 8 * AU;                   // mean, the most likely radius
            
            Func<ulong, ulong, double, double> P = (r, k, rtmr) => (4.0 * r * r * Math.Exp(-2.0 * r / m)) / (1.0 * m * m * m);
            Func<ulong, ulong, double, double> Q = (r, k, rtmr) => Math.Exp(-rtmr * k * k / 3 / Math.Pow(0.0 + r - k, 2)); // rmtr stands for root of mass ratio
            Dictionary<Func<ulong, ulong, double, double>, List<Tuple<ulong, double>>> Px =
                new Dictionary<Func<ulong, ulong, double, double>, List<Tuple<ulong, double>>> {
                    { P, new List<Tuple<ulong, double>>() { new Tuple<ulong, double>(0, 0) } },
                    {
                        Q,
                        Childeren.FindAll(c => (c.GetType() == typeof(Giant) || c.GetType() == typeof(Rock))).
                Select(g => new Tuple<ulong, double>(g.Elements.SMA, Math.Sqrt(Math.Sqrt(g.Mass / Giant.JUPITER_MASS)))).ToList()
                    }
                };
            List<Tuple<double, ulong>> C = NormRiemannSum(Px, 6 * m);

            // Find the last tuple where (rand-item1 > 0). This is the left border
            int lb = C.FindLastIndex(t => rand - t.Item1 > 0);
            // Preform and return an linear interpolation on C
            ulong R = C[lb].Item2 + (ulong)((rand - C[lb].Item1) / (C[lb + 1].Item1 - C[lb].Item1) * (C[lb + 1].Item2 - C[lb].Item2));
            return R;
        }
        
        /// <summary>
        /// Using Newtons methode to find the SMA from a probability function
        /// </summary>
        /// <returns></returns>
        ulong ChooseNewRockSMA(double rand, double rMass)
        {
            // 3/2* (-(x/a)^2 + 1)/a                        Probability function
            // e^(-k^2 / 10 / (r - k)^2)                     Probability modifier on presence other giant
            // ((log(10,x*10+1)*log(11,10)+1)/2             formulla most likly radius based on mass
            ulong m = 8 * AU;   // mean distance
            
            Func<ulong, ulong, double, double> P = (r, k, rtmr) => (4.0 * r * r * Math.Exp(-2.0 * r / m)) / (1.0 * m * m * m);
            Func<ulong, ulong, double, double> Q = (r, k, rtmr) => Math.Exp(-rtmr * k * k / 3 / Math.Pow(0.0 + r - k, 2)); // rmtr stands for root of mass ratio
            Dictionary<Func<ulong, ulong, double, double>, List<Tuple<ulong, double>>> Px =
                new Dictionary<Func<ulong, ulong, double, double>, List<Tuple<ulong, double>>> {
                    { P, new List<Tuple<ulong, double>>() { new Tuple<ulong, double>(0, 0) } },
                    {
                        Q,
                        Childeren.FindAll(c => (c.GetType() == typeof(Giant) || c.GetType() == typeof(Rock))).
                Select(g => new Tuple<ulong, double>(g.Elements.SMA, Math.Sqrt(Math.Sqrt(g.Mass / Giant.JUPITER_MASS)))).ToList()
                    }
                };
            List<Tuple<double, ulong>> C = NormRiemannSum(Px, 6 * m);

            // Find the last tuple where (rand-item1 > 0). This is the left border
            int lb = C.FindLastIndex(t => rand - t.Item1 > 0);
            // Preform and return an linear interpolation on C
            ulong R = C[lb].Item2 + (ulong)((rand - C[lb].Item1) / (C[lb + 1].Item1 - C[lb].Item1) * (C[lb + 1].Item2 - C[lb].Item2));
            
            return R;
        }

        /// <summary>
        /// Projects the riemann sum of function p(x) to (int(p,0,t),t) for t = 0 to 6*m
        /// Then it normilizes this so the maximum sum is 1
        /// </summary>
        private List<Tuple<double, ulong>> NormRiemannSum(Dictionary<Func<ulong, ulong, double, double>, List<Tuple<ulong, double>>> px, ulong max)
        {
            List<Tuple<double, ulong>> ret = new List<Tuple<double, ulong>>();
            double s = 0;
            ret.Add(new Tuple<double, ulong>(s, 0));
            for (ulong i = 1; i < max; i += max / 100)    // 100 iterations
            {
                double term = 1;
                foreach (var p in px)
                {
                    foreach(Tuple<ulong, double> t in p.Value)
                    {
                        term *= p.Key(i, t.Item1, t.Item2);
                    }
                }
                s += term * max / 100;         // Right riemann sum
                ret.Add(new Tuple<double, ulong>(s, i));
            }
            // Normilize all points so end point is 1
            ret.ForEach(t => t.Item1 *= 1 / s);
            //UnityEngine.Debug.Log("chances: " + string.Join(";", ret.Select(tu => tu.Item1.ToString("0.0000")).ToArray()));
            return ret;
        }

        public const ulong AU = 149597870700;
        public const ulong SOL_SIZE = 40 * AU;

#endregion


        internal Orbital RandLivableWorld()
        {
            foreach(Orbital o in Childeren)
            {
                if (o.GetType() == typeof(Rock) && ((Rock)o).Habitable)
                    return o;
            }
            UnityEngine.Debug.Log("No livable world detected.");
            return null;
        }
    }
}
