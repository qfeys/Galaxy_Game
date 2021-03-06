﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Bodies;

namespace Assets.Scripts
{

    /// <summary>
    /// orbital elements. All angles in radian
    /// </summary>
    struct OrbitalElements
    {
        public readonly double AOP; // Argument of periapsis
        public readonly double MAaE;// Mean anomaly at epoch
        public readonly double SMA;  // Semi-major axis in meter
        public readonly double e;   // exentricity
        public readonly double parentMass; // in kg
        // TimeSpan T { get { return TimeSpan.FromSeconds(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G * parentMass))); } }
        public AstroTimeSpan T; 
        /// <summary>
        /// The gravitational constant, unit: m^3 / (kg s)
        /// </summary>
        public const double G = 6.67408e-11;
        /// <summary>
        /// The gravitational constant, normalised to use AU instead of meters. unit: 1 / (kg s)
        /// </summary>
        public const double G_AU = 19.934712e-45;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AOP">Argument of pereapsis, between 0 and 2*PI</param>
        /// <param name="MAaE">Mean anomaly at epoch, between 0 and 2*PI</param>
        /// <param name="SMA">Semi-Major axis, in AU</param>
        /// <param name="e">Eccentricity, 0 for circles, 1 for parabolas</param>
        /// <param name="parent">The mass of the parent object, in kg</param>
        public OrbitalElements(double AOP, double MAaE, double SMA, double e, double parentMass)
        {
            this.AOP = AOP; this.MAaE = MAaE; this.SMA = SMA; this.e = e; this.parentMass = parentMass;
            T = CalculatePeriode(SMA, parentMass);
        }

        /// <summary>
        /// The position of this object in spherical coordinates, with r in AU,
        /// with zero at the place of the barycentrum of this orbit
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public VectorP GetPositionCircle(DateTime time)
        {
            if (SMA == 0) return new VectorP(0, 0);
            double n = T.TotalSeconds / (2 * Math.PI);   // average rate of sweep (s/rad)
            double meanAnomaly = MAaE + (time - EPOCH).TotalSeconds / n;
            if (double.IsNaN(meanAnomaly))
                UnityEngine.Debug.LogError("NaN detected");
            double EA = EccentricAnomaly(meanAnomaly);
            VectorP ret = new VectorP(this, EA);
            return ret;
        }

        public UnityEngine.Vector2 GetPosition(DateTime time)
        {
            return (UnityEngine.Vector2)GetPositionCircle(time);
        }

        /// <summary>
        /// Calculates the eccentric anomaly in an iterative manner
        /// </summary>
        /// <param name="meanAnomaly"></param>
        /// <param name="guess"></param>
        /// <returns></returns>
        double EccentricAnomaly(double meanAnomaly)
        {
            if (meanAnomaly == 0)
                return 0;
            double guess = meanAnomaly;
            double newGuess = meanAnomaly + e * Math.Sin(meanAnomaly);
            while (Math.Abs(guess - newGuess) > guess * 1e-10)
            {
                guess = newGuess;
                newGuess = meanAnomaly + e * Math.Sin(guess);
            }
            return newGuess;
        }

        internal VectorP[] FindPointsOnOrbit(int number, DateTime time)
        {
            if (SMA == 0) throw new Exception("Cant find points of this orbit because this is not an orbit (sma = 0)");
            VectorP[] ret = new VectorP[number + 1];
            double n = T.TotalSeconds / (2 * Math.PI);   // average rate of sweep (s/rad)
            for (int j = 0; j <= number; j++)
            {
                double meanAnomaly = MAaE + j * 2 * Math.PI / number + (time - EPOCH).TotalSeconds / n;
                double EA = EccentricAnomaly(meanAnomaly);
                VectorP point = new VectorP(this, EA);
                ret[j] = point;
            }
            return ret;
        }

