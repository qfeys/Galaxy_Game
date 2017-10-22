using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Production
{
    class Stockpile
    {
        public Dictionary<ResourceType, double> pile { get; private set; }

        public Stockpile(int init = 0)
        {
            pile = new Dictionary<ResourceType, double>();
            ResourceType.ResourceTypes.ForEach(r => pile.Add(r, init));
        }

        public void Add(string resource, double amount)
        {
            ResourceType r = ResourceType.Get(resource);
            pile[r] += amount;
        }

        public void Add(ResourceType resource, double amount)
        {
            pile[resource] += amount;
        }

        public class ResourceType
        {
            static public readonly List<ResourceType> ResourceTypes = new List<ResourceType> {
                new ResourceType("steel",true),
                new ResourceType("nonFerrous",true),
                new ResourceType("carbon",true),
                new ResourceType("silicates",true),
                new ResourceType("rareEarth",true),
                new ResourceType("water", true),
                new ResourceType("food", false),
                new ResourceType("components", false),
                new ResourceType("electronics", false),
                new ResourceType("consumerGoods", false)
            };

            public readonly string type;
            public readonly bool minable;

            ResourceType(string type, bool minable)
            {
                this.type = type; this.minable = minable;
            }

            public static ResourceType Get(string name)
            {
                if (ResourceTypes.Any(rt=>rt.type == name) == false)
                    throw new ArgumentException("The resource type: " + name + " is not valid.");
                return ResourceTypes.Find(rt => rt.type == name);
            }

            public override string ToString()
            {
                return "r_" + type;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;
                return type.Equals(((ResourceType)obj).type);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                return type.GetHashCode();
            }
        }

    }
}
