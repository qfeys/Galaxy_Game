using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Empires;

namespace Assets.Scripts.Bodies
{
    internal class Planet
    {
        #region StallarVariables&functions

        /// <summary>
        /// The parent star
        /// </summary>
        public Star Parent { get; private set; }
        /// <summary>
        /// The parent planet if this is a moon
        /// </summary>
        public Planet ParentPlanet { get; private set; }
        /// <summary>
        /// The type of planet this planet is
        /// </summary>
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
        /// The sidereal rotational period in hours
        /// </summary>
        public double RotationalPeriod { get; private set; }
        /// <summary>
        /// The axial tilt in radian [0-2Pi], relative to the ecliptica
        /// </summary>
        public double AxialTilt { get; private set; }
        /// <summary>
        /// The direction that points outward from the north pole, as measured on the ecliptica, in radian [0-2Pi]
        /// </summary>
        public double NorthDirection { get; private set; }
        /// <summary>
        /// The solar day in hours
        /// </summary>
        public double SolarDay { get { return RotationalPeriod == 0 ? 0 : 1 / (1 / RotationalPeriod + 1 / OrbElements.T.TotalHours * (AxialTilt < 90 ? -1 : +1)); } }
        /// <summary>
        /// The age of this planets system.S
        /// </summary>
        public double Age { get { return Parent.starSystem.Age; } }
        /// <summary>
        /// The luminocity of a planet, in Sol equivalents. This is only relevant for superjovians
        /// </summary>
        public double Luminosity { get; private set; }
        /// <summary>
        /// The temperature of a planet, before modifications, in Kelvin.
        /// </summary>
        public double BaseTemperature { get; private set; }
        /// <summary>
        /// The surface temperature of a planet, in Kelvin.
        /// </summary>
        public double SurfaceTemperature { get; private set; }
        /// <summary>
        /// The tectonic activity of a planet.
        /// </summary>
        public Tectonics TectonicActivity { get; private set; }
        public enum Tectonics { Dead, HotSpot, Plastic, Plates, Platelets, Extreme }
        /// <summary>
        /// The magnetic field strength of a planet, relative to Earth
        /// </summary>
        public double MagneticFieldStrength { get; private set; }
        /// <summary>
        /// The type of hydrosphere this planet has
        /// </summary>
        public HydrosphereType Hydrosphere { get; private set; }
        public enum HydrosphereType { None, Vapor, Liquid, IceSheet, Crustal }
        /// <summary>
        /// The extend of the hydrosphere, in part of the planet surface [0,1]
        /// </summary>
        public double HydrosphereExtend { get; private set; }
        /// <summary>
        /// The amount of water vapor in the atmosphere
        /// </summary>
        public double WatorVaporFactor { get; private set; }
        /// <summary>
        /// The composition of the atmosphere. Values are partial pressures given in atm
        /// Note: frozen gasses are still mentioned in this dict. Check boiling point before using.
        /// </summary>
        public Dictionary<Gases,double> AtmosphericComposition { get; private set; }
        /// <summary>
        /// The pressure at sea level, in atm
        /// </summary>
        public double PressureAtSeaLevel { get { return (AtmosphericComposition == null || AtmosphericComposition.Count == 0) ? 0 : AtmosphericComposition.Sum(g => g.Value); } }
        /// <summary>
        /// The type of life this planet has.
        /// </summary>
        public LifeTypes Life { get; private set; }
        public enum LifeTypes { None, Microbial, Algae, Complex, Thinking, Sapient}
        /// <summary>
        /// The albedo of the planet, relative to Earth. A lower factor represents a higher reflectivity
        /// </summary>
        public double AlbedoFactor { get; private set; }
        /// <summary>
        /// This keeps track of all the minable materials on this planet.
        /// </summary>
        public MiningPile MiningPile { get; private set; }

        AstroidTypes AstroidType = AstroidTypes.NotAnAstroid;
        enum AstroidTypes { NotAnAstroid, Metallic, Silicate, Carbonaceous, Icy }
        public List<Planet> moons { get; private set; }
        bool innerPlanet;
        bool isMoon;
        bool isCaptured = false;
        bool isTidallyLocked;

        public RNG rng { get { return Parent.starSystem.rng; } }

        public Planet(Star parent, double meanDistance, bool InnerPlanet)
        {
            this.Parent = parent;
            Parent.AddPlanet(this);
            innerPlanet = InnerPlanet;
            type = RollType();
            // Calculate orbital elements
            double eccentricity = 0;
            int a = rng.D10 + (type == Type.Captured ? +3 : 0);
            if (a <= 5) eccentricity = rng.D10 * 0.005;
            else if (a <= 7) eccentricity = rng.D10 * 0.01 + 0.05;
            else if (a <= 9) eccentricity = rng.D10 * 0.01 + 0.15;
            else eccentricity = rng.D10 * 0.04 + 0.25;
            // Inclination is not in the paper, except for a small mention on page 16
            // Inclination is not concidered

            OrbElements = new OrbitalElements(rng.Circle, rng.Circle, meanDistance, eccentricity, parent.Mass * Star.SOLAR_MASS);
            moons = new List<Planet>();

            CalculateSize();
            MiningPile = new MiningPile(this);
        }

        /// <summary>
        /// Bearbones constructor, just fills in the given values
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="innerPlanet"></param>
        /// <param name="type"></param>
        /// <param name="orbitalElements"></param>
        Planet(Star parent, bool innerPlanet, Type type, OrbitalElements orbitalElements)
        {
            this.Parent = parent;
            Parent.AddPlanet(this);
            this.innerPlanet = innerPlanet;
            this.type = type;
            OrbElements = orbitalElements;
            moons = new List<Planet>();
            MiningPile = new MiningPile(this);
        }

