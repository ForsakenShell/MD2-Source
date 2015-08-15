using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using MD2;
using RimWorld;

namespace MD2
{
    public class Order : IExposable
    {
        private AssemblyLine line;
        private OrderConfig config;
        private int completedCycles = 0;
        private ShoppingList shoppingList;
        private int ticksToFinish = 0;
        private bool deleted = false;
        private bool moreNeeded = true;


        public Order(AssemblyLine line, OrderConfig config)
        {
            this.shoppingList = new ShoppingList(this);
            this.line = line;
            this.SetConfigDirect(config);
            this.TicksToFinish = Config.WorkAmount;
        }

        public Order(AssemblyLine line)
        {
            this.line = line;
        }

        public void Delete()
        {
            deleted = true;
            FinishCycle(true);
            line.OrderStack.FinishOrderAndGetNext(this, true);
        }

        private void DropAcquiredMats()
        {
            List<Thing> list = new List<Thing>();
            foreach (var item in ShoppingList.AcquiredMats)
            {
                int remaining = item.amount;
                while (remaining > 0)
                {
                    int num = UnityEngine.Mathf.Min(item.thing.stackLimit, remaining);
                    Thing thing = ThingMaker.MakeThing(item.thing);
                    thing.stackCount = num;
                    remaining -= num;
                    list.Add(thing);
                }
            }

            foreach (var thing in list)
            {
                IntVec3 loc = RCellFinder.TradeDropSpot();
                DropPodUtility.MakeDropPodAt(loc, new DropPodInfo
                    {
                        SingleContainedThing = thing,
                        leaveSlag = false
                    });
            }
        }

        public void Tick()
        {
            //If the order has been deleted or suspended, then do nothing
            if (deleted || Config.Suspended) return;
            //Check for targetcount. If need some, then do the work.
            if (!shoppingList.HasAllMats)
            {
                shoppingList.Tick();
                return;
            }
            if (DecreaseTime())
            {
                FinishCycle();
            }
        }

        private void FinishCycle(bool forceComplete = false)
        {
            if (forceComplete)
            {
                DropAcquiredMats();
                if (completedCycles > 0)
                {
                    DropProducts();
                    return;
                }
                return;
            }
            switch (config.RepeatMode)
            {
                case BillRepeatMode.Forever:
                    {
                        if (config.CompleteAll)
                        {
                            completedCycles++;
                            if (completedCycles >= OrderConfig.repeatCountMaxCycles)
                            {
                                DropProducts();
                            }
                        }
                        else
                        {
                            completedCycles++;
                            DropProducts();
                        }
                        break;
                    }
                case BillRepeatMode.RepeatCount:
                    {
                        if (config.CompleteAll)
                        {
                            Cycles--;
                            completedCycles++;
                            if (Cycles == 0 || completedCycles > OrderConfig.repeatCountMaxCycles)
                            {
                                DropProducts();
                            }
                        }
                        else
                        {
                            Cycles--;
                            completedCycles++;
                            DropProducts();
                        }
                        if (Cycles == 0)
                        {
                            Messages.Message(string.Concat(new Object[]{
                            line.label,
                            " has completed its order ",
                            "\"",
                            Config.Recipe.LabelCap,
                            "\""
                            }),
                            MessageSound.Benefit);
                        }
                        break;
                    }
                case BillRepeatMode.TargetCount:
                    {
                        if (config.CompleteAll)
                        {
                            completedCycles++;
                            if (completedCycles > OrderConfig.targetCountMaxCycles)
                            {
                                DropProducts();
                            }
                        }
                        else
                        {
                            completedCycles++;
                            DropProducts();
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }
            TicksToFinish = Config.WorkAmount;
            ShoppingList.Reset();
        }

        private void DropProducts()
        {
            IEnumerable<Thing> products = Config.Recipe.MakeProducts(this, completedCycles);
            foreach (var c in products)
            {
                //Log.Message(c.Label + c.stackCount.ToString());
                IntVec3 loc = RCellFinder.TradeDropSpot();
                DropPodUtility.MakeDropPodAt(loc, new DropPodInfo
                {
                    SingleContainedThing = c,
                    leaveSlag = false
                });
            }
            completedCycles = 0;
        }

        public bool DecreaseTime()
        {
            if (config.RepeatMode == BillRepeatMode.TargetCount && moreNeeded || config.RepeatMode != BillRepeatMode.TargetCount)
            {
                TicksToFinish--;
                //Log.Message("Time: " + TicksToFinish.ToString());
                if (TicksToFinish <= 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void SetConfigDirect(OrderConfig config)
        {
            this.config = config;
            config.SetParentDirect(this);
        }

        public bool DesiresToWork
        {
            get
            {
                switch (config.RepeatMode)
                {
                    case BillRepeatMode.Forever:
                        return true;
                    case BillRepeatMode.RepeatCount:
                        {
                            if (Cycles > 0)
                                return true;
                            else
                                return false;
                        }
                    case BillRepeatMode.TargetCount:
                        {
                            int num;
                            Find.ResourceCounter.AllCountedAmounts.TryGetValue(Config.Recipe.products.First().thingDef, out num);
                            if (num < config.TargetCount)
                            {
                                moreNeeded = true;
                            }
                            else
                            {
                                moreNeeded = false;
                            }
                            return moreNeeded;
                        }
                    default:
                        {
                            Log.Error("Config for " + Config.Recipe.label + " in " + line.label + " had no repeat mode");
                            return false;
                        }
                }
            }
        }

        public AssemblyLine Line
        {
            get
            {
                return this.line;
            }
        }

        public RecipeDef Recipe
        {
            get
            {
                return this.Config.Recipe;
            }
        }

        public OrderConfig Config
        {
            get
            {
                return this.config;
            }
        }

        public int TicksToFinish
        {
            get
            {
                return this.ticksToFinish;
            }
            set
            {
                this.ticksToFinish = value;
                if (this.ticksToFinish < 0)
                {
                    this.ticksToFinish = 0;
                }
            }
        }

        public ShoppingList ShoppingList
        {
            get
            {
                return this.shoppingList;
            }
        }



        public bool Deleted
        {
            get
            {
                return this.deleted;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.completedCycles, "completedCycles");
            Scribe_Values.LookValue(ref this.ticksToFinish, "ticksToFinish");
            Scribe_Deep.LookDeep<ShoppingList>(ref this.shoppingList, "shoppingList", this);
            Scribe_Values.LookValue(ref this.deleted, "deleted");
            Scribe_Deep.LookDeep(ref this.config, "config", this);
            Scribe_Values.LookValue(ref this.moreNeeded, "moreNeeded");
        }

        public int Cycles
        {
            get
            {
                return this.config.Cycles;
            }
            set
            {
                this.config.Cycles = value;
            }
        }

        public string GetTimeRemaining
        {
            get
            {
                if (Config.RepeatMode == BillRepeatMode.RepeatCount && Config.CompleteAll)
                {
                    int num = UnityEngine.Mathf.Max(0, Cycles - 1);
                    int curTicks = this.TicksToFinish + (num * Config.WorkAmount);
                    return TicksToTime.GetTime(curTicks);
                }
                return TicksToTime.GetTime(TicksToFinish);
            }
        }

        public bool CanMoveUp
        {
            get
            {
                return line.OrderStack.IndexOf(this) > 0 && line.OrderStack.Count > 1;
            }
        }

        public bool CanMoveDown
        {
            get
            {
                return line.OrderStack.IndexOf(this) < line.OrderStack.Count - 1 && line.OrderStack.Count > 1;
            }
        }
    }

}
