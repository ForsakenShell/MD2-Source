using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    class DroidCorpse : Corpse
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.DeSpawn();
        }
        public override void Tick()
        {
            base.SpawnSetup();
            this.DeSpawn();
        }
    }
}
