using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Empires.Production;
namespace Assets.Scripts.Bodies
{
    class MiningPile
    {
        public List<Tuple<Stockpile.ResourceType, double[]>> pile { get; private set; }

        public MiningPile(Planet parent)
        {
            pile = new List<Tuple<Stockpile.ResourceType, double[]>>();

            foreach (var rt in Stockpile.ResourceType.ResourceTypes.FindAll(rt=>rt.minable == true))
            {
                pile.Add(new Tuple<Stockpile.ResourceType, double[]>(rt, new double[3] { 1e9, 1e10, 1e12 }));
            }
        }

        MiningPile()
        {
            pile = new List<Tuple<Stockpile.ResourceType, double[]>>();

            foreach (var rt in Stockpile.ResourceType.ResourceTypes.FindAll(rt => rt.minable == true))
            {
                pile.Add(new Tuple<Stockpile.ResourceType, double[]>(rt, new double[3] { 1e9, 1e10, 1e12 }));
            }
        }

        public static MiningPile Empty { get { return new MiningPile(); } }
    }
}
