using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Building_RepairStation : Building
    {
        private IRepairable repairee;
        private DeactivatedDroid innerDroid = null;
        private List<CompRepairStationSupplier> suppliers = new List<CompRepairStationSupplier>();
        private bool init = false;

        public CompPowerTrader Power
        {
            get
            {
                return GetComp<CompPowerTrader>();
            }
        }

        public ThingDef_RepairStation Def
        {
            get
            {
                return this.def as ThingDef_RepairStation;
            }
        }

        public IRepairable Repairee
        {
            get
            {
                return repairee;
            }
        }

        private void SetPowerUsage()
        {
            if (Power != null)
            {
                if (Repairee != null || InnerDroid != null)
                    Power.powerOutputInt = -Power.props.basePowerConsumption;
                else
                    Power.powerOutputInt = -1f;
            }
        }

        #region Reactivation
        public DeactivatedDroid InnerDroid
        {
            get
            {
                return innerDroid;
            }
        }

        public bool IsAvailableForReactivation
        {
            get
            {
                return InnerDroid == null;
            }
        }

        public void AddDroid(DeactivatedDroid droid)
        {
            if (IsAvailableForReactivation)
            {
                innerDroid = droid;
                if (droid.SpawnedInWorld)
                    droid.DeSpawn();
                innerDroid.InnerDroid.ShouldUsePower = false;
                SetPowerUsage();
            }
            else
                Log.Message("Tried to add a deactivated droid to: " + this.ToString() + " but it already had something in it!");
        }

        public void DropDroid()
        {
            if (!IsAvailableForReactivation)
            {
                GenSpawn.Spawn(InnerDroid, InteractionCell);
                innerDroid = null;
                SetPowerUsage();
            }
        }

        public void ReactivateDroid(bool dropDroid = false)
        {
            if (!IsAvailableForReactivation)
            {
                if (ReadyForReactivation)
                {
                    Droid droid = InnerDroid.InnerDroid;
                    droid.playerController.Drafted = false;
                    droid.ShouldUsePower = true;
                    GenSpawn.Spawn(droid, InteractionCell);
                    Messages.Message("DroidReactivated".Translate(new object[] { droid.LabelBase }), MessageSound.Benefit);
                    innerDroid.Destroy();
                    innerDroid = null;
                    SetPowerUsage();
                }
                else if (dropDroid)
                    DropDroid();
            }
        }

        public bool ReadyForReactivation
        {
            get
            {
                if (InnerDroid != null)
                {
                    return !InnerDroid.InnerDroid.ShouldGetRepairs && InnerDroid.InnerDroid.TotalCharge >= InnerDroid.InnerDroid.MaxEnergy * 0.1f;
                }
                return false;
            }
        }

        public void ReactivationTick()
        {
            if (InnerDroid != null)
            {
                Droid droid = InnerDroid.InnerDroid;
                if (droid.ShouldGetRepairs)
                {
                    if (Find.TickManager.TicksGame % TicksPerRepairCycle == 0)
                        droid.Repair(this);
                }
                float num = (Power != null) ? Power.props.basePowerConsumption * 2f : 400f;
                droid.Charge(num);

                InnerDroid.InnerDroid.health.HealthTick();

                if (ReadyForReactivation)
                    ReactivateDroid();
            }
        }
        #endregion

        #region Resources
        public IEnumerable<Thing> AllAvailableResources
        {
            get
            {
                if (suppliers.Count > 0)
                {
                    foreach (var s in suppliers)
                    {
                        foreach (var t in s.AvailableResources)
                        {
                            yield return t;
                        }
                    }
                }
            }
        }

        public bool HasEnoughOf(ThingDef def, int amount)
        {
            int amountAvailable = 0;
            foreach (var t in AllAvailableResources)
            {
                if (t.def == def)
                {
                    //Log.Message("Found some");
                    amountAvailable += t.stackCount;
                }
            }
            return amountAvailable >= amount ? true : false;
        }

        public bool TakeSomeOf(ThingDef def, int amount)
        {
            if (HasEnoughOf(def, amount))
            {
                int remaining = amount;
                while (remaining > 0)
                {
                    foreach (var t in AllAvailableResources)
                    {
                        if (t.def == def)
                        {
                            int num = Mathf.Min(t.stackCount, remaining);
                            remaining -= num;
                            t.stackCount -= num;
                            if (t.stackCount <= 0)
                                t.Destroy();
                            //Log.Message("Took some");
                        }
                        if (remaining <= 0)
                            break;
                    }
                }
                if (remaining <= 0)
                {
                    //Log.Message("Took all");
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion

        #region Override Methods
        public override void Tick()
        {
            if (!init)
            {
                init = true;
                SetPowerUsage();
            }
            base.Tick();
            if (Repairee != null && (Power == null || Power.PowerOn))
            {
                if (Find.TickManager.TicksGame % TicksPerRepairCycle == 0)
                {
                    Repairee.Repair(this);
                }
            }
            if (!IsAvailableForReactivation && (Power == null || Power.PowerOn))
            {
                ReactivationTick();
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            Notify_RepairStationDespawned();
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Notify_RepairStationSpawned();
        }

        public override string GetInspectString()
        {
            string str = base.GetInspectString();
            if (!IsAvailableForReactivation)
            {
                str += "\n";
                str += "Reactivating".Translate(new object[] { InnerDroid.LabelBase });
            }
            return str;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.LookDeep(ref this.innerDroid, "innerDroid");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;
            if (!IsAvailableForReactivation)
            {
                Command_Action c = new Command_Action();
                c.groupKey = 100045;
                c.action = () =>
                    {
                        DropDroid();
                    };
                c.defaultDesc = "StopReactivationDescription".Translate();
                c.defaultLabel = "StopReactivationLabel".Translate();
                yield return c;
            }
        }
        #endregion

        #region Spawning
        public void Notify_SupplierSpawned(CompRepairStationSupplier supplier)
        {
            if (!suppliers.Contains(supplier))
                suppliers.Add(supplier);
        }

        public void Notify_SupplierDespawned(CompRepairStationSupplier supplier)
        {
            if (suppliers.Contains(supplier))
                suppliers.Remove(supplier);
        }

        private void Notify_RepairStationSpawned()
        {
            ListerDroids.RegisterRepairStation(this);
            foreach (var c in GenAdj.CellsAdjacentCardinal(this))
            {
                Building b = Find.ThingGrid.ThingAt<Building>(c);
                if (b != null)
                {
                    CompRepairStationSupplier supplier = b.GetComp<CompRepairStationSupplier>();
                    if (supplier != null)
                    {
                        supplier.Notify_RepairStationSpawned(this);
                    }
                }
            }
        }

        private void Notify_RepairStationDespawned()
        {
            ListerDroids.DeregisterRepairStation(this);
            ReactivateDroid(true);
            if (suppliers.Count > 0)
            {
                foreach (var s in suppliers)
                {
                    s.Notify_RepairStationDespawned(this);
                }
            }
        }
        #endregion

        #region Repairing
        public int TicksPerRepairCycle
        {
            get
            {
                return Def == null ? 120 : Def.ticksPerRepairCycle;
            }
        }

        public void RegisterRepairee(IRepairable r)
        {
            this.repairee = r;
            SetPowerUsage();
        }

        public void DeregisterRepairee(IRepairable r)
        {
            repairee = null;
            SetPowerUsage();
        }

        public bool IsAvailable(IRepairable p)
        {
            if (Power != null)
            {
                return Power.PowerOn && CanUse(p);
            }
            return CanUse(p);
        }

        private bool CanUse(IRepairable p)
        {
            return repairee == null || repairee == p;
        }
        #endregion
    }
}
