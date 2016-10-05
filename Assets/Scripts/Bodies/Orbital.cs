using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    abstract class Orbital
    {
        Orbital Parent;
        List<Orbital> Childeren;
        ulong mass;
        OrbitalElements elements;
    }

    /// <summary>
    /// orbital elements. All angles in radian
    /// </summary>
    struct OrbitalElements
    {
        double LAN; // longitude of ascending node
        double i;   // inclination
        double AOP; // Argument of periapsis
        double MAaE;// Mean anomaly at epoch
        ulong SMA;  // Semi-major axis
        double e;   // exentricity
    }
}
