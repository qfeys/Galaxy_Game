using System;
using System.Collections.Generic;
using Assets.Scripts.Empires;

namespace Assets.Scripts.Bodies
{
    internal class Planet
    {
        Star parent;
        Planet parentPlanet;
        public Type type { get; private set; }
        public OrbitalElements OrbElements { get; private set; }
        /// <summary>
        /// Radius of the planet in km
        /// </summary>
        public int Radius { get; private set; }
        /// <summary>
        /// The density relative to Earth
        /// </summary>
        public double Density { get; private set; }
        /// <summary>
        /// The mass relative to Earth
        /// </summary>
        public double Mass { get; private set; }
        /// <summary>
        /// The survace gravity relative to Earth
        /// </summary>
        public double SurfaceGravity { get { return Mass / Math.Pow(Radius / 6380, 2); } }
        /// <summary>
        /// The escape velocity relative to Earth
        /// </summary>
        public double EscapeVelocity { get { return Math.Sqrt(19600 * SurfaceGravity * Radius) / 11200; } }
        /// <summary>
        /// The rotational period in hours
        /// </summary>
        public double RotationalPeriod { get; private set; }
        /// <summary>
        /// The axial tilt in degrees
        /// </summary>
        public double AxialTilt { get; private set; }
        /// <summary>
        /// The solar day in hours
        /// </summary>
        public double SolarDay { get { return 1 / (1 / RotationalPeriod + 1 / OrbElements.T.TotalHours * AxialTilt < 90 ? -1 : +1); } }
        bool innerPlanet;
        bool isMoon;
        bool isCaptured = false;
        bool isTidallyLocked;
        Tuple<Planet, Planet> doubleChilderen;

        public RNG rng { get { return parent.starSystem.rng; } }

        public Planet(Star parent, double meanDistance, bool InnerPlanet)
        {
            this.parent = parent;
            innerPlanet = InnerPlanet;
            type = RollType();
            // Calculate orbital elements
            double eccentricity = 0;
            int a = rng.D10 + type == Type.Captured ? +3 : 0;
            if (a <= 5) eccentricity = rng.D10 * 0.005;
            else if (a <= 7) eccentricity = rng.D10 * 0.01 + 0.05;
            else if (a <= 9) eccentricity = rng.D10 * 0.01 + 0.15;
            else eccentricity = rng.D10 * 0.04 + 0.25;
            double inclination = 0;         // Inclination is not in the paper, except for a small mention on page 16
            int b = rng.D10 + type == Type.Captured ? +2 : 0;
            if (b <= 3) inclination = rng.D10 * 0.3 * Math.PI / 180;
            else if (b <= 6) inclination = rng.D10 * -0.3 * Math.PI / 180;
            else if (b <= 8) inclination = (rng.D10 * 0.6 + 0.3) * Math.PI / 180;
            else inclination = (rng.D10 * -0.6 - 0.3) * Math.PI / 180;

            OrbElements = new OrbitalElements(rng.Circle, inclination, rng.Circle, rng.Circle, meanDistance, eccentricity, parent.Mass);

            CalculateSize();
        }

        Planet(Star parent, bool innerPlanet, Type type, OrbitalElements orbitalElements)
        {
            this.parent = parent;
            this.innerPlanet = innerPlanet;
            this.type = type;
            OrbElements = orbitalElements;
        }

