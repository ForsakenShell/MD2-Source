using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class Storage : IExposable
    {
        private AssemblyLine parent;
        private List<ListItem> allStoredItemsList;

        public Storage(AssemblyLine parent)
        {
            this.parent = parent;
            this.allStoredItemsList = new List<ListItem>();
        }

        public Storage()
        {
            this.allStoredItemsList = new List<ListItem>();
        }

        public bool OnAssemblyLine
        {
            get
            {
                return parent != null;
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.LookList<ListItem>(ref this.allStoredItemsList, "AllStoredItems", LookMode.Deep);
        }

        public void AddItem(ThingDef def, int amount)
        {
            if(Contains(def))
            {
                AddToExisting(def, amount);
                return;
            }
            AddNew(def, amount);
        }

        private void AddNew(ThingDef def, int amount)
        {
            this.allStoredItemsList.Add(new ListItem(def, amount));
        }

        private void AddToExisting(ThingDef def, int amount)
        {
            ListItem item = GetItemFromDef(def);
            item.amount += amount;
        }

        private ListItem GetItemFromDef(ThingDef def)
        {
            return (from t in this.allStoredItemsList
                   where t.thing.defName == def.defName
                   select t).First();
        }

        public bool Contains(ThingDef def)
        {
            foreach(ListItem item in this.allStoredItemsList)
            {
                if(item.thing.defName == def.defName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
