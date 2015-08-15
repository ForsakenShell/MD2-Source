using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MD2
{
    public class CompDroidCharger : ThingComp
    {
        private ICharge chargee;
        private bool initialised = false;

        private bool CanUse(ICharge chargee)
        {
            return this.chargee == null || this.chargee == chargee;
        }

        private CompPowerTrader Power
        {
            get
            {
                return parent.TryGetComp<CompPowerTrader>();
            }
        }

        public bool IsAvailable(ICharge d)
        {
            return CanUse(d) && (Power == null || Power.PowerOn);
        }

        public void BeginCharge(ICharge chargee)
        {
            this.chargee = chargee;
            if (Power != null)
            {
                Power.powerOutputInt = -Power.props.basePowerConsumption;
            }
        }

        public void EndCharge()
        {
            chargee = null;
            if (Power != null)
            {
                Power.powerOutputInt = -1f;
            }
        }

        public ICharge Chargee
        {
            get
            {
                return this.chargee;
            }
        }

        private void Destroy()
        {
            if (chargee != null)
                chargee.Parent.jobs.EndCurrentJob(Verse.AI.JobCondition.Incompletable);
            ListerDroids.DeregisterCharger(parent);
        }

        public override void PostDestroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDestroy(mode);
            Destroy();
        }

        public override void PostDeSpawn()
        {
            base.PostDeSpawn();
            Destroy();
        }

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();
            if (!initialised)
            {
                EndCharge();
                initialised = true;
            }
            ListerDroids.RegisterCharger(parent);
            if (Power != null)
            {
                Power.powerOutputInt = -1f;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (chargee != null)
            {
                if (Power != null && Power.PowerOn)
                {
                    Chargee.Charge(Power.props.basePowerConsumption * 2f);
                }
            }
        }
    }
}
