using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MD2
{
    public class CoalFeeder : Building_Storage
    {
        public Thing ContainedFuel
        {
            get
            {
                ThingDef def = ThingDef.Named("MD2Coal");
                ThingDef def2 = ThingDef.Named("MD2Charcoal");
                Thing thing;
                thing= Find.ThingGrid.ThingAt(this.Position, def);
                if (thing != null)
                    return thing;
                else thing = Find.ThingGrid.ThingAt(this.Position, def2);
                return thing;
            }
        }

        public bool HasFuelItem
        {
            get
            {
                return ContainedFuel != null;
            }
        }
    }
}
