using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Simulation
{
    interface IUpdatable
    {
        DateTime LastUpdate { get; }
        DateTime NextMandatoryUpdate { get; }
        bool NextUpdateHasPriority { get; }

        void Update(DateTime date);
    }
}
