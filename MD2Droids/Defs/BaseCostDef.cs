using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class BaseCostDef : ThingDef
    {
        public List<ThingCount> baseCosts = new List<ThingCount>();

        public List<ThingCount> explosionCost;
        public List<ThingCount> passionCost;
        public List<ThingCount> skillsCost;
    }
}
