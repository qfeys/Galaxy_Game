using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Planetoid : Orbital
    {

        long dayLength;
        public double SurfaceGravity { get { return Math.Pow(Mass / Rock.EARTH_MASS, 1 / 3); } }

        public Planetoid(Orbital parent, double mass, OrbitalElements elements, long dayLength) : base(parent, mass, elements)
        {
            this.dayLength = dayLength;
        }

        public override void Generate(double mass, Random rand)
        {
            throw new NotImplementedException();
        }
    }
}