        /// <summary>
        /// Constructor for moons
        /// </summary>
        /// <param name="planet">The parent planet</param>
        /// <param name="semiMajorAxis">Given in planet diameters</param>
        /// <param name="innerPlanet"></param>
        Planet(Planet planet, double semiMajorAxis, bool innerPlanet)
        {
            this.innerPlanet = innerPlanet;
            isMoon = true;
            ParentPlanet = planet;
            Parent = ParentPlanet.Parent;
            double sma = semiMajorAxis * ParentPlanet.Radius / StarSystem.AU;
            OrbElements = new OrbitalElements(0, rng.Circle, sma, 0, ParentPlanet.Mass * EARTH_MASS);
            // Radius
            int a = rng.D100 + Parent.starSystem.Abundance * (Parent.starSystem.Abundance < 0 ? 2 : 1);
            if (a <= 64) { Radius = rng.D10 * 10; type = Type.Chunk; }
            else if (a <= 84) { Radius = rng.D10 * 100; type = Type.Chunk; }
            else if (a <= 94) { Radius = rng.D10 * 100 + 1000; type = Type.Chunk; }
            else if (a <= 99) { Radius = rng.D10 * 200 + 2000; type = Type.Terrestial_planet; }
            else { Radius = 4000 + rng.D10 * 400; type = Type.Terrestial_planet; }
            Density = innerPlanet ? 0.3 + rng.D10 * 0.1 : 0.1 + rng.D10 * 0.05;
            Mass = Math.Pow(Radius / 6380, 3) * Density;
            // Test for rings
            double rocheLimit = 2.456 * Math.Pow(ParentPlanet.Density / Density, 0.33);
            if (semiMajorAxis < rocheLimit)
                type = Type.Ring;
            CalculateDay();
            MiningPile = new MiningPile(this);
        }

        void CalculateSize()
        {
            if (type == Type.Chunk || type == Type.Terrestial_planet || type == Type.Gas_Giant)
            {
                int a = rng.D10;
                if (type == Type.Chunk) Radius = 200 * a;
                else if (type == Type.Terrestial_planet)
                    if (a == 1) Radius = 2000 + rng.D10 * 100;
                    else if (a + Parent.starSystem.Abundance <= 2) Radius = 2000 + rng.D10 * 100;
                    else if (a + Parent.starSystem.Abundance <= 4) Radius = 3000 + rng.D10 * 100;
                    else if (a + Parent.starSystem.Abundance <= 8) Radius = (a - 1) * 1000 + rng.D10 * 100;
                    else if (a + Parent.starSystem.Abundance <= 9) Radius = 8000 + rng.D10 * 200;
                    else Radius = 10000 + rng.D10 * 500;
                else if (type == Type.Gas_Giant)
                    if (a <= 5) Radius = (a + 4) * 3000 + rng.D10 * 300;
                    else Radius = (a - 3) * 10000 + rng.D10 * 1000;

                int b = rng.D10 + Parent.starSystem.Abundance;
                if (b < 1) b = 1; else if (b > 11) b = 11;
                if (innerPlanet)
                    if (type == Type.Chunk || type == Type.Terrestial_planet) Density = 0.3 + b * 0.127 / Math.Pow(0.4 + OrbElements.SMA / Math.Sqrt(Parent.Luminosity), 0.67);
                    else if (type == Type.Gas_Giant) Density = 0.1 + b * 0.025;
                    else { }
                else
                    if (type == Type.Chunk || type == Type.Terrestial_planet) Density = 0.1 + b * 0.05;
                    else if (type == Type.Gas_Giant) Density = 0.08 + b * 0.025;
                if (Density > 1.5) Density = 1.5;

                Mass = Math.Pow(Radius / 6380.0, 3) * Density;
            }
            else if (type == Type.Superjovian)
            {
                int a = rng.D10;
                if (a <= 4) Mass = 500 * rng.D10 * 50;
                else if (a <= 7) Mass = 1000 + rng.D10 * 100;
                else if (a <= 9) Mass = 2000 + rng.D10 * 100;
                else Mass = 3000 + rng.D10 * 100;

                Radius = (int)(60000 + (rng.D10 - Age / 2) * 2000);
                Density = Mass * Math.Pow(6380 / Radius, 3);
                // Luminocity (see chart at the bottom of page 19)
                int row = (int)Math.Round(Mass / 500 - Age);
                if (row < 1) { Luminosity = 1.6e-8 * Math.Pow(0.5, -row + 1); BaseTemperature = 200 - 30 * (-row + 1); }
                else if (row == 1) { Luminosity = 1.6e-8; BaseTemperature = 200; }
                else if (row == 2) { Luminosity = 3.4e-8; BaseTemperature = 240; }
                else if (row == 3) { Luminosity = 6.2e-8; BaseTemperature = 280; }
                else if (row == 4) { Luminosity = 1.4e-7; BaseTemperature = 340; }
                else if (row == 5) { Luminosity = 2.6e-7; BaseTemperature = 400; }
                else if (row == 6) { Luminosity = 4.5e-7; BaseTemperature = 460; }
                else if (row == 7) { Luminosity = 8.0e-7; BaseTemperature = 530; }
                else if (row == 8) { Luminosity = 1.4e-6; BaseTemperature = 610; }
            }
        }

