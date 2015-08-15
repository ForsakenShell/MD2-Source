using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Building_DeepStorage : Building_Storage
    {
        private static readonly Texture2D EjectAllUI = ContentFinder<Texture2D>.Get("UI/Commands/EjectAll");
        private List<IntVec3> cellsThisContains = new List<IntVec3>();
        private IntVec3 inputSlot, outputSlot;
        private int maxStorage = 1500, amountOfStoredItems = 0;
        private ThingDef storedThing;
        private string storedThingName = "";

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            cellsThisContains = GenAdj.CellsOccupiedBy(this).ToList<IntVec3>();
            this.maxStorage = ((DSUDef)def).maxStorage;
            inputSlot = cellsThisContains[0];
            outputSlot = cellsThisContains[1];
            if (storedThingName != "")
            {
                this.storedThing = DefDatabase<ThingDef>.GetNamed(storedThingName);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<IntVec3>(ref this.inputSlot, "inputSlot");
            Scribe_Values.LookValue<IntVec3>(ref this.outputSlot, "outputSlot");
            Scribe_Values.LookValue<String>(ref this.storedThingName, "thingToStore", "");
            Scribe_Values.LookValue<int>(ref this.maxStorage, "maxStorage");
            Scribe_Values.LookValue<int>(ref this.amountOfStoredItems, "amountOfStoredItems");
        }

        public override void Tick()
        {
            base.Tick();
            //TODO: make DSU display stored amount in resource counter
            //int num = Find.ResourceCounter.AllCountedAmounts[storedThing];
            if (!StorageFull)
            {
                TakeItem();
            }
            if (HasItemStored)
            {
                DispenseItem();
            }
        }

        public void DispenseItem()
        {
            if (amountOfStoredItems > 0 && storedThing != null)
            {
                List<Thing> list = (
                    from t in Find.ThingGrid.ThingsAt(outputSlot)
                    where t.def == storedThing
                    select t).ToList();
                foreach (Thing thing in list)
                {
                    if (thing != null && thing.def == storedThing && thing.stackCount < thing.def.stackLimit)
                    {
                        DispenseToExistingStack(thing);
                        return;
                    }
                }
                if (list.Count == 0 && storedThing != null)
                {
                    DispenseNewStack();
                }
            }
        }
        private void DispenseNewStack()
        {
            DispenseNewStack(outputSlot);
        }
        private void DispenseNewStack(IntVec3 pos)
        {

            //Make the thing, select either the stack limit or the remaining items in storage, whichever is less. Then spawn the item
            if (storedThing != null)
            {
                int num = Mathf.Min(amountOfStoredItems, storedThing.stackLimit);
                int stack;
                ThingDef def = storedThing;
                int remainder = RemoveSomeFromStorage(num);
                if (remainder == 0)
                {
                    stack = num;
                }
                else
                {
                    stack = remainder;
                }
                Thing thing = ThingMaker.MakeThing(def);
                thing.stackCount = stack;
                GenSpawn.Spawn(thing, pos);
            }

        }

        ///<summary>
        ///Removes the given number of items from storage. Returns the remainder if the given number was greater than the amount of stored items.
        ///</summary> 
        public int RemoveSomeFromStorage(int stack)
        {
            int num = Mathf.Min(stack, amountOfStoredItems);
            amountOfStoredItems -= num;
            if (amountOfStoredItems == 0)
            {
                this.storedThing = null;
                this.storedThingName = "";
            }
            return stack - num;
        }

        private void DispenseToExistingStack(Thing thing)
        {
            if (thing != null && thing.stackCount < thing.def.stackLimit)
            {
                int amount = thing.def.stackLimit - thing.stackCount;
                int remainder = RemoveSomeFromStorage(amount);
                if (remainder == 0)
                {
                    thing.stackCount += amount;
                }
                else
                {
                    if (remainder <= amount)
                    {
                        thing.stackCount += remainder;
                    }
                    else
                    {
                        Log.Error(String.Format("Incorrect amounts when dispensing to existing stack: amount allowed {0}, amount attempted {1}", amount, remainder));
                    }
                }
                if (thing.stackCount > thing.def.stackLimit)
                {
                    thing.stackCount = thing.def.stackLimit;
                }
            }
        }

        public void TakeItem()
        {
            IEnumerable<Thing> list = (
                from t in Find.ThingGrid.ThingsAt(inputSlot)
                where t.def.EverStoreable
                select t).ToList();
            foreach (Thing thing in list)
            {
                if (thing != null && slotGroup.Settings.AllowedToAccept(thing))
                {
                    if (storedThing == null)
                    {
                        AddNewItem(thing);
                        thing.Destroy();
                        return;
                    }
                    if (storedThing != null && thing.def == storedThing)
                    {
                        int amount = StoreItem(thing.stackCount);
                        if (amount == 0)
                        {
                            thing.Destroy();
                        }
                        else
                        {
                            thing.stackCount -= amount;
                            if (thing.stackCount <= 0)
                            {
                                thing.Destroy();
                            }
                        }
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// Stores the given number of items. Returns zero if all was stored, else returns the number of items stored.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int StoreItem(int num)
        {
            int num1 = Mathf.Min(num, RemainingStorage);
            this.amountOfStoredItems += num1;
            if (num1 == num)
            {
                return 0;
            }
            else
            {
                return num1;
            }
        }

        private void AddNewItem(Thing thing)
        {
            storedThing = thing.def;
            storedThingName = thing.def.defName;
            amountOfStoredItems = thing.stackCount;
        }

        public List<IntVec3> CellsThisContains
        {
            get
            {
                return this.cellsThisContains;
            }
        }

        public bool HasItemStored
        {
            get
            {
                return this.storedThing != null;
            }
        }

        public ThingDef StoredThing
        {
            get
            {
                return this.storedThing;
            }
        }

        public IntVec3 InputSlot
        {
            get
            {
                if (this.inputSlot != null)
                {
                    return this.inputSlot;
                }
                else
                {
                    return IntVec3.Invalid;
                }
            }
        }
        public IntVec3 OutputSlot
        {
            get
            {
                if (this.outputSlot != null)
                {
                    return this.outputSlot;
                }
                else
                {
                    return IntVec3.Invalid;
                }
            }
        }

        public int RemainingStorage
        {
            get
            {
                return maxStorage - amountOfStoredItems;
            }
        }

        public bool StorageFull
        {
            get
            {
                return amountOfStoredItems == maxStorage;
            }
        }

        public override string GetInspectString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(base.GetInspectString());
            str.AppendLine(String.Format("Maximum Storage: {0}", this.maxStorage.ToString()));
            if (HasItemStored)
            {
                str.AppendLine(String.Format("Stored Item: {0}", storedThing.defName));
                str.AppendLine(String.Format("Amount: {0}", amountOfStoredItems.ToString()));
            }
            else
            {
                str.AppendLine("Nothing stored.");
            }
            return str.ToString();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (storedThing != null)
            {
                Log.Message("init");
                DispenseAllItems(mode);
            }
            base.Destroy(mode);
        }

        public void DispenseAllItems(DestroyMode mode)
        {
            if (this.storedThing != null)
            {
                if (mode == DestroyMode.Kill)
                {
                    amountOfStoredItems = Rand.Range(0, amountOfStoredItems);
                }
                int count = 0;
                while (amountOfStoredItems > 0)
                {
                    IntVec3 pos = base.Position + GenRadial.RadialPattern[count];
                    if (pos == inputSlot)
                    {
                        count++;
                        continue;
                    }
                    if (pos.Walkable())
                    {
                        List<Thing> list = (
                            from t in Find.ThingGrid.ThingsAt(pos)
                            where t.def == storedThing
                            select t).ToList();
                        if (list.Count == 0)
                        {
                            DispenseNewStack(pos);
                        }
                        else
                        {
                            foreach (Thing thing in list)
                            {
                                DispenseToExistingStack(thing);
                            }
                        }
                    }
                    count++;
                }
            }
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (base.GetGizmos() != null)
            {
                foreach (var c in base.GetGizmos())
                {
                    yield return c;
                }
            }
            var com = new Command_Action
            {
                action = () =>
                    {
                        this.DispenseAllItems(DestroyMode.Deconstruct);
                    },
                defaultDesc = "Click to eject all stored items",
                defaultLabel = "Eject All",
                activateSound = SoundDef.Named("Click"),
                hotKey = Keys.Named("DeepStorageEjectAll"),
                disabled = false,
                groupKey = 313740009,
                icon = EjectAllUI
            };
            yield return com;
        }

    }
}
