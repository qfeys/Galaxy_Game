﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires
{
    class Modifier
    {
        public readonly Name name;
        public readonly double value;

        public Modifier(Name name, double value)
        {
            this.name = name;
            this.value = value;
        }

        public Modifier(string name, double value)
        {
            this.name = (Name)Enum.Parse(typeof(Name), name);
            this.value = value;
        }

        public enum Name {income,
            add_industry_capacity,
            add_construction_capacity,
            add_production_capacity,
            add_mining_capacity,
            add_wealth
        }
    }
}
