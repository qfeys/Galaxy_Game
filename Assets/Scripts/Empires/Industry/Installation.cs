using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Data;

namespace Assets.Scripts.Empires.Industry
{
    /// <summary>
    /// All installations in population centers or in space. NOTE: for installations that are free floating, use a child of this.
    /// </summary>
    class Installation
    {
        public string name;
        public double costWork;
        public Stockpile.ResourceBill costResources;
        public List<Technology.Technology.Prerequisite> Prerequisites { get; private set; }

        public List<Modifier> Modefiers { get; private set; }

        public static List<Installation> installationList { get; private set; }

        public Installation(string name, double costWork,
            Stockpile.ResourceBill costResources, List<Modifier> modifiers)
        {
            this.name = name;
            this.costWork = costWork;
            this.costResources = costResources;
            this.Modefiers = modifiers;
        }

        Installation() { }

#region parsing

        static public void LoadInstallationList()
        {
            ModParser.Item modItem = ModParser.RetriveMasterItem("installations");
            installationList = modItem.GetChilderen().ConvertAll(i => Interpret(i));
        }

        private static Installation Interpret(ModParser.Item i)
        {
            Installation inst = new Installation() {
                name = i.name,
                costWork = i.GetNumber("cost_work"),
                costResources = new Stockpile.ResourceBill(),
                Modefiers = new List<Modifier>(),
                Prerequisites = new List<Technology.Technology.Prerequisite>()
            };
            ModParser.Item rsrcs = i.GetItem("cost_resources");
            if (rsrcs != null)
            {
                rsrcs.GetChilderen().ForEach(r => inst.costResources.Add(r.name, r.GetNumber()));
            }
            ModParser.Item mods = i.GetItem("modifiers");
            if (mods != null)
            {
                mods.GetChilderen().ForEach(m => inst.Modefiers.Add(new Modifier(m.name, m.GetNumber())));
            }
            ModParser.Item prerqs = i.GetItem("prerequisites");
            if (prerqs != null)
            {
                prerqs.GetChilderen().ForEach(p =>
                    inst.Prerequisites.Add(new Technology.Technology.Prerequisite(p.name, p.GetNumber("min"), p.GetNumber("max")))
                               );
            }
            return inst;
        }

#endregion

        public static Installation GetCopy(string name)
        {
            if(installationList.Any(i=> i.name == name))
            {
                return installationList.Find(i => i.name == name);
            }
            throw new ArgumentException("The installation " + name + " is not in the list.");
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            return name.Equals(((Installation)obj).name);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>
        /// Investigates if this building is valid for this population
        /// </summary>
        /// <param name="pop"></param>
        /// <returns></returns>
        internal bool IsValid(Population pop)
        {
            return Prerequisites.All(p => 
                    pop.Empire.Academy.Unlocks.Any(ti => ti.parent == p.Tech)
                    );
        }
    }
}
