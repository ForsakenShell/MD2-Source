using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;


namespace MD2
{
    public class Droid : Pawn, ICharge, IRepairable
    {
        public BackstoryManager backstory;
        public DrawManager drawManager;
        public PartsManager parts;
        public WorkManager work;
        public UtilityManager utilities;
        public MetaDataManager meta;

        private readonly Texture2D SDIcon = ContentFinder<Texture2D>.Get("UI/Commands/SelfDestructIcon");
        private readonly Texture2D StartIcon = ContentFinder<Texture2D>.Get("UI/Commands/BeginUI");
        private readonly Texture2D StopIcon = ContentFinder<Texture2D>.Get("UI/Commands/PauseUI");
        private readonly Texture2D DeactivateIcon = ContentFinder<Texture2D>.Get("UI/Overlays/PowerOff");
        private float totalCharge = 40f;
        private bool shouldUsePower = true;

        private bool beingRepaired = false;

        public override void SpawnSetup()
        {
            ListerDroids.RegisterDroid(this);
            meta = new MetaDataManager(this);
            backstory = new BackstoryManager(this);
            backstory.SpawnSetup();
            work.SpawnSetup();
            base.SpawnSetup();
            drawManager.SpawnSetup();
        }

        public override void Tick()
        {
            utilities.Tick();
            base.Tick();
            if (!Active)
            {
                utilities.Inactive();
            }
            else
            {
                if (ShouldUsePower && !BeingRepaired)
                {
                    //Calculate the amount of energy to use.
                    Deplete(meta.PowerUsage);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<bool>(ref this.shouldUsePower, "shouldUsePower");
            Scribe_Values.LookValue<float>(ref this.totalCharge, "TotalCharge");

            //Scribe_Deep.LookDeep(ref this.backstory, "backstoryManager", new object[] { this });
            //Scribe_Deep.LookDeep(ref this.meta, "meta", new object[] { this });
            Scribe_Deep.LookDeep(ref this.drawManager, "drawManager", new object[] {this});
            Scribe_Deep.LookDeep(ref this.parts, "parts", new object[] {this});
            Scribe_Deep.LookDeep(ref this.work, "work", new object[] {this});
            Scribe_Deep.LookDeep(ref this.utilities, "utilities", new object[] {this});
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            ListerDroids.DeregisterDroid(this);

            base.Destroy(mode);
            if (mode == DestroyMode.Kill)
            {
                Messages.Message(this.LabelBase + " was destroyed!", MessageSound.Negative);
                if (meta.ExplodeOnDeath)
                    GenExplosion.DoExplosion(this.Position, meta.ExplosionRadius, DamageDefOf.Bomb, this);
            }
        }

        public override void DeSpawn()
        {
            ListerDroids.DeregisterDroid(this);
            base.DeSpawn();
        }

        public override IEnumerable<FloatMenuOption> GetExtraFloatMenuOptionsFor(IntVec3 sq)
        {
            foreach (var o in this.ExtraFloatMenuOptions(sq, base.GetExtraFloatMenuOptionsFor(sq)))
            {
                yield return o;
            }
        }

        public override string GetInspectString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(base.GetInspectString());
            str.AppendLine(string.Format("Current energy: {0}Wd/{1}Wd", TotalCharge.ToString("0.0"), MaxEnergy));
            //Debug
            //str.AppendLine("Energy use: " + utilities.PowerUsage.ToString("0.0"));
            return str.ToString();
        }

        public override TipSignal GetTooltip()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine(this.LabelCap + " " + this.kindDef.label);
            s.AppendLine("Current energy: " + this.TotalCharge.ToString("0.0") + "W/" + this.MaxEnergy.ToString() + "Wd");
            if (this.equipment != null && this.equipment.Primary != null)
            {
                s.AppendLine(this.equipment.Primary.LabelCap);
            }
            s.AppendLine(HealthUtility.GetGeneralConditionLabel(this));
            return new TipSignal(s.ToString().TrimEnd(new char[]
            {
                '\n'
            }), this.thingIDNumber*152317, TooltipPriority.Pawn);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (base.GetGizmos() != null)
            {
                foreach (Gizmo c in base.GetGizmos())
                {
                    yield return c;
                }
            }
            Command_Action com = new Command_Action();
            com.activateSound = SoundDefOf.Click;
            if (Active)
            {
                com.defaultDesc = "DisableDescription".Translate();
                com.defaultLabel = "Disable".Translate();
                com.icon = this.StopIcon;
            }
            else
            {
                com.defaultDesc = "EnableDescription".Translate();
                com.defaultLabel = "Enable".Translate();
                com.icon = this.StartIcon;
            }
            com.disabled = false;
            com.groupKey = 313740004;
            com.hotKey = KeyBindingDef.Named("MD2DisableDroid");
            com.action = () => { Active = !Active; };
            yield return com;
            //////////////////////////
            Command_Action deactivate = new Command_Action();
            deactivate.action =
                () =>
                {
                    Find.LayerStack.Add(new Dialog_Confirm("DeactivateDialog".Translate(),
                        delegate { utilities.Disable(); }));
                };
            deactivate.activateSound = SoundDefOf.Click;
            deactivate.defaultDesc = "DeactivateDescription".Translate();
            deactivate.defaultLabel = "DeactivateLabel".Translate();
            deactivate.disabled = false;
            deactivate.groupKey = 313740005;
            deactivate.icon = this.DeactivateIcon;
            deactivate.hotKey = KeyBindingDef.Named("MD2DeactivateDroid");
            yield return deactivate;
            ///////////////////////
            Command_Action selfDestruct = new Command_Action();
            selfDestruct.action =
                () =>
                {
                    Find.LayerStack.Add(new Dialog_Confirm("DroidSelfDestructPrompt".Translate(),
                        delegate { this.Destroy(DestroyMode.Kill); }));
                };
            selfDestruct.activateSound = SoundDefOf.Click;
            selfDestruct.defaultDesc = "SelfDestructDescription".Translate();
            selfDestruct.defaultLabel = "SelfDestructLabel".Translate();
            selfDestruct.disabled = false;
            selfDestruct.groupKey = 313740006;
            selfDestruct.icon = this.SDIcon;
            selfDestruct.hotKey = KeyBindingDef.Named("MD2SelfDestructDroid");
            yield return selfDestruct;

            foreach (var g in work.specialist.GetSpecialistGizmos())
                yield return g;
        }

