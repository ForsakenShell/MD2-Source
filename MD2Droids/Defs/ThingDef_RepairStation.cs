using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class ThingDef_RepairStation : ThingDef
    {
        public int repairAmount = 1;
        public ThingDef repairThingDef;
        public int repairCostAmount = 1;
        public int ticksPerRepairCycle = 1;
        public bool replacePartsCostsResources;
    }
}
