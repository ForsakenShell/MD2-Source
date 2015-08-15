using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobGiver_DroidCharge : ThinkNode_JobGiver
    {
        public int stage = 0;

        public JobGiver_DroidCharge(int stage)
        {
            this.stage = stage;
        }

        protected override Job TryGiveTerminalJob(Pawn pawn)
        {

            if (!(pawn is ICharge))
            {
                return null;
            }
            ICharge chargee = (ICharge)pawn;
            Droid droid = (Droid)pawn;
            if (droid !=null && !droid.Active)
                return null;

            float chargeThreshold;
            float distance;
            switch (stage)
            {
                case 1:
                    {
                        chargeThreshold = chargee.PowerSafeThreshold;
                        distance = 20f;
                        break;
                    }
                case 2:
                    {
                        chargeThreshold = chargee.PowerLowThreshold;
                        distance = 50f;
                        break;
                    }
                case 3:
                    {
                        chargeThreshold = chargee.PowerCriticalThreshold;
                        distance = 9999f;
                        break;
                    }
                default:
                    {
                        chargeThreshold = chargee.PowerLowThreshold;
                        distance = 50f;
                        break;
                    }
            }
            if (chargee.TotalCharge <= chargee.MaxEnergy * chargeThreshold)
            {
                Thing target = ListerDroids.ClosestChargerFor(chargee, distance);
                if (target != null)
                {
                    return new Job(DefDatabase<JobDef>.GetNamed("MD2ChargeDroid"), new TargetInfo(target));
                }
            }
            return null;
        }
    }

    public class JobGiver_ChargeStaySafe : JobGiver_DroidCharge
    {
        public JobGiver_ChargeStaySafe() : base(1) { }
    }
    public class JobGiver_ChargeLow : JobGiver_DroidCharge
    {
        public JobGiver_ChargeLow() : base(2) { }
    }
    public class JobGiver_ChargeCritical : JobGiver_DroidCharge
    {
        public JobGiver_ChargeCritical() : base(3) { }
    }
}
