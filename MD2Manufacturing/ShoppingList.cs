using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class ShoppingList : IExposable
    {
        private List<ListItem> acquiredMats = new List<ListItem>();
        private List<ListItem> foundItems = new List<ListItem>();
        private bool hasAllMats = false;
        private Order parent;


        public ShoppingList(Order parent)
        {
            this.parent = parent;
        }

        public void Tick()
        {
            if (Find.TickManager.TicksGame % 120 == 0)
            {
                if (FindItems())
                {
                    TakeNeededItems();
                }
            }
        }

        private void TakeNeededItems()
        {
            foreach(var item in this.foundItems)
            {
                IEnumerable<Thing> things = (
                    from t in ResourceFinder.AllUsableThings
                    where t.def == item.thing
                    select t).AsEnumerable();

                int remaining = item.amount;
                acquiredMats.Add(new ListItem(item.thing, item.amount));

                foreach(var thing in things)
                {
                    int num = Mathf.Min(remaining, thing.stackCount);
                    thing.TakeFrom(num);
                    remaining -= num;
                    if (remaining <= 0)
                        break;
                }
            }
            this.hasAllMats = true;
        }

        private bool FindItems()
        {
            List<ListItem> list = new List<ListItem>();
            foreach(var ingredientCount in this.parent.Recipe.ingredients)
            {
                foreach(var thingDef in ingredientCount.filter.AllowedThingDefs.Where((ThingDef def) => parent.Config.IngredientsFilter.Allows(def)))
                {
                    int amount = (int)(ingredientCount.CountUsing(thingDef) * parent.Line.Efficiency);
                    if(SearchForItem(thingDef, amount))
                    {
                        list.Add(new ListItem(thingDef, amount));
                        break;
                    }
                }
            }
            if(list.Count==parent.Recipe.ingredients.Count)
            {
                foundItems = list;
                return true;
            }
            foundItems = null;
            return false;
        }

        private bool SearchForItem(ThingDef def, int amount)
        {
            IEnumerable<Thing> things = (
                from t in ResourceFinder.AllUsableThings
                where t.def == def
                select t).AsEnumerable();

            if(things.Count()>0)
            {
                int count=0;
                foreach(var thing in things)
                {
                    count += thing.stackCount;
                }
                if (count >= amount)
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            this.hasAllMats = false;
            this.acquiredMats = new List<ListItem>();
        }

        public IEnumerable<ListItem> AcquiredMats
        {
            get
            {
                return this.acquiredMats.AsEnumerable();
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.LookList<ListItem>(ref this.acquiredMats, "acquiredMats",LookMode.Deep);
            Scribe_Collections.LookList(ref this.foundItems,"foundItems",LookMode.Deep);
        }

        public bool HasAllMats
        {
            get
            {
                return hasAllMats;
            }
        }

    }

}
