using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobDriver_CollectClay: JobDriver
    {
        private const TargetIndex CellInd = TargetIndex.A;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnBurningImmobile(CellInd);

            yield return Toils_Reserve.Reserve(CellInd);

            yield return Toils_Goto.GotoCell(CellInd, PathEndMode.Touch);

            yield return Toils_General.Wait(400);

            yield return Toils_MD2General.MakeAndSpawnThingRandomRange(ThingDef.Named("MD2SoftClay"), 10, 20);

            yield return Toils_MD2General.RemoveDesignationAtPosition(GetActor().jobs.curJob.GetTarget(CellInd).Cell, DefDatabase<DesignationDef>.GetNamed("MD2CollectClay"));
        }
    }
}
