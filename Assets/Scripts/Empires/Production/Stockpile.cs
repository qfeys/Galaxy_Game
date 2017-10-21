﻿using System;
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
            ResourceType r = new ResourceType(resource);
            pile[r] += amount;
        }

        public void Add(ResourceType resource, double amount)
        {
            pile[resource] += amount;
        }

        public class ResourceType
        {
            static List<string> resourceTypes = new List<string> {
                "steel",
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

            public static List<ResourceType> ResourceTypes { get { return resourceTypes.ConvertAll(r=> new ResourceType(r)); } }

            public ResourceType(string name)
            {
                if (resourceTypes.Contains(name) == false)
                    throw new ArgumentException("The resource type: " + name + " is not valid.");
                type = name;
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
