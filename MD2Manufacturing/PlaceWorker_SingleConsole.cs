using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class PlaceWorker_SingleConsole : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot)
        {
            ThingDef def = checkingDef as ThingDef;
            int num = Find.ListerBuildings.allBuildingsColonist.Where((Building b)=>b.def==def).Count();
            num += Find.ListerThings.ThingsOfDef(def.blueprintDef).Count;
            if(num==0)
            {
                return AcceptanceReport.WasAccepted;
            }
            return "MaximumOneReportString".Translate();
        }
    }
}
