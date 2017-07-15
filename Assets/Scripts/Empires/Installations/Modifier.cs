using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Empires.Assets
{
    class Modifier
    {
        public readonly Name name;
        public readonly double number;
        
        public Modifier(Name name, double number)
        {
            this.name = name;
            this.number = number;
        }

        public enum Name { }
    }
}
