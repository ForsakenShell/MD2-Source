using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class StatExt
    {
        public static float GetStatValue(this Thing thing, StatDef stat, bool applyPostProcess = true)
        {
            return stat.Worker.GetValue(thing, applyPostProcess);
        }
    }
}
