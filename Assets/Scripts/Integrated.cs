using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Simulation;

namespace Assets.Scripts
{
    /// <summary>
    /// This class provides a double and it's derivative, so it can be evaluated for arbitary values in the future.
    /// </summary>
    abstract class Integrated
    {

        public static Integrated Create(double c, double d, DateTime m)
        {
            Original nw = new Original(c, d, m);
            return nw;
        }

        public static Integrated Create(double c)
        {
            Original nw = new Original(c, 0, Simulation.God.Time);
            return nw;
        }

        /// <summary>
        /// This is the value a the given moment
        /// </summary>
        /// <param name="m2"></param>
        /// <returns></returns>
        abstract public double Value(DateTime m2);

        static Integrated Combine(Integrated a, double c1, Integrated b, double c2)
        {
            if (a is Original)
            {
                Original ao = a as Original;
                if (b is Original)
                {
                    Original bo = b as Original;
                    return new Combination.Linear(new List<Combination.Linear.OriginalFraction>() {
                        new Combination.Linear.OriginalFraction(ao, c1),
                        new Combination.Linear.OriginalFraction(bo, c2)
                    });
                }
                else if (b is Combination.Linear)
                {
                    Combination.Linear blc = b as Combination.Linear;
                    Combination.Linear ans = new Combination.Linear(blc, c2);
                    ans.AddOriginal(ao, c1);
                    return ans;
                }
                else if (b is Combination.NonLinear)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    throw new Exception("WTF happened here?!?");
                }
            }
            else if (a is Combination.Linear)
            {
                Combination.Linear alc = a as Combination.Linear;
                if (b is Original)
                    return Combine(b, c2, a, c1);
                else if (b is Combination.Linear)
                {
                    Combination.Linear blc = b as Combination.Linear;
                    return new Combination.Linear(alc, blc, c1, c2);
                }
                else if (b is Combination.NonLinear)
                    throw new NotImplementedException();
                else
                    throw new Exception("WTF happened here?!?");
            }
            else if (a is Combination.NonLinear)
            {
                return b + a;
            }
            else
                throw new Exception("WTF happened here?!?");
        }

        internal abstract void Subscribe(Event.Conditional conditional);

        /// <summary>
        /// Returns the moment at which this Integrated will reach the trigger value
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        internal abstract DateTime FindMomentAtValue(double trigger);

        public static Integrated operator +(Integrated a, Integrated b)
        {
            return Combine(a, 1, b, 1);
        }

        public static Integrated operator -(Integrated a, Integrated b)
        {
            return Combine(a, 1, b, -1);
        }

        public static Integrated operator *(Integrated a, double b)
        {
            if (a is Original)
                return new Combination.Linear(a as Original, b);
            else if (a is Combination.Linear)
                return new Combination.Linear(a as Combination.Linear, b);
            else if (a is Combination.NonLinear)
                throw new NotImplementedException();
            else
                throw new Exception("Not a valid sub class");
        }

        public static Integrated operator /(Integrated a, double b)
        {
            return a * (1 / b);
        }

        public static implicit operator Integrated(double d)
        {
            return Create(d);
        }

        /// <summary>
        /// This is an original integrated. These are the only integrated the programm
        /// can create directly.
        /// </summary>
        class Original : Integrated
        {
            /// <summary>
            /// The constand. This is the value at the moment
            /// </summary>
            public double c;
            /// <summary>
            /// The derivative. This is the dirivative of the value.
            /// It must be valid at all moments.
            /// The delta t is one second.
            /// </summary>
            public double d;
            /// <summary>
            /// The moment at which c is the value.
            /// </summary>
            public DateTime m;

            /// <summary>
            /// List of all events subscribed to this Integrated
            /// </summary>
            List<Event.Conditional> subscriptions;

            public Original(double c, double d, DateTime m)
            {
                this.c = c;
                this.d = d;
                this.m = m;
                subscriptions = new List<Event.Conditional>();
            }

            public override double Value(DateTime m2)
            {
                return m == m2 ? c : c + d * (m2 - m).TotalSeconds;
            }

            internal override DateTime FindMomentAtValue(double trigger)
            {
                double seconds = (trigger - c) / d;
                return m + TimeSpan.FromSeconds(seconds);
            }

            internal override void Subscribe(Event.Conditional conditional)
            {
                subscriptions.Add(conditional);
            }
        }

        abstract class Combination : Integrated
        {
            /// <summary>
            /// This class is a linear combination of Original integrated
            /// </summary>
            public class Linear : Combination
            {
                public List<OriginalFraction> fractions;

                public Linear(Original a, double b)
                {
                    fractions = new List<OriginalFraction>() {
                        new OriginalFraction(a,b)
                    };
                }

                public Linear(Linear lc, double value = 1)
                {
                    this.fractions = new List<OriginalFraction>();
                    lc.fractions.ForEach(of => fractions.Add(of.Copy()));
                    if (value != 1)
                        fractions.ForEach(of => of.value *= value);
                }

                public Linear(List<OriginalFraction> fractions)
                {
                    this.fractions = new List<OriginalFraction>(fractions);
                }

                public Linear(Linear alc, Linear blc, double c1 = 1, double c2 = 1)
                {
                    this.fractions = new List<OriginalFraction>();
                    alc.fractions.ForEach(of => fractions.Add(of.Copy()));
                    if (c1 != 1)
                        fractions.ForEach(of => of.value *= c1);
                    blc.fractions.ForEach(ofb => AddOriginal(ofb.original, ofb.value));
                }

                public void AddOriginal(Original o, double fraction)
                {
                    if (fractions.Any(of => of.original == o))
                        fractions.Find(of => of.original == o).value += fraction;
                    else
                        fractions.Add(new OriginalFraction(o, fraction));
                }

                public override double Value(DateTime m2)
                {
                    double ans = 0;
                    fractions.ForEach(of => ans += of.original.Value(m2) * of.value);
                    return ans;
                }

                internal override DateTime FindMomentAtValue(double trigger)
                {
                    DateTime mostRecent = fractions.Max(f => f.original.m);
                    double cSum = 0; double dSum = 0;
                    foreach(OriginalFraction fr in fractions)
                    {
                        cSum += fr.original.Value(mostRecent) * fr.value;
                        dSum += fr.original.d * fr.value;
                    }
                    double seconds = (trigger - cSum) / dSum;
                    return mostRecent + TimeSpan.FromSeconds(seconds);
                }

                internal override void Subscribe(Event.Conditional conditional)
                {
                    fractions.ForEach(f => f.original.Subscribe(conditional));
                }

                public class OriginalFraction
                {
                    public readonly Original original;
                    public double value;

                    public OriginalFraction(Original original, double value)
                    {
                        this.original = original;
                        this.value = value;
                    }

                    public OriginalFraction Copy()
                    {
                        return new OriginalFraction(original, value);
                    }
                }
            }

            public class NonLinear : Combination
            {
                public override double Value(DateTime m2)
                {
                    throw new NotImplementedException();
                }

                internal override DateTime FindMomentAtValue(double trigger)
                {
                    throw new NotImplementedException();
                }

                internal override void Subscribe(Event.Conditional conditional)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
