using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Bodies
{
    static class Galaxy
    {
        public static List<StarSystem> systems;

        public static void Create(int size, int seed)
        {
            Random rand = new Random(seed);
            systems = new List<StarSystem>();
            for (int i = 0; i < size; i++)
            {
                systems.Add(new StarSystem(rand.Next()));
            }
            systems.ForEach(sys => sys.Generate());
            //Parallel.ForEach(systems, sys => sys.Generate());
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
