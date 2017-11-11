using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Simulation
{
    class Event
    {
        public DateTime moment { get; private set; }
        Action callback;
        bool haltingEvent;

        public Event(DateTime moment, Action callback, bool haltingEvent = false)
        {
            this.moment = moment;
            this.callback = callback;
            this.haltingEvent = haltingEvent;
        }

        public void Fire()
        {
            callback();
        }
    }
}
