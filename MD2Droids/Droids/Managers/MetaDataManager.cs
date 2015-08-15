using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD2
{
    public class MetaDataManager
    {
        private Droid parent;
        public float? powerUsageCached = null;

        public bool ExplodeOnDeath = false;
        public float ExplosionRadius = 0.9f;
        public float PowerSafeThreshold = 0.55f;
        public float PowerLowThreshold = 0.35f;
        public float PowerCriticalThreshold = 0.2f;
        public bool CanManThings = true;

        public MetaDataManager(Droid droid)
        {
            parent = droid;
        }

        public MetaDataManager(Droid droid, Blueprint bp):this(droid)
        {
            ExplodeOnDeath = bp.ExplodeOnDeath;
            ExplosionRadius = bp.ExplosionRadius;

        }

        public float PowerUsage
        {
            get
            {
                if (powerUsageCached == null)
                {
                    float num = 100f;
                    num += parent.work.PowerNeeds;
                    if (ExplodeOnDeath)
                        num += 20f;
                    powerUsageCached = num;
                }
                return (float)powerUsageCached;
            }
        }

    }
}
