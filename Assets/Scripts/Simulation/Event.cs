using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Simulation
{
    class Event : IComparable<Event>
    {

        public readonly DateTime date;
        public enum Interrupt { none, soft, hard }
        public readonly Interrupt interrupt;
        public readonly Action effect;

        public Event(DateTime date, Interrupt interrupt, Action effect)
        {
            this.date = date; this.interrupt = interrupt; this.effect = effect;
            EventSchedule.Add(this);
        }

        public static Event Try(TimeSpan mtth,TimeSpan interval, Interrupt interrupt, Action effect)
        {
            TimeSpan next = RNG.nextOccurence(mtth);
            if(next < interval)
            {
                return new Event(God.Time + next, interrupt, effect);
            }
            return null;
        }


        public int CompareTo(Event other)
        {
            return date.CompareTo(other.date);
        }
    }
}
