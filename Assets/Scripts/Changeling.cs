using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Simulation;

namespace Assets.Scripts
{
    /// <summary>
    /// A changeling is a double that changes over time and is the base of the entire event system.
    /// Its value can be evaluated for arbitary values in the future.
    /// Functions can subscribe to certain values and will be creaate an event when this value triggers.
    /// You should try to keep the value linear over time whenever possible (eg. a*x + b)
    /// </summary>
    abstract class Changeling
    {

        /// <summary>
        /// List of all events subscribed to this Changeling
        /// </summary>
        List<Subscription> subscriptions;

        /// <summary>
        /// Creates a linear changeling with a value and derivative at a certain moment.
        /// c + d*t
        /// </summary>
        /// <param name="c">The value</param>
        /// <param name="d">The derivative (units per second)</param>
        /// <param name="m">The moment at which this value is correct</param>
        /// <returns></returns>
        public static Changeling Create(double c, double d, DateTime m)
        {
            Original nw = new Original(c, d, m);
            return nw;
        }

        /// <summary>
        /// Creates a changeling that is constant over time. You should probably avoid this.
        /// </summary>
        /// <param name="c">The value</param>
        /// <returns></returns>
        [Obsolete("You are using a constant changeling. This is probably pointless. Use a normal double instead.")]
        public static Changeling Create(double c)
        {
            Original nw = new Original(c, 0, Simulation.God.Time);
            return nw;
        }

        /// <summary>
        /// Creating a higher order changeling.
        /// Not yet implemented.
        /// </summary>
        /// <param name="c">The value</param>
        /// <param name="d">The derivative (units per second)</param>
        /// <param name="m">The moment at which this value is correct</param>
        /// <returns></returns>
        public static Changeling Create(double c, Changeling d, DateTime m)
        {
            throw new NotImplementedException("Second order changelings");
        }

        abstract public void Modify(double c, double d, DateTime m);

        /// <summary>
        /// This is the value a the given moment
        /// </summary>
        /// <param name="m2"></param>
        /// <returns></returns>
        abstract public double Value(DateTime m2);

        public double Value()
        {
            return Value(God.Time);
        }

        public static implicit operator double(Changeling ch)
        {
            return ch.Value();
        }

        /// <summary>
        /// Combine two changelings in a linear fashion
        /// </summary>
        /// <param name="a"></param>
        /// <param name="c1"></param>
        /// <param name="b"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        static Changeling Combine(Changeling a, double c1, Changeling b, double c2)
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

        internal void Subscribe(double trigger, Action callback, bool haltingEvent = false)
        {
            subscriptions.Add(new Subscription(trigger, callback, haltingEvent));
        }

        /// <summary>
        /// Returns the moment at which this Changeling will reach the trigger value
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        internal abstract DateTime FindMomentAtValue(double trigger);

        public static Changeling operator +(Changeling a, Changeling b)
        {
            return Combine(a, 1, b, 1);
        }

        public static Changeling operator -(Changeling a, Changeling b)
        {
            return Combine(a, 1, b, -1);
        }

        public static Changeling operator +(Changeling a, double b)
        {
            if (a is Original)
                return new Combination.Linear(a as Original, 1, b);
            else if (a is Combination.Linear)
                return new Combination.Linear(a as Combination.Linear, 1, b);
            else if (a is Combination.NonLinear)
                throw new NotImplementedException();
            else
                throw new Exception("Not a valid sub class");
        }

        public static Changeling operator -(Changeling a, double b)
        {
            if (a is Original)
                return new Combination.Linear(a as Original, 1, -b);
            else if (a is Combination.Linear)
                return new Combination.Linear(a as Combination.Linear, 1, -b);
            else if (a is Combination.NonLinear)
                throw new NotImplementedException();
            else
                throw new Exception("Not a valid sub class");
        }

        public static Changeling operator *(Changeling a, double b)
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

        public static Changeling operator /(Changeling a, double b)
        {
            return a * (1 / b);
        }

        /// <summary>
        /// This is an original integrated. These are the only integrated the programm
        /// can create directly.
        /// </summary>
        class Original : Changeling
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
            /// List of all the combinations that are dependant on this Original
            /// </summary>
            List<WeakReference<Combination>> dependancies;

            public Original(double c, double d, DateTime m)
            {
                this.c = c;
                this.d = d;
                this.m = m;
                subscriptions = new List<Subscription>();
            }

            public override void Modify(double c, double d, DateTime m)
            {
                this.c = c;
                this.d = d;
                this.m = m;
                subscriptions.ForEach(sub => sub.Reprogram(this));
                dependancies.ForEach(dep => {
                    Combination comb;
                    bool stillAlive = dep.TryGetTarget(out comb);
                    if (stillAlive)
                        comb.Update();
                    else
                        dependancies.Remove(dep);
                });
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


            public void Reference(Combination combination)
            {
                dependancies.Add(new WeakReference<Combination>(combination));
            }
        }

        abstract class Combination : Changeling
        {

