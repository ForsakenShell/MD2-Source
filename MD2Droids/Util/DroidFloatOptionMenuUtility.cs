using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public static class DroidFloatOptionMenuUtility
    {
        public static IEnumerable<FloatMenuOption> ExtraFloatMenuOptions(this Droid droid, IntVec3 sq, IEnumerable<FloatMenuOption> options)
        {
            foreach (var thing in sq.GetThingList())
            {
                CompMannable mannable = thing.TryGetComp<CompMannable>();
                if (mannable != null && !mannable.MannedNow)
                {
                    yield return new FloatMenuOption("DroidManThing".Translate(thing.Label), delegate
                    {
                        droid.jobs.StartJob(new Job(JobDefOf.ManTurret, thing), JobCondition.InterruptForced);
                    });
                }
                if (droid.equipment != null && (thing.def.IsMeleeWeapon || thing.def.IsRangedWeapon))
                {
                    yield return new FloatMenuOption("Equip".Translate(thing.Label), delegate
                    {
                        CompForbiddable f = thing.TryGetComp<CompForbiddable>();
                        if (f != null && f.Forbidden)
                        {
                            f.Forbidden = false;
                        }
                        droid.jobs.StartJob(new Job(JobDefOf.Equip, thing), JobCondition.InterruptForced);
                    });
                }
                if (thing is Pawn)
                {
                    Pawn p = thing as Pawn;
                    if (p.RaceProps.intelligence >= Intelligence.Humanlike)
                    {
                        if (droid.playerController.Drafted && p.CanBeArrested() && droid.CanReserveAndReach(p, PathEndMode.Touch, Danger.Some))
                        {

                            //Try to arrest
                            yield return new FloatMenuOption("TryToArrest".Translate(p.LabelCap), delegate
                            {
                                Building_Bed bed = Find.ListerBuildings.AllBuildingsColonistOfClass<Building_Bed>().Where((Building_Bed b) => b.ForPrisoners && b.owner == null).FirstOrDefault();
                                if (bed != null)
                                {
                                    Job job = new Job(JobDefOf.Arrest, p, bed);
                                    job.maxNumToCarry = 1;
                                    droid.jobs.StartJob(job, JobCondition.InterruptForced);
                                }
                                else
                                {
                                    Messages.Message("DroidArrestNoBedsAvailable".Translate(), MessageSound.RejectInput);
                                }
                            });
                        }
                        if (p.Downed)
                        {
                            if (droid.work.Contains(WorkTypeDefOf.Doctor) && droid.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
                            {
                                if (!p.HostileTo(Faction.OfColony) && p.def.race.intelligence >= Intelligence.Humanlike)
                                {
                                    //Rescue a downed colonist or visitor
                                    yield return new FloatMenuOption("Rescue".Translate(p.LabelCap), delegate
                                    {
                                        Building_Bed freeBed = Find.ListerBuildings.AllBuildingsColonistOfClass<Building_Bed>().Where(b =>
                                            (b.Medical ||
                                            b.owner == p ||
                                            b.owner == null) &&
                                            b.CurOccupant == null && !b.ForPrisoners).OrderByDescending(b => (b.def.building != null) ? b.def.building.bed_medicalBonusFactor : 0f).FirstOrDefault();
                                        if (freeBed != null)
                                        {
                                            Job job = new Job(JobDefOf.Rescue, p, freeBed);
                                            job.maxNumToCarry = 1;
                                            droid.jobs.StartJob(job, JobCondition.InterruptForced);
                                        }
                                        else
                                        {
                                            Messages.Message("DroidRescueNoMedicalBedsAvailable".Translate(), MessageSound.RejectInput);
                                        }
                                    });
                                }
                                if (p.HostileTo(Faction.OfColony))
                                {
                                    //Capture a downed enemy or broken colonist
                                    yield return new FloatMenuOption("Capture".Translate(p.LabelCap), delegate
                                    {
                                        Building_Bed freeBed = Find.ListerBuildings.AllBuildingsColonistOfClass<Building_Bed>().Where(b =>
                                            (b.Medical ||
                                            b.owner == p ||
                                            b.owner == null)
                                            && b.CurOccupant == null && b.ForPrisoners).OrderByDescending(b => (b.def.building != null) ? b.def.building.bed_medicalBonusFactor : 0f).FirstOrDefault();
                                        if (freeBed != null)
                                        {
                                            Job job = new Job(JobDefOf.Capture, p, freeBed);
                                            job.maxNumToCarry = 1;
                                            droid.jobs.StartJob(job, JobCondition.InterruptForced);
                                        }
                                        else
                                        {
                                            Messages.Message("DroidArrestNoBedsAvailable".Translate(), MessageSound.RejectInput);
                                        }
                                    });
                                }
                            }
                            if (p.AnythingToStrip())
                            {
                                yield return new FloatMenuOption("Strip".Translate(p.LabelCap), delegate
                                {
                                    droid.jobs.StartJob(new Job(JobDefOf.Strip, p), JobCondition.InterruptForced);
                                });
                            }
                        }
                    }
                }
                if (thing is Corpse)
                {
                    Corpse c = thing as Corpse;
                    if (c.AnythingToStrip())
                    {
                        yield return new FloatMenuOption("Strip".Translate(c.LabelCap), delegate
                        {
                            droid.jobs.StartJob(new Job(JobDefOf.Strip, c), JobCondition.InterruptForced);
                        });
                    }
                }
            }
            foreach (var o in options)
            {
                yield return o;
            }

        }
    }
}