        void CalculateSize()
        {
            if (type == Type.Chunk || type == Type.Terrestial_planet || type == Type.Gas_Giant)
            {
                int a = rng.D10;
                if (type == Type.Chunk) Radius = 200 * a;
                else if (type == Type.Terrestial_planet)
                    if (a == 1) Radius = 2000 * rng.D10 * 100;
                    else if (a + parent.starSystem.Abundance <= 2 ) Radius = 2000 * rng.D10 * 100;
                    else if (a + parent.starSystem.Abundance <= 4) Radius = 3000 * rng.D10 * 100;
                    else if (a + parent.starSystem.Abundance <= 8) Radius = (a - 1) * 1000 + rng.D10 * 100;
                    else if (a + parent.starSystem.Abundance <= 9) Radius = 8000 + rng.D10 * 200;
                    else Radius = 10000 + rng.D10 * 500;
                else if (type == Type.Gas_Giant)
                    if (a <= 5) Radius = (a + 4) * 3000 + rng.D10 * 300;
                    else Radius = (a - 3) * 10000 + rng.D10 * 1000;

                int b = rng.D10 + parent.starSystem.Abundance;
                if (b < 1) b = 1; else if (b > 11) b = 11;
                if (innerPlanet)
                    if (type == Type.Chunk || type == Type.Terrestial_planet) Density = 0.3 + b * 0.127 / Math.Pow(0.4 + OrbElements.SMA / Math.Sqrt(parent.Luminosity), 0.67);
                    else if (type == Type.Gas_Giant) Density = 0.1 + b * 0.025;
                    else
                        if (type == Type.Chunk || type == Type.Terrestial_planet) Density = 0.1 + b * 0.05;
                    else if (type == Type.Gas_Giant) Density = 0.08 + b * 0.025;
                if (Density > 1.5) Density = 1.5;

                Mass = Math.Pow(Radius / 6380, 3) * Density;
            }else if(type == Type.Superjovian)
            {
                int a = rng.D10;
                if (a <= 4) Mass = 500 * rng.D10 * 50;
                else if (a <= 7) Mass = 1000 + rng.D10 * 100;
                else if (a <= 9) Mass = 2000 + rng.D10 * 100;
                else Mass = 3000 + rng.D10 * 100;

                Radius = (int)(60000 + (rng.D10 - parent.starSystem.Age/2)*2000);
                Density = Mass * Math.Pow(6380 / Radius, 3);
            }
        }

