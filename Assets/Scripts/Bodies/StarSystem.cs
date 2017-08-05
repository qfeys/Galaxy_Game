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
        public List<Orbital> Planets { get; private set; }
        public double Mass { get; private set; }
        public OrbitalElements Elements { get; private set; }
        ulong id;
        static ulong idCounter = 0;

        int seed;
        public RNG rng { get; private set; }

        public StarSystem(int seed)
        {
            this.seed = seed;
        }

        internal void Generate()
        {
            // Generate stars
            rng = new RNG(seed);
            SpectralClass spcP = RollSpectralClass();
            spcP.specification = rng.D10;
            if(rng.D10 >= 7)    // This is a binary system
            {
                SpectralClass spcS;
                if (rng.D10 <= 2)
                {
                    spcS = new SpectralClass(spcP) {
                        specification = rng.D10
                    };
                    if (spcS.specification < spcP.specification)
                        spcS.specification = spcP.specification;
                }
                else
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

            // Generate Planets

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
        
        public const ulong AU = 149597870700;
        public const ulong SOL_SIZE = 40 * AU;
        


        internal Orbital RandLivableWorld()
        {
            foreach(Orbital o in Childeren)
            {
                if (o.GetType() == typeof(Rock) && ((Rock)o).Habitable)
                    return o;
            }
            UnityEngine.Debug.Log("No livable world detected.");
            return null;
        }
        
    }

    class SpectralClass
    {
        public enum Class_ { O, B, A, F, G, K, M, WhiteDwarf, BrownDwarf }
        public enum Size { Ia, Ib, II, III, IV, V, VII }
        public Class_ class_;
        public Size size;
        public int specification;

        SpectralClass() { }

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
        public static SpectralClass BrownDwarf { get { return new SpectralClass() { class_ = Class_.BrownDwarf }; } }
    }
}
