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
        public readonly double SMA;  // Semi-major axis
        public readonly double e;   // exentricity
        public readonly double parentMass;
        // TimeSpan T { get { return TimeSpan.FromSeconds(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G * parentMass))); } }
        public TimeSpan T { get { return TimeSpan.FromSeconds(2 * Math.PI * Math.Sqrt(Math.Pow(SMA, 3) / (G_AU * parentMass))); } }
        /// <summary>
        /// The gravitational constant, unit: m^3 / (kg s)
        /// </summary>
        public const double G = 6.67408e-11;
        /// <summary>
        /// The gravitational constant, normalised to use AU instead of meters. unit: 1 / (kg s)
        /// </summary>
        public const double G_AU = 4.46134e-22;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LAN">longitude of ascending node, between 0 and 2*PI</param>
        /// <param name="i">inclenation, between 0 and 2*PI</param>
        /// <param name="AOP">Argument of pereapsis, between 0 and 2*PI</param>
        /// <param name="MAaE">Mean anomaly at epoch, between 0 and 2*PI</param>
        /// <param name="SMA">Semi-Major axis, in AU</param>
        /// <param name="e">Eccentricity, 0 for circles, 1 for parabolas</param>
        /// <param name="parent"></param>
        public OrbitalElements(double LAN, double i, double AOP, double MAaE, double SMA, double e, double parentMass)
        {
            this.LAN = LAN; this.i = i; this.AOP = AOP; this.MAaE = MAaE; this.SMA = SMA; this.e = e; this.parentMass = parentMass;
        }

        public VectorS GetPositionSphere(DateTime time)
        {
            if (SMA == 0) return new VectorS(0, 0, 0);
            VectorS ret = new VectorS();
            double n = T.TotalSeconds / (2 * Math.PI);   // average rate of sweep
            double meanAnomaly = MAaE + n * time.Second;
            double EA = EccentricAnomaly(meanAnomaly);
            ret.r = (ulong)(SMA * (1 - e * Math.Cos(EA)));
            ret.u = LAN + (AOP + EA) * Math.Sin(i);
            ret.v = i * Math.Sin(AOP + EA);
            return ret;
        }

        double EccentricAnomaly(double meanAnomaly, double guess = double.NaN)
        {
            if (double.IsNaN(guess))
                guess = meanAnomaly;
            double newGuess = meanAnomaly + e * Math.Sin(guess);
            if (Math.Abs(guess - newGuess) < guess * 1e-10)
                return newGuess;
            return EccentricAnomaly(meanAnomaly, newGuess);
        }

        internal VectorS[] FindPointsOnOrbit(int number)
        {
            if (SMA == 0) throw new Exception("Cant find points of this orbit because this is not an orbit (sma = 0)");
            VectorS[] ret = new VectorS[number];
            for (int j = 0; j < number; j++)
            {
                double meanAnomaly = MAaE + j * 2 * Math.PI / number;
                double EA = EccentricAnomaly(meanAnomaly);
                VectorS point = new VectorS() {
                    r = (ulong)(SMA * (1 - e * Math.Cos(EA))),
                    u = LAN + (AOP + EA) * Math.Cos(i),
                    v = i * Math.Sin(AOP + EA)
                };
                //UnityEngine.Debug.Log(j + ": " + j * 2 * Math.PI / number + "rad, MA: " + meanAnomaly.ToString("0.0") + " rad, EA: " + EA.ToString("0.0") + " rad, u:" + point.u.ToString("0.0") + " rad");
                ret[j] = point;
            }
            return ret;
        }

        public static OrbitalElements Center { get { return new OrbitalElements(0, 0, 0, 0, 0, 0, 0); } }
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

        /// <summary>
        /// A random number between 0 and 2 Pi
        /// </summary>
        public double Circle { get { return irand.NextDouble() * 2 * Math.PI; } }
    }


    public class SortedList<T> : ICollection<T>
    {
        private readonly List<T> collection = new List<T>();
        // TODO: initializable:
        public SortedList()
        {
            comparer = Comparer<T>.Default;
        }
        public SortedList(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        private readonly IComparer<T> comparer;

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

        public int Count { get { return collection.Count; } }
        public bool IsReadOnly { get { return false; } }
    }
}
