﻿using System;
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
            Count = Changeling.Create(1e9);
            FreeIndustrialPopulation = Count * 0.01;
        }
    }
}
