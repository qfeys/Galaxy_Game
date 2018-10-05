﻿using System;
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
            // Ticks to seconds is devide by 10 milion; one tick is 100 ns
            return new Combination.NonLinear("T " + m.Ticks/10e6 + " Diff {} mult " + c + " sum");
        }

        abstract public void Modify(double c, double d, DateTime m);
        abstract public void Modify(Changeling ch);

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
                    return new Combination.NonLinear("{1} " + c1 + " MULT {2} " + c2 + "MULT SUM", a, b);
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
                    return new Combination.NonLinear("{1} " + c1 + " MULT {2} " + c2 + "MULT SUM", a, b);
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

        abstract protected void Register(Combination combination);

        /// <summary>
        /// Returns the moment at which this Changeling will reach the trigger value
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        internal abstract DateTime FindMomentAtValue(double trigger);

        #region Operators

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
                return new Combination.NonLinear("{} " + b + " SUM", a);
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
                return new Combination.NonLinear("{} " + b + " DIFF", a);
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
                return new Combination.NonLinear("{} " + b + " MULT", a);
            else
                throw new Exception("Not a valid sub class");
        }

        public static Changeling operator /(Changeling a, double b)
        {
            return a * (1 / b);
        }

        public static implicit operator double(Changeling ch)
        {
            return ch.Value();
        }

        #endregion

        /// <summary>
        /// This is an original integrated. These are the only integrated the programm
        /// can create directly.
        /// </summary>
        protected class Original : Changeling
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
                Update();
            }

            public override void Modify(Changeling ch)
            {
                throw new Exception("You can't modify original changelings into a different changeling. Try changing the base parameters instead.");
            }

            /// <summary>
            /// Reprogram all subscriptions and subscriptions of dependancies in the event schedule.
            /// </summary>
            internal void Update()
            {
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

            /// <summary>
            /// Registers the combination to this Original so that when this original is modefied, the combination is updated.
            /// </summary>
            /// <param name="combination"></param>
            override protected void Register(Combination combination)
            {
                if (dependancies.Any(wr => { Combination comb; bool exist = wr.TryGetTarget(out comb); return exist && combination == comb; }))
                    return;     // This combination is already registered
                dependancies.Add(new WeakReference<Combination>(combination));
            }
        }



        abstract protected class Combination : Changeling
        {

            /// <summary>
            /// This class is a linear combination of Original Changelings
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
                    a.Register(this);
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
                    lc.Register(this);
                }

                /// <summary>
                /// Create a linear combination from a list of originals.
                /// </summary>
                /// <param name="fractions"></param>
                public Linear(List<OriginalFraction> fractions)
                {
                    this.fractions = new List<OriginalFraction>(fractions);
                    Register(this);
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
                    Register(this);
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
                    {
                        fractions.Add(new OriginalFraction(o, fraction));
                        o.Register(this);
                    }
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
                    foreach (OriginalFraction fr in fractions)
                    {
                        cSum += fr.original.Value(mostRecent) * fr.value;
                        dSum += fr.original.d * fr.value;
                    }
                    double seconds = (trigger - cSum) / dSum;
                    return mostRecent + TimeSpan.FromSeconds(seconds);
                }

                public override void Modify(double c, double d, DateTime m)
                {
                    throw new Exception("You can't modify combination changelings with base parameters. Try changing with a different changeling instead.");
                }

                public override void Modify(Changeling ch)
                {
                    if (ch is Original)
                    {
                        fractions = new List<OriginalFraction>();
                        constant = 0;
                        fractions.Add(new OriginalFraction(ch as Original, 1));
                    } else if (ch is Combination.Linear)
                    {
                        fractions = new List<OriginalFraction>((ch as Linear).fractions);
                        constant = (ch as Linear).constant;
                    } else if (ch is Combination.NonLinear)
                        throw new Exception("Linear becomes non-linear. Implement if necessary.");
                    else
                        throw new Exception("Not a valid sub class");
                    Update();
                }

                /// <summary>
                /// Registers the given combination to to all these combinations Originals.
                /// </summary>
                /// <param name="combination"></param>
                override protected void Register(Combination combination)
                {
                    fractions.ForEach(fr => fr.original.Register(combination));
                }
            }

            /// <summary>
            /// This class is a polynomial combination of Original Changelings
            /// </summary>
            public class NonLinear : Combination
            {
                // Contains the operations necessary to calculate the NL-combination
                List<RPN_Token> reversePolishStack;

                /// <summary>
                /// Create the Changeling by typing it's fromula in reverse polish notation using either spaces or commas as seperators
                /// Use other changelings by using "{i}" and adding that changeling to the parameters
                /// Use the time by typing T
                /// Example: "45.3 {1} T DIFF {2} MULT SUM"
                /// Available operations are SUM, DIFF, MULT, DIV, POW
                /// </summary>
                public NonLinear(string reversePolishString, params Changeling[] parameters)
                {
                    reversePolishStack = RPN_Token.CreateStack(reversePolishString, parameters);
                    foreach (Changeling changeling in parameters)
                    {
                        changeling.Register(this);
                    }
                }
                
                public override double Value(DateTime moment)
                {
                    return RPN_Token.EvaluateStack(reversePolishStack, moment);
                }

                TimeSpan[] evaluationMoments = new TimeSpan[10]{TimeSpan.FromSeconds(5),TimeSpan.FromMinutes(1),TimeSpan.FromMinutes(30),TimeSpan.FromHours(2),
                                                                TimeSpan.FromHours(12),TimeSpan.FromDays(2),TimeSpan.FromDays(7),TimeSpan.FromDays(30),
                                                                TimeSpan.FromDays(200), TimeSpan.FromDays(2000) };

                internal override DateTime FindMomentAtValue(double trigger)
                {
                    DateTime now = God.Time;
                    bool startBelow = Value(now) < trigger;
                    for (int i = 0; i < evaluationMoments.Length; i++)
                    {
                        bool currentBelow = Value(now + evaluationMoments[i]) < trigger;
                        if (startBelow != currentBelow)
                            if (i == 0)
                                goto final;
                            else
                            {
                                now += evaluationMoments[i - 1];
                                i = -1;
                            }
                    }
                    return DateTime.MinValue;
                    final:
                    double valueDiff = Value(now + evaluationMoments[0]) - Value(now);
                    TimeSpan momentDiff = evaluationMoments[0];
                    double fraction = (trigger - Value(now)) / valueDiff;
                    return now + TimeSpan.FromSeconds(momentDiff.TotalSeconds * fraction);
                }

                public override void Modify(double c, double d, DateTime m)
                {
                    throw new Exception("You can't modify combination changelings with base parameters. Try changing with a different changeling instead.");
                }

                public override void Modify(Changeling ch)
                {
                    if (ch is Original)
                    {
                        reversePolishStack = RPN_Token.CreateStack("{}", ch);
                    } else if (ch is Linear)
                    {
                        reversePolishStack = RPN_Token.CreateStack("{}", ch);
                    } else if (ch is NonLinear)
                        reversePolishStack = new List<RPN_Token>((ch as NonLinear).reversePolishStack);
                    else
                        throw new Exception("Not a valid sub class");
                    Update();
                }

                /// <summary>
                /// Registers the combination to to all these combinations Originals.
                /// </summary>
                /// <param name="combination"></param>
                override protected void Register(Combination combination)
                {
                    reversePolishStack.ForEach(token => { if (token.token_Type == RPN_Token.Token_Type.CHANGELING) token.changeling.Register(combination); });
                }

                struct RPN_Token
                {
                    public enum Token_Type { NUMBER, CHANGELING, TIME, OPERATION}
                    public enum Operation { SUM, DIFF, MULT, DIV, POW}

                    public Token_Type token_Type;
                    public double number;
                    public Changeling changeling;
                    public Operation operation;

                    /// <summary>
                    /// Create the Changeling by typing it's fromula in reverse polish notation using either spaces or commas as seperators
                    /// Use other changelings by using "{i}" and adding that changeling to the parameters
                    /// Use the time by typing T
                    /// Example: "45.3 {1} T DIFF {2} MULT SUM"
                    /// Available operations are SUM, DIFF, MULT, DIV, POW
                    /// </summary>
                    /// <param name="literal"></param>
                    /// <param name="parameters"></param>
                    /// <returns></returns>
                    static public List<RPN_Token> CreateStack(string literal, params Changeling[] parameters)
                    {
                        var list = new List<RPN_Token>();
                        literal.ToUpper();
                        string[] op_array = literal.Split(' ',',');
                        double numberResult;
                        Operation operationResult;
                        for (int i = 0; i < op_array.Length; i++)
                        {
                            string token = op_array[i];
                            if (token == "T")
                                list.Add(new RPN_Token() { token_Type = Token_Type.TIME });
                            else if (token[0] == '{')
                            {
                                if (token[token.Length - 1] == '}')
                                {
                                    if (token.Length == 2)
                                        if (parameters.Length == 1)
                                            list.Add(new RPN_Token() { token_Type = Token_Type.CHANGELING, changeling = parameters[0] });
                                        else
                                            throw new Exception("You used a numberless parameter, but there are not exactly 1 parameters. Token: " + token);
                                    else
                                    {
                                        int index = int.Parse(token.Substring(1, token.Length - 2));
                                        list.Add(new RPN_Token() { token_Type = Token_Type.CHANGELING, changeling = parameters[index] });
                                    }
                                } else throw new Exception("Bad token. Token: " + token);
                            } else if (double.TryParse(token, out numberResult))
                            {
                                list.Add(new RPN_Token() { token_Type = Token_Type.NUMBER, number = numberResult });
                            } else if (Enum.TryParse(token, out operationResult))
                            {
                                list.Add(new RPN_Token() { token_Type = Token_Type.OPERATION, operation = operationResult });
                            } else
                                throw new Exception("Invalid token. Token: " + token);
                        }
                        return list;
                    }

                    /// <summary>
                    /// We evaluate backwards, as it is faster. See wikipedia on reverse polish notation
                    /// </summary>
                    /// <param name="list"></param>
                    /// <returns></returns>
                    static public double EvaluateStack(List<RPN_Token> list, DateTime moment)
                    {
                        Stack<RPN_Token> stack = new Stack<RPN_Token>();
                        int length = list.Count;
                        stack.Push(list[length-1]);
                        bool lastIsNumber = false;
                        for (int i = length-2; i >= 0; i--)
                        {
                            RPN_Token current = list[i];
                            stack.Push(current);
                            bool currentIsNumber = current.token_Type == Token_Type.CHANGELING || current.token_Type == Token_Type.NUMBER;
                            if (currentIsNumber && lastIsNumber)
                            {
                                EvaluateSingleOperation(moment, stack);
                            } else
                            {
                            }
                            lastIsNumber = currentIsNumber;
                        }
                        while(stack.Count > 1)
                        {
                            EvaluateSingleOperation(moment, stack);
                        }
                        return stack.Pop().number;
                    }

                    private static void EvaluateSingleOperation(DateTime moment, Stack<RPN_Token> stack)
                    {
                        RPN_Token current = stack.Pop();
                        double currentNum = current.token_Type == Token_Type.NUMBER ? current.number : current.changeling.Value(moment);
                        RPN_Token last = stack.Pop();
                        double lastNum = last.token_Type == Token_Type.NUMBER ? last.number : last.changeling.Value(moment);
                        RPN_Token operation = stack.Pop();
                        switch (operation.operation)
                        {
                        case Operation.SUM: stack.Push(new RPN_Token() { token_Type = Token_Type.NUMBER, number = currentNum + lastNum }); break;
                        case Operation.DIFF: stack.Push(new RPN_Token() { token_Type = Token_Type.NUMBER, number = currentNum - lastNum }); break;
                        case Operation.MULT: stack.Push(new RPN_Token() { token_Type = Token_Type.NUMBER, number = currentNum * lastNum }); break;
                        case Operation.DIV: stack.Push(new RPN_Token() { token_Type = Token_Type.NUMBER, number = currentNum / lastNum }); break;
                        case Operation.POW: stack.Push(new RPN_Token() { token_Type = Token_Type.NUMBER, number = Math.Pow(currentNum, lastNum) }); break;
                        default: throw new Exception("Operation not implemented?");
                        }
                    }
                }
            }
            
            /// <summary>
            /// Reprogram all subscriptions and subscriptions of dependancies in the event schedule.
            /// </summary>
            internal void Update()
            {
                subscriptions.ForEach(sub => sub.Reprogram(this));
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
