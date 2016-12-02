using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Empire
    {

        List<Population> populations;
        List<Leaders.Leader> leaders;
        Production.Wtc wtc;
        Technology.Academy academy;
        List<Assets.Asset> freeAssets;  // in contrast to assets bound to populations
        List<Mobiles.Mobile> mobiles; 

        public Empire()
        {
            populations = new List<Population>();
            leaders = new List<Leaders.Leader>();
            wtc = new Production.Wtc();
            academy = new Technology.Academy();
            freeAssets = new List<Assets.Asset>();
            mobiles = new List<Mobiles.Mobile>();
        }

    }
}
