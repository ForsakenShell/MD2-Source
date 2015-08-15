using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MD2
{
    public class Alert_DroidPowerLow : Alert_Medium
    {
        private IEnumerable<Droid> NeedingDroids
        {
            get
            {
                return (from t in ListerDroids.AllDroids
                        where t.TotalCharge <= t.MaxEnergy * t.meta.PowerLowThreshold && t.TotalCharge > t.MaxEnergy * t.meta.PowerCriticalThreshold
                        select t).AsEnumerable();
            }
        }

        public override string FullExplanation
        {
            get
            {
                return "Alert_DroidPowerLowDescription".Translate();
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

        public Alert_DroidPowerLow()
        {
            this.baseLabel = "Alert_DroidPowerLowLabel".Translate();
        }
    }
}
