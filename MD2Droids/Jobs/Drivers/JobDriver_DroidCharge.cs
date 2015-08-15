using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobDriver_DroidCharge : JobDriver
    {

        protected override IEnumerable<Toil> MakeNewToils()
        {

            ICharge chargee = (ICharge)this.pawn;
            CompDroidCharger charger =TargetThingA.TryGetComp<CompDroidCharger>();
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            Toil goToPad = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            goToPad.AddFailCondition(() => { return !charger.IsAvailable(chargee); });
            yield return goToPad;
            Toil charge = new Toil();
            charge.initAction = () =>
                {
                    if (charger.parent.InteractionCell != chargee.Parent.Position)
                    {
                        pawn.jobs.EndCurrentJob(JobCondition.Errored);
                    }
                    else
                    {
                        charger.BeginCharge(chargee);
                        chargee.ShouldUsePower = false;
                    }
                };
            charge.defaultCompleteMode = ToilCompleteMode.Never;
            charge.AddFailCondition(() =>
                { return !charger.IsAvailable(chargee); });
            charge.AddEndCondition(() =>
                {
                    if (!chargee.DesiresCharge)
                    {
                        return JobCondition.Succeeded;
                    }
                    return JobCondition.Ongoing;
                });
            charge.AddFinishAction(() =>
                {
                    charger.EndCharge();
                    chargee.ShouldUsePower = true;
                });
            yield return charge;
        }
    }
}
