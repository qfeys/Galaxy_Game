using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Core : Orbital
    {
        public Core() : base(null, 0, new OrbitalElements())
        {

        }

        public override void Generate(ulong mass, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
