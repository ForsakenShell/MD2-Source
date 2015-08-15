using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class WorkGiver_DroidGotoRepairStation : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.InteractionCell;
            }
        }

        public override bool ShouldSkip(Pawn pawn)
        {
            IRepairable droid = pawn as IRepairable;
            return droid != null && !droid.BeingRepaired && !droid.ShouldGetRepairs;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t)
        {
            Building_RepairStation rps = t as Building_RepairStation;
            IRepairable repairee = pawn as IRepairable;
            return rps != null && repairee != null && rps.IsAvailable(repairee) && pawn.CanReserveAndReach(rps, PathEndMode, Danger.Some, 1);
        }

        public override Job JobOnThing(Pawn pawn, Thing t)
        {
            return new Job(DefDatabase<JobDef>.GetNamed("MD2DroidGotoRepairStation"), t);
        }
    }
}
