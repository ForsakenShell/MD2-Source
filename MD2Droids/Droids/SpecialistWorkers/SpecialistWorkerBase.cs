using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public abstract class SpecialistWorkerBase : IExposable
    {
        public abstract void SpawnSetup();

        public abstract void ExposeData();

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield break;
        }
    }
}
