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

        public class ResourceType
        {
            static List<string> ResourceTypes = new List<string> { "iron", "nonFerrous", "carbon", "silicates", "rareEarth",
            "water", "food", "components", "electronics", "consumerGoods" };

            string type;

            public ResourceType(string name)
            {
                if (ResourceTypes.Contains(name) == false)
                    throw new ArgumentException("The resource type: " + name + " is not valid.");
                type = name;
            }

            public override string ToString()
            {
                return type;
            }
        }

    }
}
