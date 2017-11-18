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

        public class Conditional : Event
        {
            /// <summary>
            /// The number that will be used to compare against
            /// </summary>
            Integrated condition;
            /// <summary>
            /// The number the condition must reach to trigger this event
            /// </summary>
            double trigger;

            Action ValueChangeCallback;

            public override DateTime moment
            {
                get { return condition.FindMomentAtValue(trigger); }
            }

            public Conditional(Integrated condition, double trigger, Action callback, Action ValueChangeCallback, bool haltingEvent = false)
            {
                this.condition = condition;
                this.trigger = trigger;
                this.callback = callback;
                this.ValueChangeCallback = ValueChangeCallback;
                this.haltingEvent = haltingEvent;
                condition.Subscribe(this);
                Schedule.Add(this);
            }

            public void ValueChanged()
            {
                ValueChangeCallback();
            }
        }
    }
}
