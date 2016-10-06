using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class Core : Orbital
    {
        public static Core instance { get; private set; }

        public static void Create(int size, int seed)
        {
            if (instance != null) throw new Exception("A core has already been created");
            instance = new Core();
            Random rand = new Random(seed);
            instance.Generate(SizeMap[size], rand);
        }

        public Core() : base(null, 0, new OrbitalElements())
        {

        }

        public override void Generate(double mass, Random rand)
        {
            StarSystem ss = new StarSystem(this, new OrbitalElements());
            ss.Generate(Star.SolarMass, rand);
        }

        const double MilkyWayMass = Star.SolarMass * 1e12;

        static readonly Dictionary<int, double> SizeMap = new Dictionary<int, double> {
            {1, MilkyWayMass * 1e-9 },
            {2, MilkyWayMass * 1e-8 },
            {3, MilkyWayMass * 1e-7 },
            {4, MilkyWayMass * 1e-6 },
            {5, MilkyWayMass * 1e-5 },
        };  
    }
}
