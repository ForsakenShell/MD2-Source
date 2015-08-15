using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
	public class WorkGiver_CollectClay : WorkGiver_Scanner//WorkGiver
    {
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            List<IntVec3> cells = new List<IntVec3>();
            foreach(var designation in Find.DesignationManager.DesignationsOfDef(DefDatabase<DesignationDef>.GetNamed("MD2CollectClay")))
            {
                IntVec3 cell = designation.target.Cell;
                if(cell!=null && cell.InBounds()&&pawn.CanReserveAndReach(designation.target,PathEndMode.Touch,Danger.Some))
                {
                    cells.Add(designation.target.Cell);
                }
            }
            return cells.AsEnumerable();
        }

        public override bool ShouldSkip(Pawn pawn)
        {
            return Find.DesignationManager.DesignationsOfDef(DefDatabase<DesignationDef>.GetNamed("MD2CollectClay")).Count() == 0;
        }

        public override bool HasJobOnCell(Pawn pawn, IntVec3 c)
        {
            return pawn.Faction == Faction.OfColony && Find.DesignationManager.DesignationAt(c, DefDatabase<DesignationDef>.GetNamed("MD2CollectClay")) != null && pawn.CanReserveAndReach(c, PathEndMode.Touch, Danger.Some);
        }

        public override Job JobOnCell(Pawn pawn, IntVec3 cell)
        {
            return new Job(DefDatabase<JobDef>.GetNamed("MD2CollectClayJob"), new TargetInfo(cell));
        }
    }
}