        /// <summary>
        /// Calculates the periode T 
        /// </summary>
        static AstroTimeSpan CalculatePeriode(double SMA, double parentMass)
        {
            if (parentMass == 0)
                throw new Exception("Parent mass == 0");
            double a = Math.Pow(SMA, 3);
            double b = Math.Pow(SMA, 3) / (G_AU * parentMass);
            double c = Math.Sqrt(Math.Pow(SMA, 3) / (G_AU * parentMass));
            double d = 2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G_AU * parentMass));
            AstroTimeSpan T = AstroTimeSpan.FromSeconds(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G_AU * parentMass)));
            if (T.TotalSeconds == 0)
                throw new Exception("Periode == 0");
            return T;
        }

        public static OrbitalElements Center { get { return new OrbitalElements(0, 0, 0, 0, double.Epsilon); } }
        public static readonly DateTime EPOCH = new DateTime(2100, 1, 1);
    }

    /// <summary>
    /// A vector for polar coordinates
    /// </summary>
    struct VectorP
    {
        /// <summary>
        /// Radius, recomended to be in AU
        /// </summary>
        public double r;
        /// <summary>
        /// Angle on the ecliptica [0, 2xPi] [rad]
        /// </summary>
        public double u;

        /// <summary>
        /// A vector for spherical coordinates
        /// </summary>
        /// <param name="r">Radius, recommended in AU</param>
        /// <param name="u">Angle on the ecliptica [0, 2xPi] [rad]</param>
        public VectorP(double r, double u)
        {
            this.r = r;
            this.u = u % (2 * Math.PI);
        }

        /// <summary>
        /// The position of a point on an orbit at a certain eccentric anomaly.
        /// </summary>
        /// <param name="el"></param>
        /// <param name="EA"></param>
        public VectorP(OrbitalElements el, double EA)
        {
            double AoA = el.AOP + EA;  // Argument of anomaly
            r = el.SMA * (1 - el.e * Math.Cos(EA));
            u = AoA;
        }

        public static explicit operator UnityEngine.Vector2(VectorP vs)
        {
            return new UnityEngine.Vector2((float)(vs.r * Math.Cos(vs.u)),
                                           (float)(vs.r * Math.Sin(vs.u)));
        }

        public override string ToString()
        {
            return "{" + r.ToString("0.00") + "," + u.ToString("0.00") + "}";
        }
    }

    /// <summary>
    /// A vector for spherical coordinates
    /// </summary>
    struct VectorS
    {
        /// <summary>
        /// Radius, recomended to be in AU
        /// </summary>
        public double r;
        /// <summary>
        /// Angle on the ecliptica [0, 2xPi] [rad]
        /// </summary>
        public double u;
        /// <summary>
        /// Angle away from the ecliptica [-Pi/2, Pi/2] [rad]
        /// </summary>
        public double v;

        /// <summary>
        /// A vector for spherical coordinates
        /// </summary>
        /// <param name="r">Radius, recommended in AU</param>
        /// <param name="u">Angle on the ecliptica [0, 2xPi] [rad]</param>
        /// <param name="v">Angle away from the ecliptica [-Pi/2, Pi/2] [rad]</param>
        public VectorS(double r, double u, double v)
        {
            this.r = r;
            this.u = u % (2 * Math.PI);
            this.v = Math.IEEERemainder(v, Math.PI);
        }

        /*
        /// <summary>
        /// The position of a point on an orbit at a certain eccentric anomaly.
        /// </summary>
        /// <param name="el"></param>
        /// <param name="EA"></param>
        public VectorS(OrbitalElements el, double EA)
        {
            double AoA = el.AOP + EA;  // Argument of anomaly
            r = el.SMA * (1 - el.e * Math.Cos(EA));
            u = el.LAN + Math.Atan2(Math.Sin(AoA) * Math.Cos(el.i), Math.Cos(AoA) * Math.Cos(el.i));
            v = Math.Asin(Math.Sin(el.i) * Math.Sin(AoA));
        }
        */

        public static explicit operator UnityEngine.Vector3(VectorS vs)
        {
            return new UnityEngine.Vector3((float)(vs.r * Math.Cos(vs.u)),
                                           (float)(vs.r * Math.Sin(vs.u)),
                                           (float)(vs.r * Math.Sin(vs.v)));
        }

        public override string ToString()
        {
            return "{" + r.ToString("0.00") + "," + u.ToString("0.00") + "," + v.ToString("0.00") + "}";
        }
    }

    public class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1; Item2 = item2;
        }
    }

    public class RNG
    {
        // Static functions
        static Random rand = new Random();
        public static bool Chance(double chance)
        {
            if (chance < 0) throw new ArgumentException("Chance must be greater then zero.");
            if (chance > 1) throw new ArgumentException("Chance must be smaller then one.");
            return rand.NextDouble() < chance;
        }

        public static TimeSpan NextOccurence(TimeSpan mtth)
        {
            return new TimeSpan((long)(mtth.Ticks * -Math.Log(rand.NextDouble(), 2)));
        }

        // instance functions
        Random irand;
        public RNG(int seed)
        {
            irand = new Random(seed);
        }

        public int D10 { get { return irand.Next(1, 11); } }
        public int D100 { get { return irand.Next(1, 101); } }
        public int D1000 { get { return irand.Next(1, 1001); } }

        public int nD10(int n)
        {
            int s = 0;
            for (int i = 0; i < n; i++)
            {
                s += D10;
            }
            return s;
        }

        /// <summary>
        /// A random number between 0 and 2 Pi
        /// </summary>
        public double Circle { get { return irand.NextDouble() * 2 * Math.PI; } }
    }

    public class SortedList<T> : ICollection<T>
    {
        private readonly List<T> collection = new List<T>();
        private readonly IComparer<T> comparer;

        public SortedList()
        {
            comparer = Comparer<T>.Default;
        }
        public SortedList(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }
        private SortedList(IComparer<T> comparer, List<T> initial)
        {
            collection = initial;
            this.comparer = comparer;
            Resort();
        }


        public void Add(T item)
        {
            if (Count == 0)
            {
                collection.Add(item);
                return;
            }
            int minimum = 0;
            int maximum = collection.Count - 1;

            while (minimum <= maximum)
            {
                int midPoint = (minimum + maximum) / 2;
                int comparison = comparer.Compare(collection[midPoint], item);
                if (comparison == 0)
                {
                    collection.Insert(midPoint + 1, item);
                    return; // already in the list, isert item here
                }
                if (comparison < 0)
                {
                    minimum = midPoint + 1;
                }
                else
                {
                    maximum = midPoint - 1;
                }
            }
            collection.Insert(minimum, item);
        }

        public bool Contains(T item)
        {
            int length = collection.Count;
            if (length < 20) // Too small, prevent overhead
                return collection.Contains(item);
            int current = length / 2;
            int last = 0;
            while (last != current)
            {
                int comp = comparer.Compare(item, collection[current]);
                int diff = Math.Abs(current - last);
                last = current;
                if (comp == 0)
                    return true;
                else
                    current += comp * diff / 2;
            }
            return false;
        }

        public bool Remove(T item)
        {
            // TODO: potential optimization
            return collection.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            collection.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        internal T PeekFirst()
        {
            if (collection.Count != 0)
                return collection[0];
            else
                throw new NullReferenceException("This sorted list is empty");
        }

        internal T TakeFirst()
        {
            if (collection.Count != 0)
            {
                var ret = collection[0];
                collection.RemoveAt(0);
                return ret;
            }
            else
                throw new NullReferenceException("This sorted list is empty");
        }

        internal void Resort()
        {
            collection.Sort(comparer);
        }

        public T this[int key]
        {
            get
            {
                return collection[key];
            }
        }

        public int Count { get { return collection.Count; } }
        public bool IsReadOnly { get { return false; } }

        internal void ForEach(Action<T> p)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                p(collection[i]);
            }
        }

        internal SortedList<U> ConvertAll<U>(Converter<T, U> p, IComparer<U> com, bool resort = false)
        {
            var r = new SortedList<U>(com, collection.ConvertAll<U>(p));
            if (resort)
                r.Resort();
            return r;
        }

        internal void RemoveAll(Predicate<T> p)
        {
            collection.RemoveAll(p);
        }

        internal void RemoveList(List<T> list, bool iterateBackwards = false)
        {
            int l = collection.Count;
            int amountLeft = list.Count;
            if (amountLeft == 0) return;
            for (int i = iterateBackwards ? l - 1 : 0; iterateBackwards ? i >= 0 : i < l; i += iterateBackwards ? -1 : +1)
            {
                if (list.Contains(collection[i]))
                {
                    collection.RemoveAt(i);
                    i += iterateBackwards ? 0 : -1;
                    amountLeft--;
                    if (amountLeft <= 0)
                        return;
                }
            }
        }

        internal static SortedList<T> FromList(IEnumerable<T> list, Comparer<T> comp)
        {
            SortedList<T> sl = new SortedList<T>(comp);
            foreach (T item in list)
            {
                sl.Add(item);
            }
            return sl;
        }
    }

    public class AstroTimeSpan
    {
        /// <summary>
        /// Represent seconds
        /// </summary>
        long ticks;

        AstroTimeSpan(long ticks)
        {
            this.ticks = ticks;
        }

        public static AstroTimeSpan FromSeconds(long seconds) { return new AstroTimeSpan(seconds); }
        public static AstroTimeSpan FromSeconds(double seconds) { return new AstroTimeSpan((long)seconds); }
        public static AstroTimeSpan FromMinutes(long minutes) { return new AstroTimeSpan(minutes * 60); }
        public static AstroTimeSpan FromMinutes(double minutes) { return new AstroTimeSpan((long)(minutes * 60)); }
        public static AstroTimeSpan FromHours(long hours) { return new AstroTimeSpan(hours * 3600); }
        public static AstroTimeSpan FromHours(double hours) { return new AstroTimeSpan((long)(hours * 3600)); }
        public static AstroTimeSpan FromDays(long days) { return new AstroTimeSpan(days * 86400); }
        public static AstroTimeSpan FromDays(double days) { return new AstroTimeSpan((long)(days * 86400)); }

        public long TotalSeconds { get { return ticks; } }
        public long TotalMinutes { get { return ticks / 60; } }
        public long TotalHours { get { return ticks / 3600; } }
        public long TotalDays { get { return ticks / 86400; } }
    }
}
