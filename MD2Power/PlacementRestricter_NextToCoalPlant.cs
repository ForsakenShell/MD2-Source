using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class PlacementRestricter_NextToCoalPlant : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot)
        {
            ThingDef def = ThingDef.Named("MD2CoalBurner");
            foreach (IntVec3 pos in GenAdj.CellsAdjacentCardinal(loc,Rot4.North,IntVec2.One))
            {
                foreach (Thing thing in Find.ThingGrid.ThingsAt(pos))
                {
                    if (thing.def == def)
                    {
                        return true;
                    }
                }
            }
            return "Must be placed next to a coal burner";
        }
    }
}
