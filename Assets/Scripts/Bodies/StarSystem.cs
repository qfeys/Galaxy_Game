using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Bodies
{
    class StarSystem
    {
        public Star Primary { get; private set; }
        public Star Secondary { get; private set; }
        public Star Tertiary { get; private set; }
        /// <summary>
        /// 1 means orbiting primary, 2 means orbiting secondary, 3 means orbiting both
        /// </summary>
        int tertiaryPos = 0;
        public List<Planet> Planets { get; private set; }
        /// <summary>
        /// Age in GY (bilions of years)
        /// </summary>
        public double Age { get; private set; }
        public int Abundance { get; private set; }

        int seed;
        public RNG rng { get; private set; }

        public StarSystem(int seed)
        {
            this.seed = seed;
        }

        /// <summary>
        /// Generate a new star system from this seed. The algoritme that is used is described in this paper: http://sol.trisen.com/downloads/wg.pdf
        /// </summary>
        internal void Generate()
        {
            // Generate primary
            rng = new RNG(seed);
            SpectralClass spcP = RollSpectralClass();
            spcP.specification = rng.D10;
            // Determine system Age
            {
                if (spcP.size <= SpectralClass.Size.IV)       // subgiants & giants
                {
                    SpectralClass comparedSpc = new SpectralClass() {
                        size = SpectralClass.Size.IV,
                        class_ = Star.LumAndMassTable.Aggregate((s, t) => s.Value[SpectralClass.Size.IV].Min(
                            k => Math.Abs(k.Value.Item2 - Star.LumAndMassTable[spcP.class_][spcP.size][spcP.specification].Item2)
                            ) < t.Value[SpectralClass.Size.IV].Min(
                                k => Math.Abs(k.Value.Item2 - Star.LumAndMassTable[spcP.class_][spcP.size][spcP.specification].Item2)
                                ) ? s : t).Key         // New record for the most complex LINQ command ! The above function seaches for the spectral class of size 4 that has the closest  
                                                       // matching mass to our primary
                    };
                    comparedSpc.specification = Star.LumAndMassTable[comparedSpc.class_][SpectralClass.Size.IV].Aggregate((l, r) => l.Value.Item2 < r.Value.Item2 ? l : r).Key;
                    if (comparedSpc.class_ == SpectralClass.Class_.B) Age = 0.1;
                    else if (comparedSpc.class_ == SpectralClass.Class_.A)
                        if (comparedSpc.specification <= 4) Age = 0.6;
                        else Age = 1.3;
                    else if (comparedSpc.class_ == SpectralClass.Class_.F)
                        if (comparedSpc.specification <= 4) Age = 3.2;
                        else Age = 5.6;
                    else if (comparedSpc.class_ == SpectralClass.Class_.G)
                        if (comparedSpc.specification <= 4) Age = 10;
                        else Age = 14;
                    else if (comparedSpc.class_ == SpectralClass.Class_.K)
                        if (comparedSpc.specification <= 4) Age = 23;
                        else Age = 42;
                    else if (comparedSpc.class_ == SpectralClass.Class_.M) Age = 50;
                    else throw new Exception("Invalid compared spectral class. The class we tried to use was: " + comparedSpc.ToString());
                }
                else        // The other stars
                {
                    if (spcP.class_ == SpectralClass.Class_.B) Age = 0.1;
                    else if (spcP.class_ == SpectralClass.Class_.A)
                        if (spcP.specification <= 4) Age = AgeTable["A0-A4"][rng.D10].Item1;
                        else Age = AgeTable["A5-A9"][rng.D10].Item1;
                    else if (spcP.class_ == SpectralClass.Class_.F)
                        if (spcP.specification <= 4) Age = AgeTable["F0-F4"][rng.D10].Item1;
                        else Age = AgeTable["F5-F9"][rng.D10].Item1;
                    else if (spcP.class_ == SpectralClass.Class_.G)
                        if (spcP.specification <= 4) Age = AgeTable["G0-G4"][rng.D10].Item1;
                        else Age = AgeTable["G5-G9"][rng.D10].Item1;
                    else if (spcP.class_ == SpectralClass.Class_.K)
                        if (spcP.specification <= 4) Age = AgeTable["K0-K4"][rng.D10].Item1;
                        else Age = AgeTable["K5-K9"][rng.D10].Item1;
                    else if (spcP.class_ == SpectralClass.Class_.M) Age = AgeTable["M0-M9"][rng.D10].Item1;
                    else if (spcP.class_ == SpectralClass.Class_.WhiteDwarf) Age = rng.D10;
                }
            }

            // Create Other stars
            if(rng.D10 >= 7)    // This is a binary system
            {
                SpectralClass spcS;
                if (rng.D10 <= 2)   // The binary has the came class
                {
                    spcS = new SpectralClass(spcP) {
                        specification = rng.D10
                    };
                    if (spcS.specification < spcP.specification)
                        spcS.specification = spcP.specification;
                }
                else                // A different class
                {
                    spcS = RollSpectralClass();
                    if (spcS.size == SpectralClass.Size.III || (spcS.size == SpectralClass.Size.IV && spcS.class_ == SpectralClass.Class_.K) || spcS.class_ < spcP.class_)
                        spcS = SpectralClass.BrownDwarf;
                    spcS.specification = rng.D10;
                }
                if (rng.D10 >= 7)   // This is a tertiary system
                {
                    SpectralClass spcT;
                    if (rng.D10 <= 2)
                    {
                        spcT = new SpectralClass(spcS) {
                            specification = rng.D10
                        };
                        if (spcT.specification < spcS.specification)
                            spcT.specification = spcS.specification;
                    }
                    else
                    {
                        spcT = RollSpectralClass();
                        if (spcT.size == SpectralClass.Size.III || (spcT.size == SpectralClass.Size.IV && spcT.class_ == SpectralClass.Class_.K) || spcT.class_ < spcS.class_)
                            spcT = SpectralClass.BrownDwarf;
                        spcT.specification = rng.D10;
                    }
                    Tertiary = new Star(this, spcT);
                }
                Secondary = new Star(this, spcS);
            }
            Primary = new Star(this, spcP);

            // Determine System Abundance
            {
                int a = rng.nD10(2) + (int)Age;
                if (a <= 9) Abundance = +2;
                else if (a <= 12) Abundance = +1;
                else if (a <= 18) Abundance = +0;
                else if (a <= 21) Abundance = -1;
                else Abundance = -3;
            }

            // Determine orbits of binary and tertiary stars
            double closestSeperation = 0;
            double furthestSeperation = 0;
            double closestSeperationT = 0;
            double furthestSeperationT = 0;
            if (Secondary != null)
            {
                double meanSeperation;  // In AU
                int b = rng.D10 + (Age > 5 ? +1 : 0) + (Age < 1 ? -1 : 0);
                bool veryClose = false;
                bool close = false;
                if (b <= 3) { meanSeperation = rng.D10 * 0.05; veryClose = true; }
                else if (b <= 6) { meanSeperation = rng.D10 * 0.5; close = true; }
                else if (b <= 8) meanSeperation = rng.D10 * 3;
                else if (b <= 9) meanSeperation = rng.D10 * 20;
                else meanSeperation = rng.D100 * 200;
                double eccentricity;
                int c = rng.D10 + (veryClose ? -2 : 0) + (close ? -1 : 0);
                if (c <= 2) eccentricity = rng.D10 * 0.01;
                else if (c <= 4) eccentricity = rng.D10 * 0.01 + 0.1;
                else if (c <= 6) eccentricity = rng.D10 * 0.01 + 0.2;
                else if (c <= 8) eccentricity = rng.D10 * 0.01 + 0.3;
                else if (c <= 9) eccentricity = rng.D10 * 0.01 + 0.4;
                else eccentricity = rng.D10 * 0.04 + 0.5;

                closestSeperation = meanSeperation * (1 - eccentricity);
                furthestSeperation = meanSeperation * (1 + eccentricity);
                double orbitalPeriod = Math.Sqrt(Math.Pow(meanSeperation, 3) / (Primary.Mass + Secondary.Mass)); // In years?
                double r1 = meanSeperation / (1 + Primary.Mass / Secondary.Mass);
                if(r1 < Primary.Radius)     // The baricentrum is inside the primary. Just set the primary as the center of the system
                {
                    Primary.SetElements(OrbitalElements.Center);
                }
                else // TODO! Test whether the masses used here are correct!, see also binary planets
                {
                    double apparentParentMassPrimary = Math.Pow(r1, 3) * (Primary.Mass + Secondary.Mass) / Math.Pow(meanSeperation, 3);
                    double apparentParentMassSecondary = apparentParentMassPrimary * Math.Pow(Primary.Mass / Secondary.Mass, 3);
                    Primary.SetElements(new OrbitalElements(0, 0, 0, 0, r1, eccentricity, apparentParentMassPrimary * Star.SOLAR_MASS));
                    Secondary.SetElements(new OrbitalElements(0, 0, Math.PI, 0, meanSeperation - r1, eccentricity, apparentParentMassSecondary * Star.SOLAR_MASS));
                }

                if(Tertiary != null)
                {
                    double meanSeperationT;  // In AU
                    double eccentricityT;

                    int j = 0;
                    do
                    {
                        int d = rng.D10;
                        if (d <= 3) tertiaryPos = 1;       // orbit primary
                        else if (d <= 6) tertiaryPos = 2;  // orbit secondary
                        else tertiaryPos = 3;              // orbit both
                        int i = 0;
                        do
                        {
                            int f = rng.D10 + (Age > 5 ? +1 : 0) + (Age < 1 ? -1 : 0);
                            bool veryCloseT = false;
                            bool closeT = false;
                            if (f <= 3) { meanSeperationT = rng.D10 * 0.05; veryCloseT = true; }
                            else if (f <= 6) { meanSeperationT = rng.D10 * 0.5; closeT = true; }
                            else if (f <= 8) meanSeperationT = rng.D10 * 3;
                            else if (f <= 9) meanSeperationT = rng.D10 * 20;
                            else meanSeperationT = rng.D100 * 200;
                            int g = rng.D10 + (veryCloseT ? -2 : 0) + (closeT ? -1 : 0);
                            if (g <= 2) eccentricityT = rng.D10 * 0.01;
                            else if (g <= 4) eccentricityT = rng.D10 * 0.01 + 0.1;
                            else if (g <= 6) eccentricityT = rng.D10 * 0.01 + 0.2;
                            else if (g <= 8) eccentricityT = rng.D10 * 0.01 + 0.3;
                            else if (g <= 9) eccentricityT = rng.D10 * 0.01 + 0.4;
                            else eccentricityT = rng.D10 * 0.04 + 0.5;
                            closestSeperationT = meanSeperationT * (1 - eccentricityT);
                            furthestSeperationT = meanSeperationT * (1 + eccentricityT);
                            i++;
                        } while (tertiaryPos != 3 ? furthestSeperationT * 3 > closestSeperation : furthestSeperation * 3 > closestSeperationT && i < 5);    // Check for invalid orbits
                        j++;
                        if (j > 5) { Tertiary = null; tertiaryPos = 0; break; }      // We tried to much. Just give up.
                    } while     (tertiaryPos != 3 ? furthestSeperationT * 3 > closestSeperation : furthestSeperation * 3 > closestSeperationT);             // Identical check

                    if (Tertiary != null)
                    {
                        double centralMass = tertiaryPos == 1 ? Primary.Mass : tertiaryPos == 2 ? Secondary.Mass : (Primary.Mass + Secondary.Mass);
                        double r3 = meanSeperationT / (1 + Tertiary.Mass / centralMass);    // Distance from tertiary to barycentrum
                        Tertiary.SetElements(new OrbitalElements(0, 0, 0, 0, r3, eccentricityT, centralMass));
                    }
                }
            }

            // Generate Planets
            Planets = new List<Planet>();
            {
                List<double> orbitSizes = GenerateOrbits(Primary);

                // Destroy invalid orbits due to binaries
                orbitSizes.RemoveAll(o => 
                    (Secondary != null && o * 3 > closestSeperation && o < furthestSeperation * 3) ||
                    (tertiaryPos == 1 && o * 3 > closestSeperationT && o < furthestSeperationT * 3) ||
                    (tertiaryPos == 3 && o * 3 > closestSeperationT && o < furthestSeperationT * 3)
                );

                if (Secondary != null)   // Repeat for secondary
                {
                    List<double> orbitSizesS = GenerateOrbits(Secondary);

                    // Destroy invalid orbits due to binaries
                    orbitSizesS.RemoveAll(o => 
                        (o * 3 > closestSeperation) ||
                        (tertiaryPos == 2 && o * 3 > closestSeperationT && o < furthestSeperationT * 3)
                    );

                    if (Tertiary != null) // repeat for tertiary
                    {
                        List<double> orbitSizesT = GenerateOrbits(Secondary);

                        // Destroy invalid orbits due to binaries
                        orbitSizesT.RemoveAll(o => 3 > closestSeperationT);
                        // Generate planets
                        orbitSizesT.ForEach(or => Planets.Add(new Planet(Tertiary, or, or < 4 * Math.Sqrt(Tertiary.Luminosity))));
                    }
                    orbitSizesS.ForEach(or => Planets.Add(new Planet(Secondary, or, or < 4 * Math.Sqrt(Secondary.Luminosity))));
                    orbitSizes.FindAll(or => or > furthestSeperation).ForEach(o =>
                        Planets.Add(new Planet(Star.Combine(Primary, Secondary), o, o < 4 * Math.Sqrt(Primary.Luminosity + Secondary.Luminosity))));
                    orbitSizes.RemoveAll(o => o > furthestSeperation);
                }
                orbitSizes.ForEach(o => Planets.Add(new Planet(Primary, o, o < 4 * Math.Sqrt(Primary.Luminosity))));

                Planets.RemoveAll(p => p.type == Planet.Type.Empty);
                Planets.FindAll(p => p.type == Planet.Type.Astroid_Belt).ForEach(p => p.ResolveAstroidBelt());
                Planets.FindAll(p => p.type == Planet.Type.Interloper).ForEach(p => p.ResolveInterloper());
                Planets.FindAll(p => p.type == Planet.Type.Trojan).ForEach(p => Planets.Add(p.ResolveTrojan()));
                Planets.FindAll(p => p.type == Planet.Type.Double_Planet).ForEach(p => p.ResolveDoublePlanet());
                Planets.FindAll(p => p.type == Planet.Type.Captured).ForEach(p => p.ResolveCaptured());

                Planets.ForEach(p => p.CalculateDay());
                Planets.ForEach(p => p.CalculateMoons());
                Planets.ForEach(p => p.CalculateGeophisicals());
            }

            Simulation.God.Log("Star system " + this + " generated with " + (Secondary == null ? 1 : Tertiary == null ? 2 : 3) + " stars, " + Planets.Count + " planets and "
                + Planets.Sum(p => p.moons.Count) + " moons\n " +
                "First planet: " + Planets[0]);
        }


        /// <summary>
        /// Generates the orbits of planets around a given star. This also removes the orbits inside the shorching radius
        /// for both normal stars and white dwarfs. This does not pay attention to validity because of binaries. In the
        /// document, this is section one/4 steps 1, 2 and 4 and the white dwarf part of step 3. (page 11)
        /// </summary>
        /// <param name="star"></param>
        /// <returns></returns>
        List<double> GenerateOrbits(Star star)
        {
            int a = rng.D10;
            if (star.spc.size == SpectralClass.Size.V)
                if (star.spc.class_ == SpectralClass.Class_.K && star.spc.specification >= 5)
                    a += 1;
                else if (star.spc.class_ == SpectralClass.Class_.M)
                    if (star.spc.specification <= 4) a += 2;
                    else a += 3;
            if (star.spc.class_ == SpectralClass.Class_.BrownDwarf)
                a += 5;
            a += (-Abundance);
            int numberOfOrbits = 0;
            if (a == 1) numberOfOrbits = rng.D10 + 10;
            else if (a <= 5) numberOfOrbits = rng.D10 + 5;
            else if (a <= 7) numberOfOrbits = rng.D10;
            else if (a <= 9) numberOfOrbits = rng.D10 / 2;
            else { numberOfOrbits = 0; return new List<double>(); }

            List<double> orbitSizes = new List<double>();
            orbitSizes.Add(0.05 * Math.Pow(star.Mass, 2) * rng.D10);
            for (int i = 1; i < numberOfOrbits; i++)
            {
                orbitSizes.Add(orbitSizes[i - 1] * (1.1 + rng.D10 * 0.1) + 0.1);
            }
            // Destroy orbits inside schorching radius
            double schorchingRadius = 0.025 * Math.Sqrt(star.Luminosity); // unit: AU
            if (star.spc.class_ == SpectralClass.Class_.WhiteDwarf)
            {
                int b = rng.D10 + (star.Mass > 0.6 ? star.Mass > 0.9 ? +4 : +2 : 0);
                if (b <= 4) schorchingRadius = Math.Max(schorchingRadius, 2);
                else if (b <= 8) schorchingRadius = Math.Max(schorchingRadius, 4);
                else if (b <= 11) schorchingRadius = Math.Max(schorchingRadius, 6);
                else schorchingRadius = Math.Max(schorchingRadius, 10);
            }
            orbitSizes.RemoveAll(o => o < schorchingRadius);
            return orbitSizes;
        }

        private SpectralClass RollSpectralClass()
        {
            int a = rng.D100;
            if (a <= 1)
                if (rng.D10 < 7) return  SpectralClass.AV;
                else return  SpectralClass.AIV;
            else if (a <= 4)
                if (rng.D10 < 9) return  SpectralClass.FV;
                else return  SpectralClass.FIV;
            else if (a <= 12)
                if (rng.D10 < 10) return  SpectralClass.GV;
                else return  SpectralClass.GIV;
            else if (a <= 26) return  SpectralClass.KV;
            else if (a <= 36) return  SpectralClass.WhiteDwarf;
            else if (a <= 85) return  SpectralClass.MV;
            else if (a <= 98) return  SpectralClass.BrownDwarf;
            else if (a <= 99)
            {
                int b = rng.D10;
                if (b <= 1) return  SpectralClass.FIII;
                else if (b <= 2) return  SpectralClass.GIII;
                else if (b <= 7) return  SpectralClass.KIII;
                else return  SpectralClass.KIV;
            }
            else if (a <= 100)
            {
                throw new NotImplementedException("Trying to make a special star");
            }
            else throw new ArgumentOutOfRangeException("The RNG gave us a number bigger than 100.");
        }

        public override string ToString()
        {
            return Primary.ToString();
        }

        /// <summary>
        /// The first number is the age (GY), the second is the change in luminocity
        /// </summary>
        static public readonly Dictionary<string, Dictionary<int, Tuple<double, double>>> AgeTable = new Dictionary<string, Dictionary<int, Tuple<double, double>>>() {
            {"A0-A4",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(0.1,0) },
                {2, new Tuple<double, double>(0.1,0) },
                {3, new Tuple<double, double>(0.2,0) },
                {4, new Tuple<double, double>(0.2,0) },
                {5, new Tuple<double, double>(0.3,0) },
                {6, new Tuple<double, double>(0.3,0) },
                {7, new Tuple<double, double>(0.4,0) },
                {8, new Tuple<double, double>(0.4,0) },
                {9, new Tuple<double, double>(0.5,0) },
                {10, new Tuple<double, double>(0.6,0) }
            }
            },
            {"A5-A9",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(0.2,-.2) },
                {2, new Tuple<double, double>(0.4,-.2) },
                {3, new Tuple<double, double>(0.5,-.1) },
                {4, new Tuple<double, double>(0.6,-.1) },
                {5, new Tuple<double, double>(0.7, .0) },
                {6, new Tuple<double, double>(0.8, .0) },
                {7, new Tuple<double, double>(0.9, .1) },
                {8, new Tuple<double, double>(1.0, .1) },
                {9, new Tuple<double, double>(1.1, .2) },
                {10, new Tuple<double, double>(1.2,.2) }
            }
            },
            {"F0-F4",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(0.3,-.4) },
                {2, new Tuple<double, double>(0.6,-.3) },
                {3, new Tuple<double, double>(1.0,-.2) },
                {4, new Tuple<double, double>(1.3,-.1) },
                {5, new Tuple<double, double>(1.6, .0) },
                {6, new Tuple<double, double>(2.0, .1) },
                {7, new Tuple<double, double>(2.3, .2) },
                {8, new Tuple<double, double>(2.6, .3) },
                {9, new Tuple<double, double>(2.9, .4) },
                {10,new Tuple<double, double>(3.2, .5) }
            }
            },
            {"F5-F9",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(0.5,-.4) },
                {2, new Tuple<double, double>(1.0,-.3) },
                {3, new Tuple<double, double>(1.5,-.2) },
                {4, new Tuple<double, double>(2.0,-.1) },
                {5, new Tuple<double, double>(2.5, .0) },
                {6, new Tuple<double, double>(3.0, .1) },
                {7, new Tuple<double, double>(3.5, .2) },
                {8, new Tuple<double, double>(4.0, .3) },
                {9, new Tuple<double, double>(4.5, .4) },
                {10,new Tuple<double, double>(5.0, .5) }
            }
            },
            {"G0-G4",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(1.0,-.4) },
                {2, new Tuple<double, double>(2.0,-.3) },
                {3, new Tuple<double, double>(3.0,-.2) },
                {4, new Tuple<double, double>(4.0,-.1) },
                {5, new Tuple<double, double>(5.0, .0) },
                {6, new Tuple<double, double>(6.0, .1) },
                {7, new Tuple<double, double>(7.0, .2) },
                {8, new Tuple<double, double>(8.0, .3) },
                {9, new Tuple<double, double>(9.0, .4) },
                {10,new Tuple<double, double>(10.0,.5) }
            }
            },
            {"G5-G9",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(1.0,-.4) },
                {2, new Tuple<double, double>(2.0,-.3) },
                {3, new Tuple<double, double>(3.0,-.2) },
                {4, new Tuple<double, double>(4.0,-.1) },
                {5, new Tuple<double, double>(5.0, .0) },
                {6, new Tuple<double, double>(6.0, .0) },
                {7, new Tuple<double, double>(7.0, .0) },
                {8, new Tuple<double, double>(8.0, .1) },
                {9, new Tuple<double, double>(9.0, .2) },
                {10,new Tuple<double, double>(10.0,.3) }
            }
            },
            {"K0-K4",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(1.0,-.2) },
                {2, new Tuple<double, double>(2.0,-.15) },
                {3, new Tuple<double, double>(3.0,-.1) },
                {4, new Tuple<double, double>(4.0,-.05) },
                {5, new Tuple<double, double>(5.0, .0) },
                {6, new Tuple<double, double>(6.0, .0) },
                {7, new Tuple<double, double>(7.0, .0) },
                {8, new Tuple<double, double>(8.0, .0) },
                {9, new Tuple<double, double>(9.0, .0) },
                {10,new Tuple<double, double>(10.0,.05) }
            }
            },
            {"K5-K9",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(1.0,-.1) },
                {2, new Tuple<double, double>(2.0,-.5) },
                {3, new Tuple<double, double>(3.0,-.0) },
                {4, new Tuple<double, double>(4.0,-.0) },
                {5, new Tuple<double, double>(5.0, .0) },
                {6, new Tuple<double, double>(6.0, .0) },
                {7, new Tuple<double, double>(7.0, .0) },
                {8, new Tuple<double, double>(8.0, .0) },
                {9, new Tuple<double, double>(9.0, .0) },
                {10,new Tuple<double, double>(10.0,.0) }
            }
            },
            {"M0-M9",new Dictionary<int, Tuple<double, double>>(){
                {1, new Tuple<double, double>(1.0, .1) },
                {2, new Tuple<double, double>(2.0, .0) },
                {3, new Tuple<double, double>(3.0,-.0) },
                {4, new Tuple<double, double>(4.0,-.0) },
                {5, new Tuple<double, double>(5.0, .0) },
                {6, new Tuple<double, double>(6.0, .0) },
                {7, new Tuple<double, double>(7.0, .0) },
                {8, new Tuple<double, double>(8.0, .0) },
                {9, new Tuple<double, double>(9.0, .0) },
                {10,new Tuple<double, double>(10.0,.0) }
            }
            },
        };

        /// <summary>
        /// One astronomical unit in meters
        /// </summary>
        public const ulong AU = 149597871;   // 149'597'870,700 kilometers
        public const ulong SOL_SIZE = 40 * AU;
        


        internal Planet RandLivableWorld()
        {
            foreach(Planet o in Planets)
            {
                //if (o.GetType() == typeof(Rock) && ((Rock)o).Habitable)
                    return o;
            }
            UnityEngine.Debug.Log("No livable world detected.");
            return null;
        }
        
    }

    class SpectralClass
    {
        public enum Class_ { O, B, A, F, G, K, WhiteDwarf, M, BrownDwarf }
        public enum Size { Ia, Ib, II, III, IV, V, VII }
        public Class_ class_;
        public Size size;
        public int specification;

        public SpectralClass() { }

        public SpectralClass(SpectralClass clone)
        {
            class_ = clone.class_; size = clone.size; specification = clone.specification;
        }

        public override string ToString()
        {
            return "" + class_ + specification + " " + size;
        }

        public static SpectralClass BV { get { return new SpectralClass() { class_ = Class_.B, size = Size.V }; } }
        public static SpectralClass AV { get { return new SpectralClass() { class_ = Class_.A, size = Size.V }; } }
        public static SpectralClass FV { get { return new SpectralClass() { class_ = Class_.F, size = Size.V }; } }
        public static SpectralClass GV { get { return new SpectralClass() { class_ = Class_.G, size = Size.V }; } }
        public static SpectralClass KV { get { return new SpectralClass() { class_ = Class_.K, size = Size.V }; } }
        public static SpectralClass MV { get { return new SpectralClass() { class_ = Class_.M, size = Size.V }; } }
        public static SpectralClass AIV { get { return new SpectralClass() { class_ = Class_.A, size = Size.IV }; } }
        public static SpectralClass FIV { get { return new SpectralClass() { class_ = Class_.F, size = Size.IV }; } }
        public static SpectralClass GIV { get { return new SpectralClass() { class_ = Class_.G, size = Size.IV }; } }
        public static SpectralClass KIV { get { return new SpectralClass() { class_ = Class_.K, size = Size.IV }; } }
        public static SpectralClass AIII { get { return new SpectralClass() { class_ = Class_.A, size = Size.III }; } }
        public static SpectralClass FIII { get { return new SpectralClass() { class_ = Class_.F, size = Size.III }; } }
        public static SpectralClass GIII { get { return new SpectralClass() { class_ = Class_.G, size = Size.III }; } }
        public static SpectralClass KIII { get { return new SpectralClass() { class_ = Class_.K, size = Size.III }; } }
        public static SpectralClass MIII { get { return new SpectralClass() { class_ = Class_.M, size = Size.III }; } }
        public static SpectralClass WhiteDwarf { get { return new SpectralClass() { class_ = Class_.WhiteDwarf, size = Size.VII }; } }
        public static SpectralClass BrownDwarf { get { return new SpectralClass() { class_ = Class_.BrownDwarf, size = Size.VII }; } }
    }
}
