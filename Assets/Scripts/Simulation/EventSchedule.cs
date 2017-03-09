using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C5;

namespace Assets.Scripts.Simulation
{
    static class EventSchedule
    {
        static TreeSet<Event> events = new TreeSet<Event>();

        public static DateTime nextEvent { get { return events.FindMin().date; } }
        

        public static void Add(Event newEvent)
        {
            events.Add(newEvent);
        }
    }
}
