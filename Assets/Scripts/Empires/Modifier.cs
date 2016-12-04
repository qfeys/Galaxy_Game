using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Modifier
    {
        internal enum PopulationModifier { localBuildTime, wealth, }
        internal enum GlobalModifier { globalBuildTime }
        internal enum AssetModifier { upkeepReduction}
        internal enum MobileModifier { increasedAcceleration}
    }
}
