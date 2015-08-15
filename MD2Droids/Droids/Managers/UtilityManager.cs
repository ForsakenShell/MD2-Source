using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class UtilityManager : IExposable
    {
        private Droid parent;
        private bool active = true;
        private const string DeactivatedDroidDefName = "MD2DeactivatedDroid";
        public static readonly JobDef ChargeJob = DefDatabase<JobDef>.GetNamed("MD2ChargeDroid");

        public UtilityManager(Droid droid)
        {
            parent = droid;
        }

        public void Tick()
        {
            CheckPowerRemaining();
            CheckDowned();
            ResetDrafter();
            CureDiseases();
            RefillNeeds();
        }

        public void Disable()
        {
            if (parent.SpawnedInWorld)
            {
                DeactivatedDroid disabledDroid = (DeactivatedDroid)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed(DeactivatedDroidDefName));
                disabledDroid.InnerDroid = parent;
                IntVec3 pos = parent.Position;
                parent.DeSpawn();
                GenSpawn.Spawn(disabledDroid, pos);
            }
        }

        public void Disable(string reportLabel, string reportString, bool reportLocation = false)
        {
            if (parent.SpawnedInWorld)
            {
                Letter letter = new Letter(reportLabel, reportString, LetterType.BadNonUrgent);
                if (reportLocation)
                    letter.lookTarget = new TargetInfo(parent.Position);
                Find.LetterStack.ReceiveLetter(letter);
                Disable();
            }
        }

        public void CheckPowerRemaining()
        {
            if (!parent.playerController.Drafted)
            {
                if (parent.TotalCharge <= parent.meta.PowerCriticalThreshold && parent.CurJob.def != ChargeJob)
                {
                    Thing target = ListerDroids.ClosestChargerFor(parent);
                    if (target != null)
                    {
                        parent.jobs.StartJob(new Job(ChargeJob, new TargetInfo(target)), JobCondition.InterruptForced);
                    }
                }
            }
            if (parent.TotalCharge < 1f)
            {
                Disable("DroidOutOfPower".Translate(), "DeactivatedNoPower".Translate(), true);
            }
        }

        public void CheckDowned()
        {
            if (parent.Downed)
            {
                Disable("DroidDowned".Translate(), "DeactivatedDowned".Translate(), true);
            }
        }

        public void Inactive()
        {
            if (parent.pather != null && parent.pather.Moving)
                parent.pather.StopDead();
            if ((parent.jobs.curJob == null || parent.jobs.curJob.def != JobDriver_DroidDeactivated.Def) && !parent.stances.FullBodyBusy)
            {
                parent.jobs.StartJob(new Job(JobDriver_DroidDeactivated.Def), JobCondition.InterruptForced);
            }
        }

        private void ResetDrafter()
        {
            if (parent.playerController.Drafted)
            {
                if (Find.TickManager.TicksGame % 4800 == 0)
                {
                    AutoUndrafter undrafter = (AutoUndrafter)typeof(Drafter).GetField("autoUndrafter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(parent.playerController.drafter);
                    if (undrafter != null)
                    {
                        typeof(AutoUndrafter).GetField("lastNonWaitingTick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(undrafter, Find.TickManager.TicksGame);
                        //Log.Message("Reset ticks " + ((int)typeof(AutoUndrafter).GetField("lastNonWaitingTick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(undrafter)).ToString());
                    }
                }
            }
        }

        private void CureDiseases()
        {
            IEnumerable<Hediff_Staged> diseases = parent.health.hediffSet.GetDiseases();
            if (diseases != null)
            {
                foreach (Hediff_Staged d in diseases)
                {
                    d.DirectHeal(1000f);
                }
            }
        }

        private void RefillNeeds()
        {
            if (Find.TickManager.TicksGame % 180 == 0)
            {
                foreach (Need n in parent.needs.AllNeeds)
                {
                    if (n.CurLevel < 90)
                    {
                        n.CurLevel = 100f;
                    }
                }
            }
        }

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                this.active = value;
                if (!active)
                {
                    parent.jobs.StopAll();
                    parent.jobs.StartJob(new Job(JobDriver_DroidDeactivated.Def), JobCondition.InterruptForced);
                }
                else
                {
                    parent.jobs.StopAll();
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.active, "active");
        }
    }
}
