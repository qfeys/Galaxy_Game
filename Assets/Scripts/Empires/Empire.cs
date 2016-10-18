using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Empire
    {

        List<Population> populaitons;
        List<Leaders.Leader> leaders;
        Production.Wtc wtc;
        Technology.Academy academy;
        List<Assets.Asset> freeAssets;  // in contrast to assets bound to populations
        List<Mobiles.Mobile> mobiles; 

        public Empire()
        {
            populaitons = new List<Population>();
            leaders = new List<Leaders.Leader>();
            wtc = new Production.Wtc();
            academy = new Technology.Academy();
            freeAssets = new List<Assets.Asset>();
            mobiles = new List<Mobiles.Mobile>();
        }

    }
}
