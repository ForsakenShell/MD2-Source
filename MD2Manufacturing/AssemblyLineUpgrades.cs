using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class AssemblyLineUpgrades
    {
        public static IEnumerable<AssemblyLineUpgradeDef> AllUpgrades
        {
            get
            {
                return DefDatabase<AssemblyLineUpgradeDef>.AllDefs;
            }
        }

        public static Dictionary<string, AssemblyLineUpgradeDef> AllUpgradesDictionary
        {
            get
            {
                Dictionary<string, AssemblyLineUpgradeDef> dict = new Dictionary<string, AssemblyLineUpgradeDef>();
                foreach (AssemblyLineUpgradeDef def in AssemblyLineUpgrades.AllUpgrades)
                {
                    dict.Add(def.defName, def);
                }
                return dict;
            }
        }
        public static IEnumerable<AssemblyLineUpgradeDef> AvailableUpgrades(Dictionary<string, AssemblyLineUpgradeDef> installedUpgrades)
        {
            foreach (AssemblyLineUpgradeDef def in AssemblyLineUpgrades.AllUpgrades)
            {
                if (!(installedUpgrades.ContainsKey(def.defName)))
                {
                    yield return def;
                }
            }
        }
    }
}
