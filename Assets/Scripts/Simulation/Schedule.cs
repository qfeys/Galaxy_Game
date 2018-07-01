using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Simulation
{
    static class Schedule
    {
        //static SortedList<Event> events = new SortedList<Event>();
        static SortedList<Event> events =
            new SortedList<Event>(Comparer<Event>.Create((x, y) => x.moment.CompareTo(y.moment)));

        public static DateTime NextEvent
        {
            get
            {
                if (events.Count != 0) return events.PeekFirst().moment;
                else return DateTime.MaxValue;
            }
        }

        public static void Add(Event evnt)
        {
            events.Add(evnt);
        }

        public static void Remove(Event evnt)
        {
            events.Remove(evnt);
        }

        /// <summary>
        /// Processes all the events up untill date. The amount of events handeled is returned
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static int Progress(DateTime date)
        {
            int i = 0;  // i will return the amount of events handeled
            while (events.Count != 0 && events.PeekFirst().moment <= date)
            {
                events.TakeFirst().Fire();
                i++;
                if (i > 100) {
                    God.Log("Exxesive (100+) amount of events in one Progress tick");
                    break;
                }
            }
            return i;
        }
    }
}
