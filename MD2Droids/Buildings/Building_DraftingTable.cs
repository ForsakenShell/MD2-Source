using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class Building_DraftingTable : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;

            Command_Action a = new Command_Action();
            a.action = () => Find.LayerStack.Add(new Page_Drafting(null));
            yield return a;
        }
    }
}
