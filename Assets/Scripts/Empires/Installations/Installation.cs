using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Empires.Installations
{
    /// <summary>
    /// All installations in population centers or in space. NOTE: for installations that are free floating, use a child of this.
    /// </summary>
    class Installation
    {
        public string name;
        public double costWork;
        public Dictionary<Production.Stockpile.ResourceType, double> costResources;

        public List<Modifier> modefiers { get; private set; }

        public Installation(string name, double costWork,
            Dictionary<Production.Stockpile.ResourceType, double> costResources, List<Modifier> modefiers)
        {
            this.name = name;
            this.costWork = costWork;
            this.costResources = costResources;
            this.modefiers = modefiers;
        }

        static List<Installation> installationList;

        static public void SetInstallationList(List<ModParser.Item> itemList)
        {
            if (installationList == null)
            {
                UnityEngine.Debug.Log("Installation List set with " + itemList.Count + " installations.");
            }
            else
            {
                UnityEngine.Debug.LogError("Installation List reset with " + itemList.Count + " installations.");
            }
            installationList = itemList.ConvertAll(i => Installation.Interpret(i));
        }

        private static Installation Interpret(ModParser.Item i)
        {
            throw new NotImplementedException();
        }

        internal static List<ModParser.Signature> Signature()
        {
            throw new NotImplementedException();
        }
    }
}
