using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class ResourceFinder
    {
        public static IEnumerable<Thing> AllUsableThings
        {
            get
            {
                List<Thing> thingsFound = TradeUtility.AllLaunchableThings.ToList();
                List<IntVec3> cells = new List<IntVec3>();
                ThingDef def = ThingDef.Named("OrbitalTradeBeacon");
                foreach(var beacon in Find.ListerBuildings.AllBuildingsColonistOfClass<Building_OrbitalTradeBeacon>())
                {
                    foreach(var cell in beacon.TradeableCells)
                    {
                        cells.Add(cell);
                    }
                }
                foreach(var cell in cells)
                {
                    foreach(var thing in cell.GetThingList())
                    {
                        if(thing.def.category==ThingCategory.Item && thing.def.defName.Contains("Chunk") && !thingsFound.Contains(thing))
                        {
                            thingsFound.Add(thing);
                        }
                    }
                }
                return thingsFound.AsEnumerable();
            }
        }
    }
}
