using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class AssemblyLineUpgrade : IExposable
    {
        private AssemblyLineUpgradeDef def;
        private BillOfMaterials billOfMaterials=new BillOfMaterials();
        private int workRemaining = 0;
        public AssemblyLineUpgrade(AssemblyLineUpgradeDef def)
        {
            this.def = def;
            this.billOfMaterials = new BillOfMaterials(def.RequiredMaterials);
        }

        public AssemblyLineUpgrade()
        {

        }

        public bool Tick()
        {
            if (!billOfMaterials.HasMats)
            {
                billOfMaterials.Tick();
                return false;
            }
            else
            {
                workRemaining++;
                if (workRemaining >= def.workTicksAmount)
                    return true;
                return false;
            }
        }

        public BillOfMaterials BillOfMaterials
        {
            get
            {
                return this.billOfMaterials;
            }
        }

        public float PercentageComplete
        {
            get
            {
                return (workRemaining > 0) ? ((float)workRemaining / (float)def.workTicksAmount) : 0;
            }
        }

        public int TicksRemaining
        {
            get
            {
                return def.workTicksAmount - workRemaining;
            }
        }

        public AssemblyLineUpgradeDef Def
        {
            get
            {
                return this.def;
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.LookDef(ref this.def, "def");
            Scribe_Values.LookValue(ref this.workRemaining, "workRemaining");
            Scribe_Deep.LookDeep(ref this.billOfMaterials, "billOfMaterials");
        }
    }
}
