using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace MD2
{
    public class JobDriver_DroidButcher : JobDriver
    {
        private const TargetIndex CorpseIndex = TargetIndex.A;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Set what will cause the job to fail:
            this.FailOnDestroyedOrForbidden(CorpseIndex);
            this.FailOnBurningImmobile(CorpseIndex);
            this.FailOn(() => !(pawn is Droid) || TargetA.Thing.TryGetComp<CompRottable>() != null && TargetA.Thing.TryGetComp<CompRottable>().Stage != RotStage.Fresh);
            Droid droid = pawn as Droid;
            CremationWorker worker = droid.work.specialist.GetWorker<CremationWorker>();

            //Reserve the corpse
            yield return Toils_Reserve.Reserve(CorpseIndex);
            //Go to the corpse
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch);

            //If wanting to strip bodies, then strip it.
            if (worker.StripBodies)
            {
                yield return Toils_General.Wait(80);
                //Strip the body
                Toil t = new Toil();
                t.initAction = () =>
                {
                    Corpse corpse = (Corpse)TargetA.Thing;
                    if (corpse != null && corpse.AnythingToStrip())
                        corpse.Strip();
                };
                yield return t;
            }

            //Wait the time to do the butchering
            Toil effecterWait = Toils_General.Wait(300);
            effecterWait.WithEffect(() => ((Corpse)TargetA.Thing).innerPawn.RaceProps.isFlesh ? DefDatabase<EffecterDef>.GetNamed("ButcherFlesh") : DefDatabase<EffecterDef>.GetNamed("ButcherMechanoid"), CorpseIndex);
            effecterWait.WithSustainer(() => ((Corpse)TargetA.Thing).innerPawn.RaceProps.isFlesh ? DefDatabase<SoundDef>.GetNamed("Recipe_ButcherCorpseFlesh") : DefDatabase<SoundDef>.GetNamed("Recipe_ButcherCorpseMechanoid"));
            yield return effecterWait;

            Toil toil = new Toil();
            toil.initAction = delegate
            {
                List<Thing> products;
                Corpse c = (Corpse)TargetA.Thing;
                if (c != null)
                {
                    float efficiency = Rand.Range(0.9f, 1f);
                    if (c.innerPawn.RaceProps.isFlesh)
                        products = TargetA.Thing.ButcherProducts(Find.ListerPawns.FreeColonists.RandomElement(), efficiency).ToList();
                    else
                        products = c.innerPawn.ButcherProducts(Find.ListerPawns.FreeColonists.RandomElement(), efficiency).ToList();
                    TargetA.Thing.Destroy();
                    foreach (var thing in products)
                    {
                        if (!GenPlace.TryPlaceThing(thing, pawn.Position, ThingPlaceMode.Near))
                        {
                            Log.Error(string.Concat(new object[]{
                            pawn,
                            " could not drop butcher product ",
                            thing,
                        " near ",
                        pawn.Position
                        }));
                        }
                    }
                }
                else
                {
                    Log.Error(string.Concat(new object[]{
                        pawn,
                        " tried to butcher ",
                        TargetA.Thing,
                        " at ",
                        TargetA.Thing.Position,
                        " which is not a corpse."
                    }));
                    return;
                }
            };
            yield return toil;
        }

    }
}
