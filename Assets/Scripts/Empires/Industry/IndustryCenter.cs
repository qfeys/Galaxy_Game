using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Empires.Industry
{
    class IndustryCenter
    {
        public readonly Stockpile stockpile = new Stockpile();

        public Dictionary<Installation, int> installations { get; private set; }
    }
}
