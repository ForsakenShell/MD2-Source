using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;

namespace MD2
{
    public class JobDriver_DroidDeactivated : JobDriver
    {
        public static Verse.JobDef Def
        {
            get
            {
                return Verse.DefDatabase<Verse.JobDef>.GetNamed("MD2DroidDeactivatedJob");
            }
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_General.Wait(10000);
        }
    }
}
