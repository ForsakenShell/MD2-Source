using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MD2
{
    public class Alert_DroidPowerCriticallyLow : Alert_Critical
    {
        private IEnumerable<Droid> NeedingDroids
        {
            get
            {
                return (from t in ListerDroids.AllDroids
                        where t.TotalCharge <= t.MaxEnergy * t.meta.PowerCriticalThreshold
                        select t).AsEnumerable();
            }
        }

        public override string FullExplanation
        {
            get
            {
                return "Alert_DroidPowerCriticallyLowDescription".Translate();
            }
        }

        public override AlertReport Report
        {
            get
            {
                Droid droid = NeedingDroids.FirstOrDefault();
                return (droid == null) ? false : AlertReport.CulpritIs(droid);
            }
        }

        public Alert_DroidPowerCriticallyLow()
        {
            this.baseLabel = "Alert_DroidPowerCriticallyLowLabel".Translate();
        }
    }
}
