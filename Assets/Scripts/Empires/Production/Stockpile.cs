using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Production
{
    class Stockpile
    {
        public double iron;
        public double nonFerrous;         // think copper, concrete
        public double carbon;      // think petrochemicals
        public double silicates;
        public double rareEarth;

        public double water;            // potable water
        public double food;

        public double components;
        public double electronics;
        public double consumerGoods;
    }
}
