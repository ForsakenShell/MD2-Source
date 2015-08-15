using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobDriver_ReactivateDroid : JobDriver
    {
        private const TargetIndex DeactivatedDroid = TargetIndex.A;
        private const TargetIndex RPS = TargetIndex.B;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnBurningImmobile(DeactivatedDroid);
            this.FailOnBurningImmobile(RPS);
            this.FailOnDestroyedOrForbidden(DeactivatedDroid);
            this.FailOnDestroyedOrForbidden(RPS);

            DeactivatedDroid droid = TargetThingA as DeactivatedDroid;
            Building_RepairStation rps = TargetThingB as Building_RepairStation;

            //Reserve the item
            yield return Toils_Reserve.Reserve(DeactivatedDroid);
            //Go to the item
            yield return Toils_Goto.GotoThing(DeactivatedDroid, PathEndMode.ClosestTouch);
            //Pick up the item
            yield return Toils_Haul.StartCarryThing(DeactivatedDroid);
            //Go to the rps
            yield return Toils_Goto.GotoThing(RPS, PathEndMode.InteractionCell);
            //Drop the item
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Thing thing;
                if (!toil.actor.carryHands.TryDropCarriedThing(rps.InteractionCell, ThingPlaceMode.Direct, out thing))
                {
                    toil.actor.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
            };
            yield return toil;
            //Add the item to the rps
            toil = new Toil();
            toil.initAction = delegate
            {
                rps.AddDroid(droid);
            };
            yield return toil;
        }
    }
}
