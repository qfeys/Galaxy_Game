using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Giant : Orbital
    {
        public Giant(Orbital parent, double mass, OrbitalElements elements) : base(parent, mass, elements)
        {
        }

        public override void Generate(double mass, Random rand)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Giant-" + (Mass / JUPITER_MASS).ToString("#0.00");
        }

        public const double JUPITER_MASS = 1.899e27;
        public const ulong JUPITER_SMA = StarSystem.AU * 52 / 10;
    }
}
