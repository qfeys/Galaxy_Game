using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Simulation
{
    static class EventSchedule
    {
        //static SortedList<Event> events = new SortedList<Event>();
        static SortedList<IUpdatable> events =
            new SortedList<IUpdatable>(Comparer<IUpdatable>.Create((x, y) => x.NextMandatoryUpdate.CompareTo(y.NextMandatoryUpdate)));

        public static DateTime NextEvent
        {
            get
            {
                if (events.Count != 0) return events.FindFirst().NextMandatoryUpdate;
                else return DateTime.MaxValue;
            }
        }
        

        public static void Add(IUpdatable newUpdatable)
        {
            events.Add(newUpdatable);
        }

        /// <summary>
        /// Signals wether the simulation can progress untill the given date. If not, the latest allowed date is outputted.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        internal static bool CanProgress(DateTime date, out DateTime maxDate)
        {
            int i = 0;
            while (events.ElementAt(i).NextMandatoryUpdate <= date)
            {
                if (events.ElementAt(i).NextUpdateHasPriority)
                {
                    maxDate = events.ElementAt(i).NextMandatoryUpdate;
                    return false;
                }
                i++;
                if (events.Count >= i)
                    break;
            }
            maxDate = date;
            return true;
        }

        /// <summary>
        /// Processes all the events up untill date. The amount of events handeled is returned
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static int Progress(DateTime date)
        {
            int i = 0;  // i will return the amount of events handeled
            while (events.Count != 0 && events.FindFirst().NextMandatoryUpdate <= date)
            {
                var d = events.FindFirst().NextMandatoryUpdate;
                events.TakeFirst().Update(d);
                i++;
            }
            return i;
        }
    }
}
