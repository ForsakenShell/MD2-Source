using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class Building_ClayCollector : Building
    {
        public static readonly ThingDef SoftClayDef = DefDatabase<ThingDef>.GetNamed("MD2SoftClay");

        public override void TickRare()
        {
            base.TickRare();
            if (Power != null && !Power.PowerOn) return;

            Thing clay = FindSoftClay;
            if(clay!=null)
            {
                int num = Rand.Range(3,10);
                clay.stackCount += num;
            }
            else
            {
                Thing thing = ThingMaker.MakeThing(SoftClayDef);
                thing.stackCount = Rand.Range(5, 15);
                GenSpawn.Spawn(thing, this.InteractionCell);
            }
        }

        public RimWorld.CompPowerTrader Power
        {
            get
            {
                return base.GetComp<RimWorld.CompPowerTrader>();
            }
        }

        private Thing FindSoftClay
        {
            get
            {
                foreach(Thing thing in Find.ThingGrid.ThingsListAt(this.InteractionCell))
                {
                    if(thing.def==SoftClayDef)
                    {
                        return thing;
                    }
                }
                return null;
            }
        }
    }
}
