using System;
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
        public readonly double LAN; // longitude of ascending node
        public readonly double i;   // inclination
        public readonly double AOP; // Argument of periapsis
        public readonly double MAaE;// Mean anomaly at epoch
        public readonly ulong SMA;  // Semi-major axis
        public readonly double e;   // exentricity
        public readonly Orbital parent;
        TimeSpan T { get { return TimeSpan.FromSeconds(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G * parent.Mass))); } }
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

        public VectorS GetPositionSphere(DateTime time)
        {
            if (SMA == 0) return new VectorS(0, 0, 0);
            VectorS ret = new VectorS();
            double n = T.TotalSeconds / (2 * Math.PI);   // average rate of sweep
            double meanAnomaly = MAaE + n * time.Second;
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
            for (int j = 0; j < number; j++)
            {
                VectorS point = new VectorS();
                double meanAnomaly = MAaE + j * 2 * Math.PI / number;
                double EA = eccentricAnomaly(meanAnomaly);
                point.r = (ulong)(SMA * (1 - e * Math.Cos(EA)));
                point.u = LAN + (AOP + EA) * Math.Cos(i);
                point.v = i * Math.Sin(AOP + EA);
                //UnityEngine.Debug.Log(j + ": " + j * 2 * Math.PI / number + "rad, MA: " + meanAnomaly.ToString("0.0") + " rad, EA: " + EA.ToString("0.0") + " rad, u:" + point.u.ToString("0.0") + " rad");
                ret[j] = point;
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

    public class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1; Item2 = item2;
        }
    }

    public static class RNG
    {
        static Random rand = new Random();
        public static bool Chance(double chance)
        {
            if (chance < 0) throw new ArgumentException("Chance must be greater then zero.");
            if (chance > 1) throw new ArgumentException("Chance must be smaller then one.");
            return rand.NextDouble() < chance;
        }

        public static TimeSpan nextOccurence(TimeSpan mtth)
        {
            return new TimeSpan((long)(mtth.Ticks * -Math.Log(rand.NextDouble(), 2)));
        }
    }

    public class SortedList<T> : ICollection<T>
    {
        private readonly List<T> collection = new List<T>();
        // TODO: initializable:
        private readonly IComparer<T> comparer = Comparer<T>.Default;

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
                    return; // already in the list, do nothing
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
            // TODO: potential optimization
            return collection.Contains(item);
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

        internal T FindFirst()
        {
            if (collection.Count != 0)
                return collection[0];
            else
                throw new NullReferenceException("This sorted list is empty");
        }

        public int Count { get { return collection.Count; } }
        public bool IsReadOnly { get { return false; } }
    }
}
