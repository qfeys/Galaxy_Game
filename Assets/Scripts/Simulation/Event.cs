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

        /// <summary>
        /// Try to scedule an event mith a mean time to happen. The event will automatically be sceduled.
        /// </summary>
        /// <param name="mtth">Mean Time to Happen</param>
        /// <param name="interval">The maximum time interval for wich this event might be sceduled.</param>
        /// <param name="interrupt">The priority level of the event</param>
        /// <param name="effect">The event effect</param>
        /// <returns>The event also gets returned.</returns>
        public static Event Try(TimeSpan mtth,TimeSpan interval, Interrupt interrupt, Action effect)
        {
            TimeSpan next = RNG.nextOccurence(mtth);
            if(next < interval)
            {
                Event e = new Event(God.Time + next, interrupt, effect);
                EventSchedule.Add(e);
                return e;
            }
            return null;
        }


        public int CompareTo(Event other)
        {
            return date.CompareTo(other.date);
        }
    }
}
