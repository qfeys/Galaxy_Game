using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Empires.Demographics
{
    class Demographics
    {
        /// <summary>
        /// The number of people this population has
        /// </summary>
        public Changeling Count { get; private set; }
        public Changeling FreeIndustrialPopulation { get; private set; }
        public Changeling YearlyGrowth { get; private set; }

        double poverty; // part of people living in poverty
        double inequality; // gini-index - lower is better
        double happiness; // negative is bad
        // species
        // culture

        public Demographics()
        {
            poverty = 0.1;
            inequality = 0.4;
            happiness = 0;
            // 1e9 * (1 + {0}) ^ ((T-t) / (3600*24*356))
            // 1e9 (1 {0} SUM)(T t diff num div) POW MULT
            YearlyGrowth = Changeling.Create(0.04);
            Count = Changeling.Create("1e9 1 {0} SUM T now diff " + (3600 * 24 * 256) + " div POW MULT", YearlyGrowth);
            FreeIndustrialPopulation = Count * 0.01;
        }
    }
}
