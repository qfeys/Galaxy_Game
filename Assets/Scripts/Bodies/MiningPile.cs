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
        List<Tuple<Stockpile.ResourceType, double[]>> pile;

        public MiningPile(Planet parent)
        {
            pile = new List<Tuple<Stockpile.ResourceType, double[]>>();

            foreach (var rt in Stockpile.ResourceType.ResourceTypes.FindAll(rt=>rt.minable == true))
            {
                pile.Add(new Tuple<Stockpile.ResourceType, double[]>(rt, new double[3] { 1e9, 1e10, 1e12 }));
            }
        }
    }
}
