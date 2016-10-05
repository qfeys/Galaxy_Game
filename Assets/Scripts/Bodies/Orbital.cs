using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    abstract class Orbital
    {
        Orbital Parent;
        List<Orbital> Childeren;
        public double Mass { get; private set; }
        OrbitalElements Elements;
        ulong id;
        static ulong idCounter = 0;

        public Orbital(Orbital parent, double mass, OrbitalElements elements)
        {
            Parent = parent; Mass = mass; Elements = elements;
            id = idCounter;
            idCounter++;
            if(parent != null)
                Parent.Childeren.Add(this);
        }

        public abstract void Generate(double mass, Random rand);
    }

    /// <summary>
    /// orbital elements. All angles in radian
    /// </summary>
    struct OrbitalElements
    {
        double LAN; // longitude of ascending node
        double i;   // inclination
        double AOP; // Argument of periapsis
        double MAaE;// Mean anomaly at epoch
        ulong SMA;  // Semi-major axis
        double e;   // exentricity
        Orbital parent;
        ulong T { get { return (ulong)(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G * parent.Mass))); } }
        const double G = 6.67408e-11;

        public VectorS GetPositionSphere(long time)
        {
            VectorS ret = new VectorS();
            double n = T / (2 * Math.PI);   // average rate of sweep
            double meanAnomaly = MAaE + n * time;
            double EA = eccentricAnomaly(meanAnomaly);
            ret.r = (ulong)( SMA * (1 - e * Math.Cos(EA)));
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
    }
}
