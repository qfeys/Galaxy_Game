using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C5;

namespace Assets.Scripts.Simulation
{
    static class EventSchedule
    {
        static TreeSet<Event> events;

        public static DateTime nextEvent { get { return events.FindMin().date; } }

        public static void Init()
        {
            events = new TreeSet<Event>();
        }

        public static void Add(Event newEvent)
        {
            events.Add(newEvent);
        }
    }
}
