using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobDriver_DroidCremate : JobDriver
    {
        private const TargetIndex CorpseIndex = TargetIndex.A;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Set what will cause the job to fail:
            this.FailOnDestroyedOrForbidden(CorpseIndex);
            this.FailOnBurningImmobile(CorpseIndex);
            this.FailOn(() => !(pawn is Droid));

            //Reserve the corpse
            yield return Toils_Reserve.Reserve(CorpseIndex);
            //Go to the corpse
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);
            Toil toil = new Toil();
            toil.initAction = () =>
                {
                    //Check if the pawn is set to strip bodies, if yes then strip it, otherwise skip this step
                    Droid droid = (Droid)pawn;
                    CremationWorker worker = droid.work.specialist.GetWorker<CremationWorker>();
                    if (worker.StripBodies)
                    {
                        Corpse corpse = (Corpse)TargetThingA;
                        if (corpse.AnythingToStrip())
                            corpse.Strip();
                    }
                };
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 300;
            toil.WithEffect(() => DefDatabase<EffecterDef>.GetNamed("Cremate"), CorpseIndex);
            toil.WithSustainer(() => DefDatabase<SoundDef>.GetNamed("Recipe_Cremate"));
            toil.AddFinishAction(() => TargetA.Thing.Destroy());
            toil.FailOnBurningImmobile(CorpseIndex);
            toil.FailOnDestroyedOrForbidden(CorpseIndex);
            toil.AddEndCondition(() => this.ticksLeftThisToil <= 0 ? JobCondition.Succeeded : JobCondition.Ongoing);
            yield return toil;
        }
    }
}
