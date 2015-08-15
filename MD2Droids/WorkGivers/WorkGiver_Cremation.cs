using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class WorkGiver_Cremation : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForGroup(ThingRequestGroup.Corpse);
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.ClosestTouch;
            }
        }

        public override bool ShouldSkip(Pawn pawn)
        {
            Droid droid = pawn as Droid;
            bool flag = droid != null && droid.work.Contains(WorkPackageDef.Named("MD2CremationPackage")) && droid.work.specialist.GetWorker<CremationWorker>() != null;
            return !flag;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t)
        {
            Droid droid = pawn as Droid;
            if(droid==null)return false;
            CremationWorker worker = droid.work.specialist.GetWorker<CremationWorker>();
            Predicate<Thing> predicate1 = (Thing corpse) => !corpse.IsForbidden(Faction.OfColony) && droid.AwareOf(corpse) && droid.CanReserve(corpse);
            bool flag = t is Corpse && droid != null && predicate1(t) && worker.AllTargets.Any((CremationTarget target) => target.Accepts(t));
            return flag;
        }

        public override Job JobOnThing(Pawn pawn, Thing t)
        {
            JobDef jDef;
            Droid droid = pawn as Droid;
            if (droid == null) return null;
            CremationTarget target = droid.work.specialist.GetWorker<CremationWorker>().AllTargets.First((CremationTarget ta) => ta.Accepts(t));
            switch(target.Mode)
            {
                case CremationOperationMode.Butcher:
                    {
                        jDef = DefDatabase<JobDef>.GetNamed("MD2DroidButcherCorpse");
                        break;
                    }
                case CremationOperationMode.Cremate:
                    {
                        jDef = DefDatabase<JobDef>.GetNamed("MD2DroidCremateCorpse");
                        break;
                    }
                default:
                    return null;
            }
            return new Job(jDef, t);
        }
    }
}
