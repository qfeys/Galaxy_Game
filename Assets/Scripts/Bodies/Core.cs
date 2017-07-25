using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Core : Orbital
    {
        public static Core Instance { get; private set; }

        public static void Create(int size, int seed)
        {
            if (Instance != null) throw new Exception("A core has already been created");
            Instance = new Core();
            Random rand = new Random(seed);
            Instance.Generate(sizeMap[size], rand);
        }

        public Core() : base(null, 0, new OrbitalElements())
        {

        }

        public override void Generate(double mass, Random rand)
        {
            StarSystem ss = new StarSystem(this, new OrbitalElements());
            ss.Generate(Star.SOLAR_MASS, rand);
        }

        const double MILKY_WAY_MASS = Star.SOLAR_MASS * 1e12;

        static readonly Dictionary<int, double> sizeMap = new Dictionary<int, double> {
            {1, MILKY_WAY_MASS * 1e-9 },
            {2, MILKY_WAY_MASS * 1e-8 },
            {3, MILKY_WAY_MASS * 1e-7 },
            {4, MILKY_WAY_MASS * 1e-6 },
            {5, MILKY_WAY_MASS * 1e-5 },
        };  
    }
}
