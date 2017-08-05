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
        public OrbitalElements Elements { get; private set; }
        public List<Orbital> Planets { get; private set; }

        public double Luminosity { get { return LumAndMassTable[spc.class_][spc.size][spc.specification].Item1; } }
        public double Mass { get { return LumAndMassTable[spc.class_][spc.size][spc.specification].Item2; } }
        public int Temperature { get { return LumAndMassTable[spc.class_][spc.size][spc.specification].Item3; } }
        public double Radius { get { return LumAndMassTable[spc.class_][spc.size][spc.specification].Item4; } }

        public Star(StarSystem starSystem, SpectralClass spc)
        {
            this.starSystem = starSystem;
            this.spc = spc;
        }


        static Dictionary<SpectralClass.Class_, Dictionary<SpectralClass.Size, Dictionary<int, Tuple<double, double, int, double>>>> LumAndMassTable =
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
