using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class StarSystem : Orbital
    {
        public StarSystem(Orbital parent, long mass, OrbitalElements elements) : base(parent, 0, elements)
        {
        }

        public override void Generate(System.UInt64 mass, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
