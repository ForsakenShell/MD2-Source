using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobDriver_DroidRepair : JobDriver
    {
        private const TargetIndex RepairStationIndex = TargetIndex.A;

        private IRepairable Repairee
        {
            get
            {
                return this.pawn as IRepairable;
            }
        }

        private Building_RepairStation RPS
        {
            get
            {
                return TargetThingA as Building_RepairStation;
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Set what will cause the job to fail
            this.FailOnBurningImmobile(RepairStationIndex);
            this.FailOnDestroyedOrForbidden(RepairStationIndex);
            this.FailOn(delegate
            {
                return Repairee == null || !Repairee.ShouldGetRepairs;
            });

            //Reserve the repair station
            yield return Toils_Reserve.Reserve(RepairStationIndex);
            //Go to the repair station interaction cell
            yield return Toils_Goto.GotoThing(RepairStationIndex, PathEndMode.InteractionCell);
            //Make a new toil that sets the droid to repair mode, then wait until fully repaired
            Toil toil = new Toil();
            toil.FailOnDestroyedOrForbidden(RepairStationIndex);
            toil.FailOn(() =>
                {
                    return Repairee == null || RPS == null || Repairee.Pawn.Position != TargetThingA.InteractionCell || !RPS.IsAvailable(Repairee);
                });

            toil.initAction = () =>
                {
                    //Log.Message("initAction");
                    Repairee.BeingRepaired = true;
                    RPS.RegisterRepairee(Repairee);
                };
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.AddFinishAction(delegate
            {
                //Log.Message("Finish action");
                RPS.DeregisterRepairee(Repairee);
                Repairee.BeingRepaired = false;
            });
            toil.AddEndCondition(() =>
            {
                if (Repairee.ShouldGetRepairs)
                    return JobCondition.Ongoing;
                return JobCondition.Succeeded;
            });
            toil.WithEffect(DefDatabase<EffecterDef>.GetNamed("Repair"), TargetIndex.A);
            toil.WithSustainer(() => { return DefDatabase<SoundDef>.GetNamed("Interact_Repair"); });
            yield return toil;
        }
    }
}
