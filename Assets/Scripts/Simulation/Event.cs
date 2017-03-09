using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Simulation
{
    class Event : IComparable<Event>
    {

        public readonly DateTime date;
        public readonly Action effect;
        public enum Interrupt { none, soft, hard }

        public int CompareTo(Event other)
        {
            return date.CompareTo(other.date);
        }
    }
}
