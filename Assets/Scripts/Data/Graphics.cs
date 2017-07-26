using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Data
{
    static class Graphics
    {


        public static Font GetStandardFont()
        {
            return Resources.Load<Font>(@"Fonts\Orbitron\orbitron-light");
        }
    }
}
