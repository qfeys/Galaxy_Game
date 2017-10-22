using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    /// <summary>
    /// This class provides a double and it's derivative, so it can be evaluated for arbitary values in the future.
    /// </summary>
    class Integrated
    {
        /// <summary>
        /// The constand. This is the value at the moment
        /// </summary>
        double c;
        /// <summary>
        /// The derivative. This is the dirivative of the value.
        /// It must be valid at all moments.
        /// The delta t is one second.
        /// </summary>
        double d;
        /// <summary>
        /// The moment at which c is the value.
        /// </summary>
        DateTime m;

        public Integrated(double c, double d, DateTime m)
        {
            this.c = c; this.d = d; this.m = m;
        }

        public Integrated(double c)
        {
            this.c = c;
            d = 0;
            m = Simulation.God.Time;
        }

        /// <summary>
        /// This is the value a the given moment
        /// </summary>
        /// <param name="m2"></param>
        /// <returns></returns>
        public double Value(DateTime m2)
        {
            return m == m2 ? c : c + d * (m2 - m).TotalSeconds;
        }

        public static Integrated operator +(Integrated a, Integrated b)
        {
            DateTime dt = a.m > b.m ? a.m : b.m;
            return new Integrated(a.Value(dt) + a.Value(dt), a.d + b.d, dt);
        }

        public static Integrated operator -(Integrated a, Integrated b)
        {
            DateTime dt = a.m > b.m ? a.m : b.m;
            return new Integrated(a.Value(dt) - a.Value(dt), a.d - b.d, dt);
        }

        public static Integrated operator +(Integrated a, double b)
        {
            return new Integrated(a.c + b, a.d, a.m);
        }

        public static Integrated operator -(Integrated a, double b)
        {
            return new Integrated(a.c - b, a.d, a.m);
        }

        public static Integrated operator *(Integrated a, double b)
        {
            return new Integrated(a.c * b, a.d * b, a.m);
        }

        public static Integrated operator /(Integrated a, double b)
        {
            return new Integrated(a.c / b, a.d / b, a.m);
        }
    }
}
