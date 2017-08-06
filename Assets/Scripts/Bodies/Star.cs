using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Star
    {

        public StarSystem starSystem { get; private set; }
        public SpectralClass spc { get; private set; }
        public OrbitalElements OrbElements { get; private set; }
        public List<Orbital> Planets { get; private set; }

        /// <summary>
        /// unit is Sol luminocity
        /// </summary>
        public double Luminosity { get; private set; }
        /// <summary>
        /// unit is Sol mass
        /// </summary>
        public double Mass { get; private set; }
        /// <summary>
        /// unit is Kelvin
        /// </summary>
        public int Temperature { get; private set; }
        /// <summary>
        /// unit is Sol radius
        /// </summary>
        public double Radius { get; private set; }

        public Star(StarSystem starSystem, SpectralClass spc)
        {
            this.starSystem = starSystem;
            this.spc = spc;
            
            // Set luminocity
            if(spc.class_ == SpectralClass.Class_.WhiteDwarf)
            {
                int a = starSystem.rng.D10;
                if (a == 1) { Mass = 1.3; Radius = 0.004; }
                if (a == 2) { Mass = 1.1; Radius = 0.007; }
                if (a == 3) { Mass = 0.9; Radius = 0.009; }
                if (a == 4) { Mass = 0.7; Radius = 0.010; }
                if (a == 5) { Mass = 0.6; Radius = 0.011; }
                if (a == 6) { Mass = .55; Radius = 0.012; }
                if (a == 7) { Mass = .50; Radius = 0.013; }
                if (a == 8) { Mass = .45; Radius = 0.014; }
                if (a == 9) { Mass = .40; Radius = 0.015; }
                if (a ==10) { Mass = .35; Radius = 0.016; }
                int b = starSystem.rng.D10 + (int)Math.Round(starSystem.Age - 1) / 2 - 4;
                if (b <= 1) Temperature = 30000;
                if (b == 2) Temperature = 25000;
                if (b == 3) Temperature = 20000;
                if (b == 4) Temperature = 16000;
                if (b == 5) Temperature = 14000;
                if (b == 6) Temperature = 12000;
                if (b == 7) Temperature = 10000;
                if (b == 8) Temperature = 8000;
                if (b == 9) Temperature = 6000;
                if (b ==10) Temperature = 4000;
                Luminosity = Radius * Radius * Math.Pow(Temperature / 5800, 4);
            }
            else if(spc.class_ == SpectralClass.Class_.BrownDwarf)
            {
                int a = starSystem.rng.D10;
                if (a == 1) { Mass = .070; Radius = 0.07; }
                if (a == 2) { Mass = .064; Radius = 0.08; }
                if (a == 3) { Mass = .058; Radius = 0.09; }
                if (a == 4) { Mass = .052; Radius = 0.10; }
                if (a == 5) { Mass = .046; Radius = 0.11; }
                if (a == 6) { Mass = .040; Radius = 0.12; }
                if (a == 7) { Mass = .034; Radius = 0.12; }
                if (a == 8) { Mass = .026; Radius = 0.12; }
                if (a == 9) { Mass = .020; Radius = 0.12; }
                if (a ==10) { Mass = .014; Radius = 0.12; }
                int b = starSystem.rng.D10 +
                    (starSystem.Age == 1 ? 0 : starSystem.Age <= 3 ? 1 : starSystem.Age <= 5 ? 2 : (int)starSystem.Age - 3);
                if (b <= 1) Temperature = 2200;
                if (b == 2) Temperature = 2000;
                if (b == 3) Temperature = 1800;
                if (b == 4) Temperature = 1600;
                if (b == 5) Temperature = 1400;
                if (b == 6) Temperature = 1200;
                if (b == 7) Temperature = 1000;
                if (b == 8) Temperature = 900;
                if (b == 9) Temperature = 800;
                if (b ==10) Temperature = 700;
                Luminosity = Radius * Radius * Math.Pow(Temperature / 5800, 4);
            }
            else    // all other stars
            {
                Luminosity = LumAndMassTable[spc.class_][spc.size][spc.specification].Item1;
                Mass = LumAndMassTable[spc.class_][spc.size][spc.specification].Item2;
                Temperature = LumAndMassTable[spc.class_][spc.size][spc.specification].Item3;
                Radius = LumAndMassTable[spc.class_][spc.size][spc.specification].Item4;

                if (spc.size == SpectralClass.Size.IV)
                {
                    int a = starSystem.rng.D10;
                    double mult = 0;
                    if (a == 3) mult = -.1;
                    if (a == 4) mult = -.2;
                    if (a == 5) mult = -.3;
                    if (a == 6) mult = -.4;
                    if (a == 7) mult = .1;
                    if (a == 8) mult = .2;
                    if (a == 9) mult = .3;
                    if (a == 10) mult = .4;
                    Mass *= mult + 1;
                    Luminosity *= 2 * mult + 1;
                    Radius = Math.Sqrt(Luminosity) * Math.Pow(5800 / Temperature, 2);
                    Luminosity *= 1.1; // As referenced in the age chart
                }
                else if (spc.size <= SpectralClass.Size.III)
                {
                    int a = starSystem.rng.D10;
                    if (a == 1) { Mass *= 0.3; Luminosity *= 0.3; }
                    if (a == 2) { Mass *= 0.4; Luminosity *= 0.4; }
                    if (a == 3) { Mass *= 0.5; Luminosity *= 0.5; }
                    if (a == 4) { Mass *= 0.6; Luminosity *= 0.6; }
                    if (a == 5) { Mass *= 0.7; Luminosity *= 0.7; }
                    if (a == 6) { Mass *= 0.8; Luminosity *= 0.8; }
                    if (a == 7) { Mass *= 0.9; Luminosity *= 0.9; }
                    if (a == 8) { Mass *= 1.0; Luminosity *= 1.0; }
                    if (a == 9) { Mass *= 1.25; Luminosity *= 1.5; }
                    if (a == 10) { Mass *= 1.5; Luminosity *= 2.0; }
                    Radius = Math.Sqrt(Luminosity) * Math.Pow(5800 / Temperature, 2);
                    Luminosity *= 1.2; // As referenced in the age chart
                }
                else
                {
                    // Appely luminocity due to age
                    if (spc.class_ == SpectralClass.Class_.A)
                        if (spc.specification <= 4) Luminosity *= StarSystem.AgeTable["A0-A4"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                        else Luminosity *= StarSystem.AgeTable["A5-A9"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                    else if (spc.class_ == SpectralClass.Class_.F)
                        if (spc.specification <= 4) Luminosity *= StarSystem.AgeTable["F0-F4"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                        else Luminosity *= StarSystem.AgeTable["F5-F9"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                    else if (spc.class_ == SpectralClass.Class_.G)
                        if (spc.specification <= 4) Luminosity *= StarSystem.AgeTable["G0-G4"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                        else Luminosity *= StarSystem.AgeTable["G5-G9"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                    else if (spc.class_ == SpectralClass.Class_.K)
                        if (spc.specification <= 4) Luminosity *= StarSystem.AgeTable["K0-K4"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                        else Luminosity *= StarSystem.AgeTable["K5-K9"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                    else if (spc.class_ == SpectralClass.Class_.M) Luminosity *= StarSystem.AgeTable["M0-M9"].First(t => t.Value.Item1 == starSystem.Age).Value.Item2;
                }
            }



        }

        Star() { }

        internal void SetElements(OrbitalElements el)
        {
            OrbElements = el;
        }

        internal static Star Combine(Star parent1, Star parent2)
        {
            return new Star() {
                starSystem = parent1.starSystem,
                spc = parent1.spc,
                OrbElements = parent1.OrbElements,
                Luminosity = parent1.Luminosity + parent2.Luminosity,
                Mass = parent1.Mass + parent2.Mass,
                Temperature = parent1.Temperature,
                Radius = parent1.Radius
            };
        }

        /// <summary>
        /// Values are: Luminosity, mass, surface Temperature and radius
        /// </summary>
        public static readonly Dictionary<SpectralClass.Class_, Dictionary<SpectralClass.Size, Dictionary<int, Tuple<double, double, int, double>>>> LumAndMassTable =
            new Dictionary<SpectralClass.Class_, Dictionary<SpectralClass.Size, Dictionary<int, Tuple<double, double, int, double>>>>() {
                {SpectralClass.Class_.B, new Dictionary<SpectralClass.Size, Dictionary<int, Tuple<double, double, int, double>>>() {
                    {SpectralClass.Size.V, new Dictionary<int, Tuple<double, double, int, double>>() {
                        {0, new Tuple<double, double, int, double>(13000, 17.5, 28000,4.9) },
                        {1, new Tuple<double, double, int, double>(7800,15.1,25000,4.8) },
                        {2, new Tuple<double, double, int, double>(4700,13.0,22000,4.8) },
                        {3, new Tuple<double, double, int, double>(2800,11.1,19000,4.6) },
                        {4, new Tuple<double, double, int, double>(1700,8.2,15000,4.7) },
                        {5, new Tuple<double, double, int, double>(1000,8.2,15000,4.7) },
                        {6, new Tuple<double, double, int, double>(600,7.0,14000,4.2) },
                        {7, new Tuple<double, double, int, double>(370,6.0,13000,3.8) },
                        {8, new Tuple<double, double, int, double>(220,5.0,12000,3.5) },
                        {9, new Tuple<double, double, int, double>(130,4.0,11000,3.2) }
                    }
                    }
                }
                },
                {SpectralClass.Class_.A, new Dictionary<SpectralClass.Size, Dictionary<int, Tuple<double, double, int, double>>>() {
                    {SpectralClass.Size.V, new Dictionary<int, Tuple<double, double, int, double>>() {
                        {0, new Tuple<double, double, int, double>(80,3.0,10000,3) },
                        {1, new Tuple<double, double, int, double>(62,2.8,9750,2.6) },
                        {2, new Tuple<double, double, int, double>(48,2.6,9500,2.6) },
                        {3, new Tuple<double, double, int, double>(38, 2.5,9250,2.4) },
                        {4, new Tuple<double, double, int, double>(29,2.3,9000,2.2) },
                        {5, new Tuple<double, double, int, double>(23,2.2,8750,2.1) },
                        {6, new Tuple<double, double, int, double>(18,2.0,8500,2.0) },
                        {7, new Tuple<double, double, int, double>(14,1.9,8500,2.0) },
                        {8, new Tuple<double, double, int, double>(11, 1.8,8000,1.7) },
                        {9, new Tuple<double, double, int, double>(8.2,1.7,7750,1.6) }
                    }
                    },{SpectralClass.Size.IV, new Dictionary<int, Tuple<double, double, int, double>>() {
                        {0, new Tuple<double, double, int, double>(156,6,9700,4.5) },
                        {1, new Tuple<double, double, int, double>(127,5.1,9450,4.2) },
                        {2, new Tuple<double, double, int, double>(102,4.6,9200,4.0) },
                        {3, new Tuple<double, double, int, double>(83,4.3,8950,3.8) },
                        {4, new Tuple<double, double, int, double>(67,4.0,8700,3.6) },
                        {5, new Tuple<double, double, int, double>(54,3.7,8450,3.5) },
                        {6, new Tuple<double, double, int, double>(44,3.4,8200,3.2) },
                        {7, new Tuple<double, double, int, double>(36,3.1,7950,3.2) },
                        {8, new Tuple<double, double, int, double>(29,2.9,7700,3.1) },
                        {9, new Tuple<double, double, int, double>(23,2.7,7500,2.9) }
                    }
                    },{SpectralClass.Size.III, new Dictionary<int, Tuple<double, double, int, double>>() {
                        {0, new Tuple<double, double, int, double>(280,12,9500,6.2) },
                        {1, new Tuple<double, double, int, double>(240,11.5,9250,6.1) },
                        {2, new Tuple<double, double, int, double>(200, 11.0,9000,5.9) },
                        {3, new Tuple<double, double, int, double>(170,10.5,8750,5.7) },
                        {4, new Tuple<double, double, int, double>(140,10,8500,5.6) },
                        {5, new Tuple<double, double, int, double>(120, 9.6, 8250,5.3) },
                        {6, new Tuple<double, double, int, double>(100,9.2,8000,5.3) },
                        {7, new Tuple<double, double, int, double>(87,8.9,7750,5.2) },
                        {8, new Tuple<double, double, int, double>(74, 8.6,7500,5.1) },
                        {9, new Tuple<double, double, int, double>(63,8.3,7350,4.9) }
                    }
                    }
                } }
            };

        public const double SOLAR_MASS = 1.98855e30;
    }
}
