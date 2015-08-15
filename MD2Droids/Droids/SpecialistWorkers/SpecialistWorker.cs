using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class SpecialistWorker : SpecialistWorkerBase
    {
        private Droid parent;

        public SpecialistWorker(Droid droid)
        {
            parent = droid;
        }

        public Droid Parent
        {
            get
            {
                return parent;
            }
        }

        public override void SpawnSetup()
        {
        }

        public override void ExposeData()
        {
        }
    }
}
