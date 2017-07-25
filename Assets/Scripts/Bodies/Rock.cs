﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Rock : Orbital
    {

        long dayLength;
        double atmosphere;    // Atmospheric pressure at 'sea' level
        bool breathable;
        public double SurfaceGravity { get { return Math.Pow(Mass / EARTH_MASS, 1 / 3); } }

        public Rock(Orbital parent, double mass, OrbitalElements elements, long dayLength, double atmosphere, bool breathable) : base(parent, mass, elements)
        {
            this.dayLength = dayLength; this.atmosphere = atmosphere; this.breathable = breathable;
        }

        public override void Generate(double mass, Random rand)
        {
            throw new NotImplementedException();
        }

        public bool Habitable { get { return breathable; } }

        public const double EARTH_MASS = 5.972e24;
    }
}