        public void CalculateDay()
        {
            double tidalForce = parent.Mass * 26640000 / Math.Pow(OrbElements.SMA * 400, 3);
            isTidallyLocked = (0.83 + rng.D10 * 0.03) * tidalForce * parent.starSystem.Age / 6.6 > 1;
            if (isTidallyLocked == false)
            {
                // Rotational period (table 2.2.2)
                int a = rng.D10 + (int)(tidalForce * parent.starSystem.Age);
                if (type == Type.Chunk)
                    if (a <= 5) RotationalPeriod = rng.D10 * 2;
                    else if (a <= 7) RotationalPeriod = rng.D10 * 24;
                    else if (a <= 9) RotationalPeriod = rng.D100 * 24;
                    else RotationalPeriod = (rng.D1000 + 100) * 24;
                else if (type == Type.Terrestial_planet)
                    if (a <= 5) RotationalPeriod = 10 + rng.D10 * 2;
                    else if (a <= 7) RotationalPeriod = 30 + rng.D100;
                    else if (a <= 9) RotationalPeriod = rng.D100 * 48;
                    else RotationalPeriod = (rng.D1000 + 100) * 24;
                else if (type == Type.Gas_Giant || type == Type.Superjovian)
                    if (a <= 5) RotationalPeriod = 6 + rng.D10 / 2.0;
                    else if (a <= 7) RotationalPeriod = 11 + rng.D10 / 2.0;
                    else if (a <= 9) RotationalPeriod = 16 + rng.D10;
                    else RotationalPeriod = 26 + rng.D10;
                else throw new ArgumentException("You tried to calculate the day for an " + type);

                double mod = (int)(tidalForce * parent.starSystem.Age);
                if (type == Type.Terrestial_planet) mod -= Math.Sqrt(Mass);
                if (type == Type.Gas_Giant && Mass < 50) mod += 2;
                RotationalPeriod *= 1 + (mod * 0.1);
                // Axial tilt (table 2.2.3)
                int b = rng.D10;
                if (b <= 2) AxialTilt = rng.D10;
                else if (b <= 4) AxialTilt = 10 + rng.D10;
                else if (b <= 6) AxialTilt = 20 + rng.D10;
                else if (b <= 8) AxialTilt = 30 + rng.D10;
                else AxialTilt = 40 + rng.D100 * 1.4;
            }
            else
            {
                if (OrbElements.e < 0.11) RotationalPeriod = OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.30) RotationalPeriod = 3 / 2 * OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.48) RotationalPeriod = 2 / 1 * OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.65) RotationalPeriod = 5 / 2 * OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.80) RotationalPeriod = 3 / 1 * OrbElements.T.TotalHours;
                else RotationalPeriod = 7 / 2 * OrbElements.T.TotalHours;
                AxialTilt = 0;
            }
        }

        private Type RollType()
        {
            int a = rng.D10;
            if (innerPlanet)
                if (a <= 18) return Type.Astroid_Belt;
                else if (a <= 62) return Type.Terrestial_planet;
                else if (a <= 71) return Type.Chunk;
                else if (a <= 82) return Type.Gas_Giant;
                else if (a <= 86) return Type.Superjovian;
                else if (a <= 96) return Type.Empty;
                else if (a <= 97) return Type.Interloper;
                else if (a <= 98) return Type.Trojan;
                else if (a <= 99) return Type.Double_Planet;
                else return Type.Captured;
            else
                if (a <= 15) return Type.Astroid_Belt;
            else if (a <= 23) return Type.Terrestial_planet;
            else if (a <= 35) return Type.Chunk;
            else if (a <= 74) return Type.Gas_Giant;
            else if (a <= 84) return Type.Superjovian;
            else if (a <= 94) return Type.Empty;
            else if (a <= 95) return Type.Interloper;
            else if (a <= 97) return Type.Trojan;
            else if (a <= 99) return Type.Double_Planet;
            else return Type.Captured;
        }

        public List<Planet> ResolveAstroidBelt()
        {
            // NOTE: the sol astroid belt contains more then 200 objects larger than 100 km. The chunk minimum size is 200 km
            throw new NotImplementedException();
        }

        public void ResolveInterloper()
        {
            if (type != Type.Interloper)
                throw new ArgumentException("You tried to resolve the interloper status of a planet with type: " + type);
            innerPlanet = !innerPlanet;
            do
            {
                type = RollType();
            } while (type != Type.Chunk || type != Type.Terrestial_planet || type != Type.Gas_Giant);
            CalculateSize();
        }

        public Planet ResolveTrojan()
        {
            if (type != Type.Trojan)
                throw new ArgumentException("You tried to resolve the trojan status of a planet with type: " + type);
            if (rng.D10 <= 8) type = Type.Gas_Giant;
            else type = Type.Superjovian;
            OrbitalElements companionElements = new OrbitalElements(OrbElements.LAN, OrbElements.i, OrbElements.AOP,
                OrbElements.MAaE + Math.PI / 3 * rng.D10 > 5 ? +1 : -1,
                OrbElements.SMA, OrbElements.e, OrbElements.parentMass);
            Planet companion = new Planet(parent, innerPlanet, rng.D10 <= 9 ? Type.Chunk : Type.Terrestial_planet, companionElements);
            CalculateSize();
            companion.CalculateSize();
            return companion;
        }

        public Planet ResolveDoublePlanet()
        {
            if (type != Type.Double_Planet)
                throw new ArgumentException("You tried to resolve the double planet status of a planet with type: " + type);
            Type firstType = RollType();
            if (firstType != Type.Chunk || firstType != Type.Terrestial_planet || firstType != Type.Gas_Giant || firstType != Type.Superjovian)
                firstType = Type.Chunk;
            Type secondType = RollType();
            if (secondType != Type.Chunk || secondType != Type.Terrestial_planet || secondType != Type.Gas_Giant || secondType != Type.Superjovian)
                secondType = Type.Chunk;
            throw new NotImplementedException();
        }

        public void ResolveCaptured()
        {
            do type = RollType();
            while (type != Type.Chunk || type != Type.Terrestial_planet || type != Type.Gas_Giant || type != Type.Superjovian);
            isCaptured = true;
            CalculateSize();
        }

        public enum Type { Chunk, Terrestial_planet, Gas_Giant, Superjovian, Astroid_Belt, Empty, Interloper, Trojan, Double_Planet, Captured}




        internal void AddPopulation(Population p)
        {
            throw new NotImplementedException();
        }
    }
}