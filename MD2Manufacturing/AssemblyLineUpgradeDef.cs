using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class AssemblyLineUpgradeDef : Def
    {
        public UpgradeTarget PropertyToAffect;
        public float PercentageDecrease;
        public string Description;
        public List<ListItem> RequiredMaterials;
        public List<AssemblyLineUpgradeDef> Prerequisites;
        public bool alwaysDisplay = false;
        public int workTicksAmount = 600;
    }
}
