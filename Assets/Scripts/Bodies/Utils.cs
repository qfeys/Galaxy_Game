using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{

    /// <summary>
    /// orbital elements. All angles in radian
    /// </summary>
    struct OrbitalElements
    {
        public readonly double LAN; // longitude of ascending node
        public readonly double i;   // inclination
        public readonly double AOP; // Argument of periapsis
        public readonly double MAaE;// Mean anomaly at epoch
        public readonly ulong SMA;  // Semi-major axis
        public readonly double e;   // exentricity
        public readonly Orbital parent;
        ulong T { get { return (ulong)(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G * parent.Mass))); } }
        public const double G = 6.67408e-11;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LAN">longitude of ascending node, between 0 and 2*PI</param>
        /// <param name="i">inclenation, between 0 and 2*PI</param>
        /// <param name="AOP">Argument of pereapsis, between 0 and 2*PI</param>
        /// <param name="MAaE">Mean anomaly at epoch, between 0 and 2*PI</param>
        /// <param name="SMA">Semi-Major axis</param>
        /// <param name="e">Eccentricity, 0 for circles, 1 for parabolas</param>
        /// <param name="parent"></param>
        public OrbitalElements(double LAN, double i, double AOP, double MAaE, ulong SMA, double e, Orbital parent)
        {
            this.LAN = LAN; this.i = i; this.AOP = AOP; this.MAaE = MAaE; this.SMA = SMA; this.e = e; this.parent = parent;
        }

        public VectorS GetPositionSphere(long time)
        {
            if (SMA == 0) return new VectorS(0, 0, 0);
            VectorS ret = new VectorS();
            double n = T / (2 * Math.PI);   // average rate of sweep
            double meanAnomaly = MAaE + n * time;
            double EA = eccentricAnomaly(meanAnomaly);
            ret.r = (ulong)(SMA * (1 - e * Math.Cos(EA)));
            ret.u = LAN + (AOP + EA) * Math.Sin(i);
            ret.v = i * Math.Sin(AOP + EA);
            return ret;
        }

        double eccentricAnomaly(double meanAnomaly, double guess = double.NaN)
        {
            if (double.IsNaN(guess)) guess = meanAnomaly;
            double newGuess = meanAnomaly + e * Math.Sin(guess);
            if (Math.Abs(guess - newGuess) < guess * 1e-10) return newGuess;
            return eccentricAnomaly(meanAnomaly, newGuess);
        }

        internal VectorS[] FindPointsOnOrbit(int number)
        {
            if (SMA == 0) throw new Exception("Cant find points of this orbit because this is not an orbit (sma = 0)");
            VectorS[] ret = new VectorS[number];
            for (int i = 0; i < number; i++)
            {
                VectorS point = new VectorS();
                double meanAnomaly = MAaE + i * 2 * Math.PI / number;
                double EA = eccentricAnomaly(meanAnomaly);
                point.r = (ulong)(SMA * (1 - e * Math.Cos(EA)));
                point.u = LAN + (AOP + EA) * Math.Sin(i);
                point.v = i * Math.Sin(AOP + EA);
                ret[i] = point;
            }
            return ret;
        }
    }

    /// <summary>
    /// A vector for spherical coordinates
    /// </summary>
    struct VectorS
    {
        // Radius
        public ulong r;
        // Angle on the ecliptica [0, 2xPi] [rad]
        public double u;
        // Angle away from the ecliptica [-Pi/2, Pi/2] [rad]
        public double v;

        /// <summary>
        /// A vector for spherical coordinates
        /// </summary>
        /// <param name="r">Radius</param>
        /// <param name="u">Angle on the ecliptica [0, 2xPi] [rad]</param>
        /// <param name="v">Angle away from the ecliptica [-Pi/2, Pi/2] [rad]</param>
        public VectorS(ulong r, double u, double v)
        {
            this.r = r;
            this.u = u % (2 * Math.PI);
            this.v = Math.IEEERemainder(v, Math.PI);
        }

        public static explicit operator UnityEngine.Vector3(VectorS vs)
        {
            return new UnityEngine.Vector3((float)(vs.r * Math.Cos(vs.u) * Math.Cos(vs.v)),
                                           (float)(vs.r * Math.Sin(vs.u) * Math.Cos(vs.v)),
                                           (float)(vs.r * Math.Sin(vs.v)));
        }

        public override string ToString()
        {
            return "{" + r.ToString("e2") + "," + u.ToString("0.00") + "," + v.ToString("0.00") + "}";
        }
    }

    class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1; Item2 = item2;
        }
    }
}
