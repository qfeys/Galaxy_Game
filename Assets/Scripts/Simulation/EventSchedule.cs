using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Simulation
{
    static class EventSchedule
    {
        static SortedList<Event> events = new SortedList<Event>();

        public static DateTime NextEvent { get
            {
                if (events.Count != 0) return events.FindFirst().date;
                else return DateTime.MaxValue;
            } }
        

        public static void Add(Event newEvent)
        {
            events.Add(newEvent);
        }

        internal static DateTime ProcessNextEvent(out bool hardInterrupt)
        {
            Event ev = events.TakeFirst();
            ev.effect();
            if (ev.interrupt == Event.Interrupt.hard)
                hardInterrupt = true;
            else
                hardInterrupt = false;
            return ev.date;
        }
    }
}
