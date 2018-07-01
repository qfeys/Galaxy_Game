using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Simulation
{
    class Event
    {
        virtual public DateTime moment { get; private set; }
        Action callback;
        bool haltingEvent;

        public Event(DateTime moment, Action callback, bool haltingEvent = false)
        {
            this.moment = moment;
            this.callback = callback;
            this.haltingEvent = haltingEvent;
            Schedule.Add(this);
        }

        protected Event() { }

        public void Fire()
        {
            callback();
        }

        public void Delete()
        {
            Schedule.Remove(this);
        }
    }
}
