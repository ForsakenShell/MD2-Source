using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class WorkGiver_ReactivateDroid : WorkGiver_Scanner
    {
        private readonly ThingDef DeactivatedDroidDef = DefDatabase<ThingDef>.GetNamed("MD2DeactivatedDroid");
        private readonly JobDef ReactivateDroidJobDef = DefDatabase<JobDef>.GetNamed("MD2ReactivateDroid");

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(DeactivatedDroidDef);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.InteractionCell;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t)
        {
            DeactivatedDroid droid = t as DeactivatedDroid;
            return droid != null && ListerDroids.AllRepairStations.Any((Building_RepairStation rps) => rps.IsAvailableForReactivation) && pawn.CanReserveAndReach(droid, PathEndMode, Danger.Some, 1);
        }

        public override Job JobOnThing(Pawn pawn, Thing t)
        {
            Building_RepairStation rps = ListerDroids.ClosestRepairStationFor(pawn,t);
            Job job = new Job(ReactivateDroidJobDef, t, rps);
            job.maxNumToCarry = 1;
            return job;
        }
    }
}