            internal void Update()
            {
                subscriptions.ForEach(sub => sub.Reprogram(this));
            }

            /// <summary>
            /// This class is a linear combination of Original integrated
            /// </summary>
            public class Linear : Combination
            {
                /// <summary>
                /// List with original changelings together with a value it must be multiplied with.
                /// </summary>
                public List<OriginalFraction> fractions;
                /// <summary>
                /// A constant value that gets added to all the original fractions.
                /// </summary>
                public double constant;

                /// <summary>
                /// Create a linear combination from a single original, a value to multiplie it with and a constant to add to it.
                /// </summary>
                /// <param name="a">The original changeling</param>
                /// <param name="b">The multiplier</param>
                /// <param name="constant"></param>
                public Linear(Original a, double b, double constant = 0)
                {
                    fractions = new List<OriginalFraction>() {
                        new OriginalFraction(a,b)
                    };
                    this.constant = constant;
                }

                /// <summary>
                /// Create a linear combination from an other linear combination, a value to multiplie it with and a constant to add to it.
                /// </summary>
                /// <param name="lc">The other linear combination</param>
                /// <param name="fraction">The multiplier</param>
                /// <param name="constant"></param>
                public Linear(Linear lc, double fraction = 1, double constant = 0)
                {
                    this.fractions = new List<OriginalFraction>();
                    lc.fractions.ForEach(of => fractions.Add(of.Copy()));
                    if (fraction != 1)
                        fractions.ForEach(of => of.value *= fraction);
                    this.constant = constant + lc.constant * fraction;
                }

                /// <summary>
                /// Create a linear combination from a list of originals.
                /// </summary>
                /// <param name="fractions"></param>
                public Linear(List<OriginalFraction> fractions)
                {
                    this.fractions = new List<OriginalFraction>(fractions);
                }

                /// <summary>
                /// Create a linear combination by combining two previous linear combinations.
                /// </summary>
                /// <param name="lc1">Changeling 1</param>
                /// <param name="lc2">Changeling 2</param>
                /// <param name="c1">multiplier 1</param>
                /// <param name="c2">multiplier 2</param>
                public Linear(Linear lc1, Linear lc2, double c1 = 1, double c2 = 1)
                {
                    this.fractions = new List<OriginalFraction>();
                    lc1.fractions.ForEach(of => fractions.Add(of.Copy()));
                    if (c1 != 1)
                        fractions.ForEach(of => of.value *= c1);
                    lc2.fractions.ForEach(ofb => AddOriginal(ofb.original, ofb.value));
                }

                /// <summary>
                /// Add an original to this linear combination
                /// </summary>
                /// <param name="o"></param>
                /// <param name="fraction"></param>
                public void AddOriginal(Original o, double fraction)
                {
                    if (fractions.Any(of => of.original == o))
                        fractions.Find(of => of.original == o).value += fraction;
                    else
                        fractions.Add(new OriginalFraction(o, fraction));
                }

                /// <summary>
                /// Calculate the value at a given moment.
                /// </summary>
                /// <param name="m2"></param>
                /// <returns></returns>
                public override double Value(DateTime m2)
                {
                    double ans = 0;
                    fractions.ForEach(of => ans += of.original.Value(m2) * of.value);
                    return ans;
                }

                /// <summary>
                /// Find the moment at which a value gets triggered.
                /// </summary>
                /// <param name="trigger"></param>
                /// <returns></returns>
                internal override DateTime FindMomentAtValue(double trigger)
                {
                    DateTime mostRecent = fractions.Max(f => f.original.m);
                    double cSum = constant; double dSum = 0;
                    foreach(OriginalFraction fr in fractions)
                    {
                        cSum += fr.original.Value(mostRecent) * fr.value;
                        dSum += fr.original.d * fr.value;
                    }
                    double seconds = (trigger - cSum) / dSum;
                    return mostRecent + TimeSpan.FromSeconds(seconds);
                }

                public override void Modify(double c, double d, DateTime m)
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// The combination of an original and a multiplier.
                /// </summary>
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

                public override void Modify(double c, double d, DateTime m)
                {
                    throw new NotImplementedException();
                }
            }
        }

        class Subscription
        {
            double trigger;
            Action callback;
            bool haltingEvent;
            Event evnt;

            public Subscription(double trigger, Action callback, bool haltingEvent)
            {
                this.trigger = trigger;
                this.callback = callback;
                this.haltingEvent = haltingEvent;
            }

            public void Reprogram(Changeling changeling)
            {
                DateTime date = changeling.FindMomentAtValue(trigger);
                bool isFuture = date.CompareTo(God.Time) > 0;
                if(evnt == null && isFuture)
                {
                    evnt = new Event(date, callback, haltingEvent);
                }else if(evnt != null && isFuture)
                {
                    if(evnt.moment.CompareTo(date) != 0)
                    {
                        evnt.Delete();
                        evnt = new Event(date, callback, haltingEvent);
                    }
                }
                if(evnt != null && !isFuture)
                {
                    evnt.Delete();
                    evnt = null;
                }
            }
        }
    }
}
