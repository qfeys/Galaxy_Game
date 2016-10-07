﻿using System;
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
    }

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
