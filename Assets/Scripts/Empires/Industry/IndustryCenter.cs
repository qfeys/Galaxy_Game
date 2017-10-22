using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Empires.Industry
{
    class IndustryCenter
    {
        public readonly Industry.Stockpile stockpile = new Industry.Stockpile();

        public Dictionary<Industry.Installation, int> installations { get; private set; }
    }
}
