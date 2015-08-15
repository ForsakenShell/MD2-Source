using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class ListItem : IExposable
    {
        public ListItem(ThingDef thing, int amount)
        {
            this.thing = thing;
            this.amount = amount;
        }
        public ListItem()
        {
            thing = null;
            amount = 0;
        }
        public ThingDef thing;
        public int amount;

        public void ExposeData()
        {
            Scribe_Values.LookValue<int>(ref this.amount, "amount");
            Scribe_Defs.LookDef<ThingDef>(ref this.thing, "thing");
        }
    }
}