        public override void DrawGUIOverlay()
        {
            base.DrawGUIOverlay();
            drawManager.DrawGUIOverlay();
        }

        public bool Active
        {
            get { return utilities.Active; }
            set { utilities.Active = value; }
        }

        #region Charge

        public float TotalCharge
        {
            get { return this.totalCharge; }
            set { this.totalCharge = value; }
        }

        public float MaxEnergy
        {
            get { return parts.MaxEnergy; }
        }

        public bool AddPowerDirect(float amount)
        {
            TotalCharge += amount;
            if (TotalCharge > MaxEnergy)
            {
                TotalCharge = MaxEnergy;
                return false;
            }
            return true;
        }

        public bool RemovePowerDirect(float amount)
        {
            TotalCharge -= amount;
            if (TotalCharge < 0)
            {
                TotalCharge = 0f;
                return false;
            }
            return true;
        }

        public bool Charge(float rate)
        {
            if (TotalCharge < MaxEnergy)
            {
                TotalCharge += (rate*CompPower.WattsToWattDaysPerTick);
                if (TotalCharge > MaxEnergy)
                    TotalCharge = MaxEnergy;
                return true;
            }
            return false;
        }

        public bool Deplete(float rate)
        {
            if (TotalCharge > 0)
            {
                TotalCharge -= (rate*CompPower.WattsToWattDaysPerTick);
                if (TotalCharge < 0)
                    TotalCharge = 0;
                return true;
            }
            return false;
        }

        public bool DesiresCharge
        {
            get { return this.TotalCharge < MaxEnergy; }
        }

        public bool ShouldUsePower
        {
            get { return shouldUsePower; }
            set { shouldUsePower = value; }
        }

        public bool CanTryGetCharge
        {
            get { return Active; }
        }

        public Pawn Parent
        {
            get { return this; }
        }

        public float PowerSafeThreshold
        {
            get { return meta.PowerSafeThreshold; }
        }

        public float PowerLowThreshold
        {
            get { return meta.PowerLowThreshold; }
        }

        public float PowerCriticalThreshold
        {
            get { return meta.PowerCriticalThreshold; }
        }

        #endregion

        #region Repair

        public virtual bool BeingRepaired
        {
            get { return beingRepaired; }
            set { beingRepaired = value; }
        }

        public virtual void Repair(Building_RepairStation repairStation)
        {
            List<Hediff_Injury> allInjuries = health.hediffSet.GetHediffs<Hediff_Injury>().ToList();
            List<Hediff_MissingPart> allMissingParts = health.hediffSet.GetHediffs<Hediff_MissingPart>().ToList();


            float num = Rand.Value;

            if ((allInjuries.Count == 0 || num > 0.6) && allMissingParts.Count > 0 && repairStation != null &&
                repairStation.HasEnoughOf(repairStation.Def.repairThingDef, repairStation.Def.repairCostAmount))
            {
                Hediff_MissingPart hediff = allMissingParts.RandomElement();
                if (repairStation.TakeSomeOf(repairStation.Def.repairThingDef, repairStation.Def.repairCostAmount))
                {
                    health.hediffSet.RestorePart(hediff.Part.HighestMissingPart(this));
                }
            }
            else if (allInjuries.Count > 0)
            {
                Hediff_Injury hediff = allInjuries.RandomElement();
                if (hediff.def.injuryProps.fullyHealableOnlyByTreatment)
                {
                    HediffComp_Treatable treatable = hediff.TryGetComp<HediffComp_Treatable>();
                    if (treatable != null && !treatable.treatedWithMedicine)
                    {
                        treatable.NewlyTreated(1f, ThingDefOf.Medicine);
                    }
                }
                hediff.DirectHeal(repairStation.Def.repairAmount);
            }
        }

        public virtual bool ShouldGetRepairs
        {
            get
            {
                return (health.hediffSet.GetHediffs<Hediff_Injury>().Count() > 0 ||
                        health.hediffSet.HasFreshMissingPartsCommonAncestor() ||
                        health.hediffSet.GetHediffs<Hediff_MissingPart>().Count() > 0) &&
                       ((this.Faction == Faction.OfColony && !this.IsPrisonerOfColony) ||
                        (this.guest != null && this.guest.DoctorsCare));
            }
        }

        public Pawn Pawn
        {
            get { return this; }
        }

        public virtual int RepairsNeededCount
        {
            get
            {
                return health.hediffSet.GetHediffs<Hediff_Injury>().Count() +
                       health.hediffSet.GetHediffs<Hediff_MissingPart>().Count();
            }
        }

        #endregion

        public override string LabelBase
        {
            get { return this.Nickname; }
        }

        public override string LabelBaseShort
        {
            get { return this.Nickname; }
        }

        public override string LabelCap
        {
            get { return this.Nickname; }
        }
    }
}