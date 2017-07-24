using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Production
{
    class Stockpile
    {

        public class ResourceType
        {
            static List<string> ResourceTypes = new List<string> {
                "iron",
                "nonFerrous",
                "carbon",
                "silicates",
                "rareEarth",
                "water",
                "food",
                "components",
                "electronics",
                "consumerGoods"
            };

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