        public void CalculateDay()
        {
            if (type == Type.Double_Planet || type == Type.Astroid_Belt || type == Type.Ring) return; // These have nothing that has to be resolved here.
            double tidalForce = (isMoon ? (ParentPlanet.Mass * Star.SOLAR_MASS / EARTH_MASS) : Parent.Mass) * 26640000 / Math.Pow(OrbElements.SMA * 400, 3);
            isTidallyLocked = (0.83 + rng.D10 * 0.03) * tidalForce * Age / 6.6 > 1;
            if (isTidallyLocked == false)
            {
                // Rotational period (table 2.2.2)
                int a = rng.D10 + (int)(tidalForce * Age);
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

                double mod = (int)(tidalForce * Age);
                if (type == Type.Terrestial_planet && Mass > 2) mod -= Math.Sqrt(Mass);
                if (type == Type.Gas_Giant && Mass < 50) mod += 2;
                RotationalPeriod *= 1 + (mod * 0.1);
                // Axial tilt (table 2.2.3)
                if (NorthDirection == 0)    // Double planet descendants will have this already assigned
                {
                    int b = rng.D10;
                    if (b <= 2) AxialTilt = rng.D10 * Math.PI / 180;
                    else if (b <= 4) AxialTilt = (10 + rng.D10) * Math.PI / 180;
                    else if (b <= 6) AxialTilt = (20 + rng.D10) * Math.PI / 180;
                    else if (b <= 8) AxialTilt = (30 + rng.D10) * Math.PI / 180;
                    else AxialTilt = (40 + rng.D100 * 1.4) * Math.PI / 180;
                    NorthDirection = rng.Circle;
                }
            }
            else
            {
                if (OrbElements.e < 0.11) RotationalPeriod = OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.30) RotationalPeriod = 3 / 2 * OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.48) RotationalPeriod = 2 / 1 * OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.65) RotationalPeriod = 5 / 2 * OrbElements.T.TotalHours;
                else if (OrbElements.e < 0.80) RotationalPeriod = 3 / 1 * OrbElements.T.TotalHours;
                else RotationalPeriod = 7 / 2 * OrbElements.T.TotalHours;
                AxialTilt = isMoon ? ParentPlanet.AxialTilt : 0;
                NorthDirection = (OrbElements.MAaE + 3 * Math.PI / 4) % (Math.PI * 2);
            }
        }

        public void CalculateMoons()
        {
            if (isTidallyLocked || type == Type.Astroid_Belt || type == Type.Double_Planet)
                return;
            int numberOfMoons = 0;
            int a = rng.D10 + (innerPlanet ? 0 : 5);
            if (type == Type.Chunk)
                if (a <= 9) numberOfMoons = 0;
                else numberOfMoons = 1;
            else if (type == Type.Terrestial_planet) // Numbers a little bit different than in the paper
                if (a <= 5) numberOfMoons = 0;
                else if (a <= 7) numberOfMoons = 1;
                else if (a <= 9) numberOfMoons = 1 + rng.D10 / 5;
                else if (a <= 13) numberOfMoons = 1 + rng.D10 / 2;
                else numberOfMoons = 1 + rng.D10;
            else if (type == Type.Gas_Giant || type == Type.Superjovian)
                if (a <= 0) numberOfMoons = 0;
                else if (a <= 5) numberOfMoons = rng.D10 / 2;
                else if (a < 7) numberOfMoons = rng.D10;
                else if (a < 9) numberOfMoons = rng.D10 + 5;
                else if (a < 13) numberOfMoons = rng.D10 + 10;
                else numberOfMoons = rng.D10 + 20;
            else throw new ArgumentException("You try to calculate the moons for a planet of type " + type);

            List<double> orbits = new List<double>();
            for (int i = 0; i < numberOfMoons; i++)
            {
                int b = rng.D10;
                if (b <= 4) orbits.Add(1 + rng.D10 * 0.5);      // Close
                else if (b <= 6) orbits.Add(6 + rng.D10 * 1);   // Average
                else if (b <= 9) orbits.Add(16 + rng.D10 * 3);  // Distant
                else orbits.Add(45 + rng.D100 * 3);             // Very Distant
                // Disregarded special orbits TODO: implement special orbits
            }
            // Check for collisions
            int n = Parent.Planets.IndexOf(this);
            double maxDistance = double.PositiveInfinity;
            if (n == 0 && Parent.Planets.Count > 1)  // closest point of biggest orbit - biggest point of closest orbit.
                maxDistance = (Parent.Planets[n + 1].OrbElements.SMA * (1 - Parent.Planets[n + 1].OrbElements.e) - OrbElements.SMA * (1 + OrbElements.e)) / 3;
            else if (n > 0 && Parent.Planets.Count > n + 1)
                maxDistance = Math.Min(
                    OrbElements.SMA * (1 - OrbElements.e) - Parent.Planets[n - 1].OrbElements.SMA * (1 + Parent.Planets[n - 1].OrbElements.e),
                    Parent.Planets[n + 1].OrbElements.SMA * (1 - Parent.Planets[n + 1].OrbElements.e) - OrbElements.SMA * (1 + OrbElements.e)
                    ) / 3;
            else if (n != 0 && Parent.Planets.Count == n + 1)
                maxDistance = (OrbElements.SMA * (1 - OrbElements.e) - Parent.Planets[n - 1].OrbElements.SMA * (1 + Parent.Planets[n - 1].OrbElements.e)) / 3;
            else if (n == 0 && Parent.Planets.Count == 1)
            { }
            else Simulation.God.Log("The index of this planet is " + n + " and the number of planets around this star is " + Parent.Planets.Count + ". Is this alright?");

            orbits.RemoveAll(o => o * Radius / StarSystem.AU > maxDistance);
            orbits.ForEach(o => moons.Add(new Planet(this, o, innerPlanet)));
        }

        public void CalculateGeophisicals()
        {
            // Plate tectonics
            {
                double tectonicActivityFactor = (5 + rng.D10) * Math.Sqrt(Mass) / Age;
                if (moons != null && moons.Count != 0)
                {
                    Planet bestMoon = moons.Find(p => p.Mass / Math.Pow(p.OrbElements.SMA, 3) == moons.Max(m => m.Mass / Math.Pow(m.OrbElements.SMA, 3)));
                    double tidalForce = (bestMoon.Mass * Star.SOLAR_MASS / EARTH_MASS) * 26640000 / Math.Pow(bestMoon.OrbElements.SMA * 400, 3);
                    tectonicActivityFactor *= 1 + 0.25 * tidalForce;
                    // TODO: Verify this is a usefull number
                }
                if (innerPlanet == false && Density <= 0.45)
                    tectonicActivityFactor *= Density;
                if (RotationalPeriod <= 18) tectonicActivityFactor *= 1.25;
                else if (RotationalPeriod >= 100) tectonicActivityFactor *= 0.75;
                if (RotationalPeriod >= OrbElements.T.TotalHours || isTidallyLocked == true) tectonicActivityFactor *= 0.5;
                int a = rng.D10;
                if (tectonicActivityFactor < 0.5) TectonicActivity = Tectonics.Dead;
                else if (tectonicActivityFactor < 1.0)
                    if (a <= 7) TectonicActivity = Tectonics.Dead;
                    else if (a <= 9) TectonicActivity = Tectonics.HotSpot;
                    else TectonicActivity = Tectonics.Plastic;
                else if (tectonicActivityFactor < 2.0)
                    if (a <= 1) TectonicActivity = Tectonics.Dead;
                    else if (a <= 5) TectonicActivity = Tectonics.HotSpot;
                    else if (a <= 9) TectonicActivity = Tectonics.Plastic;
                    else TectonicActivity = Tectonics.Plates;
                else if (tectonicActivityFactor < 3.0)
                    if (a <= 2) TectonicActivity = Tectonics.HotSpot;
                    else if (a <= 6) TectonicActivity = Tectonics.Plastic;
                    else TectonicActivity = Tectonics.Plates;
                else if (tectonicActivityFactor < 5.0)
                    if (a <= 1) TectonicActivity = Tectonics.HotSpot;
                    else if (a <= 3) TectonicActivity = Tectonics.Plastic;
                    else if (a <= 8) TectonicActivity = Tectonics.Plates;
                    else TectonicActivity = Tectonics.Platelets;
                else
                    if (a <= 1) TectonicActivity = Tectonics.Plastic;
                else if (a <= 2) TectonicActivity = Tectonics.Plates;
                else if (a <= 7) TectonicActivity = Tectonics.Platelets;
                else TectonicActivity = Tectonics.Extreme;
            }

            // Magnetic field: 10 * 1/sqrt(hours/24) * density^2 * sqrt(mass) / Age
            if (type == Type.Terrestial_planet || type == Type.Chunk)
            {
                double magneticFieldFactor = 10 * 1 / Math.Sqrt(Math.Min(RotationalPeriod, OrbElements.T.TotalHours) / 24) * Math.Pow(Density, 2) * Math.Sqrt(Mass) / Age;
                if (innerPlanet == false && Density <= 0.45)
                    magneticFieldFactor *= 0.5;
                int b = rng.D10;
                if (magneticFieldFactor <= 0.05) MagneticFieldStrength = 0;
                else if (magneticFieldFactor <= 0.5)
                    if (b <= 5) MagneticFieldStrength = 0;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.001;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 0.002;
                    else MagneticFieldStrength = rng.D10 * 0.01;
                else if (magneticFieldFactor <= 1.0)
                    if (b <= 3) MagneticFieldStrength = 0;
                    else if (b <= 5) MagneticFieldStrength = rng.D10 * 0.001;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.002;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 0.01;
                    else MagneticFieldStrength = rng.D10 * 0.05;
                else if (magneticFieldFactor <= 2.0)
                    if (b <= 3) MagneticFieldStrength = rng.D10 * 0.001;
                    else if (b <= 5) MagneticFieldStrength = rng.D10 * 0.002;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.01;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 0.05;
                    else MagneticFieldStrength = rng.D10 * 0.1;
                else if (magneticFieldFactor <= 4.0)
                    if (b <= 3) MagneticFieldStrength = rng.D10 * 0.05;
                    else if (b <= 5) MagneticFieldStrength = rng.D10 * 0.1;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.2;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 0.3;
                    else MagneticFieldStrength = rng.D10 * 0.5;
                else
                    if (b <= 3) MagneticFieldStrength = rng.D10 * 0.1;
                else if (b <= 5) MagneticFieldStrength = rng.D10 * 0.2;
                else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.3;
                else if (b <= 9) MagneticFieldStrength = rng.D10 * 0.5;
                else MagneticFieldStrength = rng.D10 * 1.0;
            }
            else if (type == Type.Gas_Giant || type == Type.Superjovian)
            {
                int b = rng.D10;
                if (Mass <= 50)
                    if (b <= 1) MagneticFieldStrength = rng.D10 * 0.1;
                    else if (b <= 4) MagneticFieldStrength = rng.D10 * 0.25;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.50;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 0.75;
                    else MagneticFieldStrength = rng.D10 * 1.00;
                else if (Mass <= 200)
                    if (b <= 1) MagneticFieldStrength = rng.D10 * 0.25;
                    else if (b <= 4) MagneticFieldStrength = rng.D10 * 0.50;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 0.75;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 1.00;
                    else MagneticFieldStrength = rng.D10 * 1.50;
                else if (Mass <= 500)
                    if (b <= 1) MagneticFieldStrength = rng.D10 * 0.50;
                    else if (b <= 4) MagneticFieldStrength = rng.D10 * 1.00;
                    else if (b <= 7) MagneticFieldStrength = rng.D10 * 1.50;
                    else if (b <= 9) MagneticFieldStrength = rng.D10 * 2.00;
                    else MagneticFieldStrength = rng.D10 * 3.00;
                else
                    if (b <= 1) MagneticFieldStrength = rng.D10 * 1.5;
                else if (b <= 4) MagneticFieldStrength = rng.D10 * 2.5;
                else if (b <= 7) MagneticFieldStrength = rng.D10 * 5;
                else if (b <= 9) MagneticFieldStrength = rng.D10 * 10;
                else MagneticFieldStrength = rng.D10 * 25;
            }

            // Base Temperature
            if (isMoon == false)
                if (type == Type.Chunk || type == Type.Terrestial_planet || type == Type.Gas_Giant)
                    BaseTemperature = 255 / Math.Sqrt(OrbElements.SMA / Math.Sqrt(Parent.Luminosity));
                else if (type == Type.Superjovian)
                    BaseTemperature = Math.Pow(Math.Pow(BaseTemperature, 4) + Math.Pow(255 / Math.Sqrt(OrbElements.SMA / Math.Sqrt(Parent.Luminosity)), 4), 0.25);
                else { }
            else
                BaseTemperature = Math.Pow(Math.Pow(255 / Math.Sqrt(OrbElements.SMA / Math.Sqrt(ParentPlanet.Luminosity)), 4) +
                    Math.Pow(255 / Math.Sqrt(ParentPlanet.OrbElements.SMA / Math.Sqrt(Parent.Luminosity)), 4), 0.25);
            // Parts only for small bodies
            if (type == Type.Chunk || type == Type.Terrestial_planet)
            {
                // Hydrosphere
                if (innerPlanet || (isMoon && ParentPlanet.Mass > 200))
                    if (BaseTemperature > 500) Hydrosphere = HydrosphereType.None;
                    else if (BaseTemperature > 370) Hydrosphere = HydrosphereType.Vapor;
                    else if (BaseTemperature > 245) Hydrosphere = HydrosphereType.Liquid;
                    else Hydrosphere = HydrosphereType.IceSheet;
                else
                    Hydrosphere = HydrosphereType.Crustal;
                if (Hydrosphere == HydrosphereType.Liquid || Hydrosphere == HydrosphereType.IceSheet)
                {
                    int b = rng.D10 + ((isMoon ? ParentPlanet.OrbElements.SMA : OrbElements.SMA) > 1.4 * Math.Sqrt(Parent.Luminosity) ? +1 : 0);
                    if (Radius < 2000)
                        if (b <= 5) HydrosphereExtend = 0;
                        else if (b <= 7) HydrosphereExtend = rng.D10;
                        else if (b <= 8) HydrosphereExtend = 10 + rng.D10;
                        else if (b <= 9) HydrosphereExtend = rng.nD10(5);
                        else HydrosphereExtend = 10 + rng.nD10(10);
                    else if (Radius < 4000)
                        if (b <= 2) HydrosphereExtend = 0;
                        else if (b <= 4) HydrosphereExtend = rng.D10;
                        else if (b <= 9) HydrosphereExtend = (b - 4) * 10 + rng.D10;
                        else HydrosphereExtend = 10 + rng.nD10(10);
                    else if (Radius < 7000)
                        if (b <= 1) HydrosphereExtend = 0;
                        else if (b <= 2) HydrosphereExtend = rng.nD10(2);
                        else if (b <= 8) HydrosphereExtend = (b - 1) * 10 + rng.D10;
                        else if (b <= 9) HydrosphereExtend = 80 + rng.nD10(2);
                        else HydrosphereExtend = 100;
                    else
                        if (b <= 1) HydrosphereExtend = 0;
                    else if (b <= 2) HydrosphereExtend = rng.nD10(2);
                    else if (b <= 4) HydrosphereExtend = (b - 2) * 20 + rng.nD10(2);
                    else if (b <= 8) HydrosphereExtend = (b + 1) * 10 + rng.D10;
                    else HydrosphereExtend = 100;
                    HydrosphereExtend *= 0.01;  // Transfer from percentage to parts
                    WatorVaporFactor = (BaseTemperature - 240) * HydrosphereExtend * rng.D10;
                    if (WatorVaporFactor < 0) WatorVaporFactor = 0;
                }

                // Atmospheric composition
                {
                    List<Gases> atmGases = new List<Gases>();
                    int c = rng.D10;
                    if (c <= 4)
                        if (BaseTemperature > 150) { atmGases.Add(Gases.N2); atmGases.Add(Gases.CO2); }
                        else if (BaseTemperature > 50) { atmGases.Add(Gases.N2); atmGases.Add(Gases.CH4); }
                        else { atmGases.Add(Gases.H2); }
                    else if (c <= 6)
                        if (BaseTemperature > 150) atmGases.Add(Gases.CO2);
                        else if (BaseTemperature > 50) { atmGases.Add(Gases.H2); atmGases.Add(Gases.He); atmGases.Add(Gases.N2); }
                        else atmGases.Add(Gases.He);
                    else if (c <= 8)
                        if (BaseTemperature > 400) { atmGases.Add(Gases.NO2); atmGases.Add(Gases.SO2); }
                        else if (BaseTemperature > 150) { atmGases.Add(Gases.N2); atmGases.Add(Gases.CH4); }
                        else if (BaseTemperature > 50) { atmGases.Add(Gases.N2); atmGases.Add(Gases.CO); }
                        else { atmGases.Add(Gases.He); atmGases.Add(Gases.H2); }
                    else if (c <= 10) // not using special atmospheres. TODO: Add special atmospheres
                        if (BaseTemperature > 400) { atmGases.Add(Gases.SO2); }
                        else if (BaseTemperature > 240) { atmGases.Add(Gases.CO2); atmGases.Add(Gases.CH4); atmGases.Add(Gases.NH3); }
                        else if (BaseTemperature > 150) { atmGases.Add(Gases.H2); atmGases.Add(Gases.He); }
                        else if (BaseTemperature > 50) { atmGases.Add(Gases.He); atmGases.Add(Gases.H2); }
                        else { atmGases.Add(Gases.Ne); }

                    double minWeight = 0.02783 * BaseTemperature / Math.Pow(EscapeVelocity, 2);
                    bool hasEscapedGas = false;
                    foreach (var gas in atmGases.ToList())
                    {
                        if (GasData[gas].Item1 < minWeight)
                        {
                            hasEscapedGas = true;
                            atmGases.Remove(gas);
                        }
                    }
                    SpectralClass.Class_ primeClass_ = Parent.starSystem.Primary.spc.class_;
                    double primeTemp = Parent.starSystem.Primary.Temperature;
                    bool hasUVInfall = ((primeClass_ == SpectralClass.Class_.B || primeClass_ == SpectralClass.Class_.A) && primeTemp > 150000) ||
                        (primeClass_ == SpectralClass.Class_.F && primeTemp > 180000) ||
                        (primeClass_ == SpectralClass.Class_.G && primeTemp > 200000) ||
                        (primeClass_ == SpectralClass.Class_.K && primeTemp > 230000) ||
                        (primeClass_ == SpectralClass.Class_.M && primeTemp > 260000);
                    if (hasUVInfall)
                    {
                        if (atmGases.Contains(Gases.NH3)) { atmGases.Remove(Gases.NH3); atmGases.Add(Gases.N2); }
                        if (atmGases.Contains(Gases.CH4)) { atmGases.Remove(Gases.CH4); hasEscapedGas = true; }
                        if (atmGases.Contains(Gases.H2S)) { atmGases.Remove(Gases.H2S); hasEscapedGas = true; }
                        if (atmGases.Contains(Gases.H2O)) { atmGases.Remove(Gases.H2O); hasEscapedGas = true; }
                    }
                    if (TectonicActivity == Tectonics.Dead)
                    {
                        if (atmGases.Contains(Gases.SO2)) atmGases.Remove(Gases.SO2);
                        if (atmGases.Contains(Gases.H2S)) atmGases.Remove(Gases.H2S);
                    }

                    double majorityGasPerc = 0;
                    int d = rng.D10;
                    if (d <= 5) majorityGasPerc = 50 + rng.nD10(4);
                    else if (d <= 8) majorityGasPerc = 75 + rng.nD10(2);
                    else majorityGasPerc = 95 + rng.D10 / 2.0;
                    double totalPressure = 0;
                    int e = rng.D10 + (TectonicActivity == Tectonics.Dead ? -1 : 0) +
                        (TectonicActivity == Tectonics.Extreme ? +1 : 0) +
                        (hasEscapedGas ? -1 : 0);
                    if (e <= 2) totalPressure = rng.D10 * 0.01;
                    else if (e <= 4) totalPressure = rng.D10 * 0.1;
                    else if (e <= 7) totalPressure = rng.D10 * 0.2;
                    else if (e <= 8) totalPressure = rng.D10 * 0.5;
                    else if (e <= 9) totalPressure = rng.D10 * 2.0;
                    else totalPressure = rng.D10 * 20.0;

                    AtmosphericComposition = new Dictionary<Gases, double>();
                    if (atmGases.Count != 0)
                    {
                        if (atmGases.Count == 1)
                            AtmosphericComposition.Add(atmGases[0], totalPressure * 0.01);
                        else
                        {
                            AtmosphericComposition.Add(atmGases[0], totalPressure * majorityGasPerc * 0.01);
                            for (int i = 1; i < atmGases.Count; i++)
                            {
                                AtmosphericComposition.Add(atmGases[i], totalPressure * (1 - majorityGasPerc * 0.01) / (atmGases.Count - 1));
                            }
                        }
                    }
                }
                // Albedo
                {
                    int c = rng.D10;
                    if (innerPlanet || (isMoon && ParentPlanet.Mass > 200))
                        c += (AtmosphericComposition.Count == 0 ? +2 : 0) +
                            Math.Min((PressureAtSeaLevel > 5 ? PressureAtSeaLevel < 50 ? -4 : -2 : 0),
                            (Hydrosphere == HydrosphereType.IceSheet && HydrosphereExtend > 0.5 ? HydrosphereExtend > 0.9 ? -4 : -2 : 0));
                    else c += (PressureAtSeaLevel > 1 ? +1 : 0);
                    
                    if(innerPlanet || (isMoon && ParentPlanet.Mass > 200))
                        if (c <= 1) AlbedoFactor = 0.75 * rng.D10 * 0.01;
                        else if (c <= 3) AlbedoFactor = 0.85 + rng.D10 * 0.01;
                        else if (c <= 6) AlbedoFactor = 0.95 + rng.D10 * 0.01;
                        else if (c <= 9) AlbedoFactor = 1.05 + rng.D10 * 0.01;
                        else AlbedoFactor = 1.15 + rng.D10 * 0.01;
                    else
                        if (c <= 3) AlbedoFactor = 0.75 * rng.D10 * 0.01;
                        else if (c <= 5) AlbedoFactor = 0.85 + rng.D10 * 0.01;
                        else if (c <= 7) AlbedoFactor = 0.95 + rng.D10 * 0.01;
                        else if (c <= 9) AlbedoFactor = 1.05 + rng.D10 * 0.01;
                        else AlbedoFactor = 1.15 + rng.D10 * 0.01;
                }

                // Surface temperature
                {
                    double greenhousPressure = AtmosphericComposition.Sum(g =>
                        (g.Key == Gases.CO2 || g.Key == Gases.CH4 || g.Key == Gases.SO2 || g.Key == Gases.NO2) ? g.Value : 0);
                    double greenhouseFactor = 1 + Math.Sqrt(PressureAtSeaLevel) * 0.01 * rng.D10 + Math.Sqrt(greenhousPressure) * 0.1 + WatorVaporFactor * 0.1;
                    SurfaceTemperature = BaseTemperature * AlbedoFactor * greenhouseFactor;
                    // TODO: Check back with the hydrosphere, etc
                }

                // LIFE
                if (Hydrosphere == HydrosphereType.Liquid)
                    if (AtmosphericComposition.ContainsKey(Gases.CO) || AtmosphericComposition.ContainsKey(Gases.CO2) || AtmosphericComposition.ContainsKey(Gases.CH4))
                        if (rng.D10 <= 3)
                            if (rng.D10 == 10)
                                if (rng.D10 == 10)
                                    if (rng.D10 == 10)
                                        if (rng.D10 == 10)
                                            Life = LifeTypes.Sapient;
                                        else Life = LifeTypes.Thinking;
                                    else Life = LifeTypes.Complex;
                                else Life = LifeTypes.Algae;
                            else Life = LifeTypes.Microbial;
                if(Life != LifeTypes.None)
                {
                    if (AtmosphericComposition.ContainsKey(Gases.CO)) { double o2cont; AtmosphericComposition.TryGetValue(Gases.O2, out o2cont); AtmosphericComposition[Gases.O2] = o2cont + AtmosphericComposition[Gases.CO]; AtmosphericComposition.Remove(Gases.CO); }
                    if (AtmosphericComposition.ContainsKey(Gases.CO2)) { double o2cont; AtmosphericComposition.TryGetValue(Gases.O2, out o2cont); AtmosphericComposition[Gases.O2] = o2cont + AtmosphericComposition[Gases.CO2]; AtmosphericComposition.Remove(Gases.CO2); }
                    if (AtmosphericComposition.ContainsKey(Gases.CH4)) { double o2cont; AtmosphericComposition.TryGetValue(Gases.O2, out o2cont); AtmosphericComposition[Gases.O2] = o2cont + AtmosphericComposition[Gases.CH4]; AtmosphericComposition.Remove(Gases.CH4); }
                }
            }

            if (moons != null)
                moons.ForEach(m => m.CalculateGeophisicals());
        }

        private Type RollType()
        {
            int a = rng.D100;
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

        public void ResolveAstroidBelt()
        {
            // NOTE: the sol astroid belt contains more then 200 objects larger than 100 km. The chunk minimum size is 200 km
            if (type != Type.Astroid_Belt)
                throw new ArgumentException("You tried to resolve the astroid belt status of a planet with type: " + type);
            // Desity
            {
                int b = rng.D10 + Parent.starSystem.Abundance;
                if (b < 1) b = 1; else if (b > 11) b = 11;
                if (innerPlanet) Density = 0.3 + b * 0.127 / Math.Pow(0.4 + OrbElements.SMA / Math.Sqrt(Parent.Luminosity), 0.67);                 
                else Density = 0.1 + b * 0.05;
                if (Density > 1.5) Density = 1.5;
            }
            // Type
            int a = rng.D10 + (innerPlanet ? 0 : +6) + (OrbElements.SMA < 0.47 * Math.Sqrt(Parent.Luminosity) ? -2 : 0) +
                (Density <= 0.6 ? 0 : Density <= 0.8 ? -1 : Density <= 1 ? -2 : Density <= 1.2 ? -3 : -5);
            if (a <= -2) AstroidType = AstroidTypes.Metallic;
            else if (a <= 5) AstroidType = AstroidTypes.Silicate;
            else if (a <= 10) AstroidType = AstroidTypes.Carbonaceous;
            else AstroidType = AstroidTypes.Icy;
            // Mass
            int c = rng.D10 + Parent.starSystem.Abundance + (innerPlanet ? -1 : +2) + (Age >= 7 ? -1 : 0) + (Parent.IsCombinedStar ? +2 : 0);
            if (c <= 4) Mass = rng.D10 * 0.0001;
            else if (c <= 6) Mass = rng.D10 * 0.001;
            else if (c <= 8) Mass = rng.D10 * 0.01;
            else if (c <= 10) Mass = rng.D10 * 0.1;
            else Mass = rng.D10 * 1;
            // Create major bodies - This is not part of the paper of Trisen
            double massToLose = Mass * 0.66;
            for (int i = 0; i < 5; i++)
            {
                double d2r = Math.PI / 180;
                Planet astroid = new Planet(Parent, innerPlanet, Type.Chunk,
                    new OrbitalElements(OrbElements.AOP + (rng.D10 - 5) / 2.0 * d2r, rng.Circle,
                        OrbElements.SMA * (1 + (rng.D10 - 5) / 40.0), OrbElements.e, Parent.Mass * Star.SOLAR_MASS)) {
                    isMoon = true,
                    ParentPlanet = this
                };
                astroid.CalculateSize();
                if (massToLose >= astroid.Mass)
                {
                    moons.Add(astroid);
                    i = 0;
                    massToLose -= astroid.Mass;
                    Mass -= astroid.Mass;
                }
            }
        }

        public void ResolveInterloper()
        {
            if (type != Type.Interloper)
                throw new ArgumentException("You tried to resolve the interloper status of a planet with type: " + type);
            innerPlanet = !innerPlanet;
            do
            {
                type = RollType();
            } while (type != Type.Chunk && type != Type.Terrestial_planet && type != Type.Gas_Giant);
            CalculateSize();
        }

        public Planet ResolveTrojan()
        {
            if (type != Type.Trojan)
                throw new ArgumentException("You tried to resolve the trojan status of a planet with type: " + type);
            if (rng.D10 <= 8) type = Type.Gas_Giant;
            else type = Type.Superjovian;
            OrbitalElements companionElements = new OrbitalElements(OrbElements.AOP,
                OrbElements.MAaE + Math.PI / 3 * rng.D10 > 5 ? +1 : -1,
                OrbElements.SMA, OrbElements.e, OrbElements.parentMass * Star.SOLAR_MASS);
            Planet companion = new Planet(Parent, innerPlanet, rng.D10 <= 9 ? Type.Chunk : Type.Terrestial_planet, companionElements);
            CalculateSize();
            companion.CalculateSize();
            return companion;
        }

        public void ResolveDoublePlanet()
        {
            if (type != Type.Double_Planet)
                throw new ArgumentException("You tried to resolve the double planet status of a planet with type: " + type);
            Type firstType = RollType();
            if (firstType != Type.Chunk || firstType != Type.Terrestial_planet || firstType != Type.Gas_Giant || firstType != Type.Superjovian)
                firstType = Type.Chunk;
            Type secondType = RollType();
            if (secondType != Type.Chunk || secondType != Type.Terrestial_planet || secondType != Type.Gas_Giant || secondType != Type.Superjovian)
                secondType = Type.Chunk;

            double seperation;
            int b = rng.D10;
            if (b <= 4) seperation = 1 + rng.D10 * 0.5;      // Close
            else if (b <= 6) seperation = 6 + rng.D10 * 1;   // Average
            else if (b <= 9) seperation = 16 + rng.D10 * 3;  // Distant
            else seperation = 45 + rng.D100 * 3;             // Very Distant

            if (firstType < secondType) // switch them around so firstType is the biggest
            {
                var temp = firstType;
                firstType = secondType;
                secondType = temp;
            }
            if(secondType + 1 < firstType) // First type is two classes lower than second. Make second prime and first a moon
            {
                type = firstType;
                CalculateSize();
                Planet moon = new Planet(Parent, innerPlanet, secondType, 
                    new OrbitalElements(0, rng.Circle, seperation * Radius / StarSystem.AU, 0, Mass * EARTH_MASS)) {
                    isMoon = true,
                    ParentPlanet = this
                };
                moons.Add(moon);
                moon.CalculateSize();
                moon.CalculateDay();
            }
            else    // The two planets are close in mass, so a double system
            {
                Planet prime = new Planet(this.Parent, innerPlanet, firstType, new OrbitalElements()) {
                    isMoon = true,
                    ParentPlanet = this
                };
                Planet second = new Planet(this.Parent, innerPlanet, firstType, new OrbitalElements()) {
                    isMoon = true,
                    ParentPlanet = this
                };
                prime.CalculateSize();
                second.CalculateSize();
                double r1 = seperation / (1 + prime.Mass / second.Mass);
                double apparentParentMassPrimary = Math.Pow(r1, 3) * (prime.Mass + second.Mass) / Math.Pow(seperation, 3);
                double apparentParentMassSecondary = apparentParentMassPrimary * Math.Pow(prime.Mass / second.Mass, 3);
                prime.OrbElements = new OrbitalElements(0, 0, r1, 0, apparentParentMassPrimary * EARTH_MASS);
                second.OrbElements = new OrbitalElements(0, Math.PI, seperation - r1, 0, apparentParentMassSecondary * EARTH_MASS);
                prime.isTidallyLocked = true;
                second.isTidallyLocked = true;
                prime.RotationalPeriod = prime.OrbElements.T.TotalHours;
                second.RotationalPeriod = second.OrbElements.T.TotalHours;
                // They do no longer have to calculate day
            }
        }

        public void ResolveCaptured()
        {
            do type = RollType();
            while (type != Type.Chunk && type != Type.Terrestial_planet && type != Type.Gas_Giant && type != Type.Superjovian);
            isCaptured = true;
            CalculateSize();
        }

        public enum Type { Chunk, Terrestial_planet, Gas_Giant, Superjovian, Astroid_Belt, Ring, Empty, Interloper, Trojan, Double_Planet, Captured }
        public enum Gases { H2, He, CH4, NH3, H2O, Ne, N2, CO, NO, O2, H2S, Ar, CO2, NO2, SO2 }
        /// <summary>
        /// The mass of the Earth in kg
        /// </summary>
        public const double EARTH_MASS = 5.972e24;
        /// <summary>
        /// Gasses with their molecular weight and boiling point at 1 atm.
        /// </summary>
        static public Dictionary<Gases, Tuple<double, double>> GasData = new Dictionary<Gases, Tuple<double, double>>() {
            {Gases.H2, new Tuple<double, double>(2,20) },
            {Gases.He, new Tuple<double, double>(4,4) },
            {Gases.CH4, new Tuple<double, double>(16,109) },
            {Gases.NH3, new Tuple<double, double>(17,240) },
            {Gases.H2O, new Tuple<double, double>(18,373) },
            {Gases.Ne, new Tuple<double, double>(20,27) },
            {Gases.N2, new Tuple<double, double>(28,77) },
            {Gases.CO, new Tuple<double, double>(28,82) },
            {Gases.NO, new Tuple<double, double>(30,121) },
            {Gases.O2, new Tuple<double, double>(32,90) },
            {Gases.H2S, new Tuple<double, double>(34,212) },
            {Gases.Ar, new Tuple<double, double>(40,87) },
            {Gases.CO2, new Tuple<double, double>(44,195) },
            {Gases.NO2, new Tuple<double, double>(46,294) },
            {Gases.SO2, new Tuple<double, double>(64,263) }
        };

        #endregion

        public override string ToString()
        {
            string rt = type.ToString() + " " + Parent.ToString();
            if (isMoon == false)
                rt += (char)(Parent.Planets.IndexOf(this) + 'b');
            else
                rt += (char)(Parent.Planets.IndexOf(ParentPlanet) + 'b') + ParentPlanet.moons.IndexOf(this).ToString();
            return rt;
        }

        public List<Population> Populations { get; private set; }
        public bool IsPopulated { get { return Populations != null && Populations.Count != 0; } }
        public double PopulationCount { get { return Populations == null ? 0 : Populations.Sum(p => p.Count); } }

        internal void AddPopulation(Population p)
        {
            if (Populations == null)
                Populations = new List<Population>();
            Populations.Add(p);
        }
    }
}