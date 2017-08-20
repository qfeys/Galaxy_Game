using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Bodies
{
    static class Galaxy
    {
        public static List<SystemContainer> systems;

        public static void Create(int size, int seed)
        {
            Random rand = new Random(seed);
            systems = new List<SystemContainer>();
            List<Vect3> positions = GeneratePositions(size, 1.5, 4, 10, rand);
            for (int i = 0; i < size; i++)
            {
                systems.Add(new SystemContainer(new StarSystem(rand.Next()), positions[i]));
            }
            systems.ForEach(sys => sys.sys.Generate());
            //Parallel.ForEach(systems, sys => sys.Item1.Generate());
        }

        /// <summary>
        /// Poisson-disc sampling
        /// </summary>
        /// <param name="numberOfPoints"></param>
        /// <param name="minDis"></param>
        /// <param name="maxDis"></param>
        /// <param name="iter_per_point"></param>
        /// <param name="rand"></param>
        /// <returns></returns>
        private static List<Vect3> GeneratePositions(int numberOfPoints, double minDis, double maxDis, int iter_per_point, Random rand)
        {
            List<Tuple<Vect3,bool>> points = new List<Tuple<Vect3, bool>>(numberOfPoints);
            points.Add(new Tuple<Vect3, bool>(Vect3.Zero, true));
            RNG rng = new RNG(rand.Next());

            for (int i = 0; i < numberOfPoints; i++)
            {
                while (true) {
                    List<Vect3> validPoints = points.FindAll(tpl => tpl.Item2).ConvertAll(tpl => tpl.Item1);
                    if (validPoints.Count == 0)
                    {
                        UnityEngine.Debug.LogError("Not all points could be placed. Points placed: " + validPoints.Count);
                        break;
                    }
                    Vect3 central = validPoints[rand.Next(validPoints.Count)];
                    for (int j = 0; j < iter_per_point; j++)
                    {
                        Vect3 newPoint = new Vect3(rand.NextDouble() * (maxDis - minDis) + minDis, rng);
                        if (points.All(tpl => Vect3.Distance(tpl.Item1, newPoint) > minDis))
                        {
                            points.Add(new Tuple<Vect3, bool>(newPoint, true));
                            goto Found;
                        }
                    }
                    points.Find(tpl => tpl.Item1 == central).Item2 = false;
                    if (i > numberOfPoints * 2)
                        throw new Exception("Too many iterations. Infinite loop detected.");
                }
                break;  // At this point we left because we coudn't place a point, thus break the for loop immediatly
                Found:;

            }
            return points.ConvertAll(tpl => tpl.Item1);
        }

        const double MILKY_WAY_MASS = Star.SOLAR_MASS * 1e12;

        public class Vect3
        {
            double x, y, z;

            public Vect3(double radius, RNG rng)
            {
                double th = rng.Circle;
                double ga = rng.Circle;
                x = radius * Math.Cos(th) * Math.Cos(ga);
                y = radius * Math.Sin(th) * Math.Cos(ga);
                z = radius * Math.Sin(ga);
            }

            public Vect3(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static double Distance(Vect3 v1, Vect3 v2)
            {
                return Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2) + Math.Pow(v1.z - v2.z, 2));
            }

            public static implicit operator UnityEngine.Vector3(Vect3 v)
            {
                return new UnityEngine.Vector3((float)v.x,
                                               (float)v.y,
                                               (float)v.z);
            }

            public static Vect3 Zero { get { return new Vect3(0, 0, 0); } }
        }

        public class SystemContainer
        {
            public StarSystem sys;
            Vect3 pos;

            public SystemContainer(StarSystem sys, Vect3 pos)
            {
                this.sys = sys; this.pos = pos;
            }

            public static implicit operator UnityEngine.Vector3(SystemContainer sysCon)
            {
                return sysCon.pos;
            }

            public static implicit operator StarSystem(SystemContainer sysCon)
            {
                return sysCon.sys;
            }

            public override string ToString()
            {
                UnityEngine.Vector3 p = pos;
                return "(" + p.x.ToString("0") + "," + p.y.ToString("0") + "," + p.z.ToString("0") + ")" + sys.ToString();
            }
        }
    }
}
