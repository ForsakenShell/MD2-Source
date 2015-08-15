using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MD2
{
    public class CoalPowerPlant : Building_PowerPlant
    {
        private static readonly Texture2D Icon = ContentFinder<Texture2D>.Get("Default");
        private float fuel = 0f;
        private float maxFuel = 100f;
        private float powerOutputInt;
        private bool isActive = true, reachedMax = false, takingResources = true;
        private static readonly float fuelConsumptionPerTick = (1f / 300f);
        private CompPowerTrader power;
        private CompGlower glower;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<float>(ref this.fuel, "fuel");
            Scribe_Values.LookValue<bool>(ref this.isActive, "isActive");
        }
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            power = base.GetComp<CompPowerTrader>();
            glower = base.GetComp<CompGlower>();
            powerOutputInt = power.powerOutputInt;
        }

        public override void Tick()
        {
            base.Tick();
            if (HasCoalFeeder && fuel < (maxFuel - 1f) && isActive)
            {
                TryTakeFuelFromFeeder(AdjacentCoalFeeder);
            }
            TryConsumeFuelAndSetPowerGeneration();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            Command_Toggle com = new Command_Toggle();
            if (isActive)
            {
                com.defaultDesc = "Click to deactivate this building";
                com.defaultLabel = "Active";
                com.icon = Icon;
            }
            else
            {
                com.icon = Icon;
                com.defaultLabel = "Not active";
                com.defaultDesc = "Click to activate this building";
            }
            com.disabled = false;
            com.groupKey = 313740004;
            com.hotKey = Keys.BuildingOnOffToggle;
            com.toggleAction = () =>
                {
                    this.isActive = !this.isActive;
                };
            com.isActive = () => { return this.isActive; };
            yield return com;
            com = new Command_Toggle();
            if (takingResources)
            {
                com.defaultDesc = "Click to stop this building from taking fuel";
                com.defaultLabel = "Fuel intake enabled";
                com.icon = Icon;
            }
            else
            {
                com.icon = Icon;
                com.defaultLabel = "Fuel intake disabled";
                com.defaultDesc = "Click to this building to take fuel";
            }
            com.disabled = false;
            com.groupKey = 313740005;
            com.hotKey = Keys.Named("CoalPowerToggleFuelConsume");
            com.toggleAction = () =>
            {
                this.takingResources = !this.takingResources;
            };
            com.isActive = () => { return this.takingResources; };
            yield return com;
            foreach (Command c in base.GetGizmos())
            {
                yield return c;
            }
        }

        public override string GetInspectString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(base.GetInspectString());
            str.AppendLine(string.Format("Fuel level: {0}%", this.fuel.ToString("0.0")));
            return str.ToString();
        }

        private void TryConsumeFuelAndSetPowerGeneration()
        {
            if (HasPowerComp)
            {
                if (HasFuel && isActive)
                {
                    UseFuel();
                    power.powerOutputInt = powerOutputInt;
                    if(glower!=null)
                    {
                        glower.Lit = true;
                    }
                    if (Find.TickManager.TicksGame % 200 == 0)
                    {
                        GenTemperature.PushHeat(this, 2f);
                    }
                }
                else
                {
                    power.powerOutputInt = 0;
                    if(glower!=null)
                    {
                        glower.Lit = false;
                    }
                }
            }
        }

        private void UseFuel()
        {
            fuel -= fuelConsumptionPerTick;
            if (fuel < 0)
            {
                fuel = 0;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }

        private void TryTakeFuelFromFeeder(CoalFeeder feeder)
        {
            if (Find.TickManager.TicksGame % 5 == 0 && takingResources)
            {
                if (feeder.HasFuelItem && !reachedMax)
                {
                    feeder.ContainedFuel.stackCount--;
                    if (feeder.ContainedFuel.stackCount == 0)
                    {
                        feeder.ContainedFuel.Destroy();
                    }
                    fuel++;
                    if (fuel >= maxFuel - 1)
                    {
                        reachedMax = true;
                    }
                }
                else
                {
                    if (reachedMax && fuel < maxFuel / 2)
                    {
                        reachedMax = false;
                    }
                }
            }
        }
        public bool HasPowerComp
        {
            get
            {
                return power != null;
            }
        }

        public CoalFeeder AdjacentCoalFeeder
        {
            get
            {
                foreach (IntVec3 current in GenAdj.CellsAdjacentCardinal(this))
                {
                    foreach(Thing thing in Find.ThingGrid.ThingsAt(current))
                    {
                        if (thing is CoalFeeder)
                        {
                            CoalFeeder feeder = thing as CoalFeeder;
                            if (feeder.HasFuelItem)
                                return feeder;
                            else
                                continue;
                        }
                    }
                }
                return null;
            }
        }

        public bool HasFuel
        {
            get
            {
                return fuel > 0f;
            }
        }

        public bool HasCoalFeeder
        {
            get
            {
                return AdjacentCoalFeeder != null;
            }
        }

        public float FuelLevel
        {
            get
            {
                return fuel;
            }
        }
    }
}
