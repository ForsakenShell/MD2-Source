using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class CremationWorker : SpecialistWorker
    {
        private static readonly Predicate<Thing> animalPredicate = (Thing t) => (t is Corpse) && ((Corpse)t).innerPawn.RaceProps.Animal && !((Corpse)t).innerPawn.RaceProps.mechanoid && !((Corpse)t).innerPawn.RaceProps.Humanlike && !t.IsBuried();
        private static readonly Predicate<Thing> humanPredicate = (Thing t) => (t is Corpse) && !((Corpse)t).innerPawn.RaceProps.Animal && !((Corpse)t).innerPawn.RaceProps.mechanoid && ((Corpse)t).innerPawn.RaceProps.Humanlike && !t.IsBuried();
        private static readonly Predicate<Thing> mechanoidPredicate = (Thing t) => (t is Corpse) && !((Corpse)t).innerPawn.RaceProps.Animal && ((Corpse)t).innerPawn.RaceProps.mechanoid && !((Corpse)t).innerPawn.RaceProps.Humanlike && !t.IsBuried();

        private string animalLabel = "AnimalCorpses".Translate();
        private string humanLabel = "HumanoidCorpses".Translate();
        private string mechanoidLabel = "MechanoidCorpses".Translate();

        private CremationTarget targetAnimal;
        private CremationTarget targetHumanLike;
        private CremationTarget targetMechanoid;

        private bool stripBodies = true;
        private readonly Texture2D ShirtIcon = ContentFinder<Texture2D>.Get("UI/Commands/ShirtIcon");

        public CremationWorker(Droid droid) : base(droid)
        {
        }

        public bool StripBodies
        {
            get
            {
                return stripBodies;
            }
        }

        public CremationTarget TargetAnimal
        {
            get
            {
                return targetAnimal;
            }
        }

        public CremationTarget TargetHumanLike
        {
            get
            {
                return targetHumanLike;
            }
        }

        public CremationTarget TargetMechanoid
        {
            get
            {
                return targetMechanoid;
            }
        }

        public IEnumerable<CremationTarget> AllTargets
        {
            get
            {
                yield return TargetAnimal;
                yield return TargetHumanLike;
                yield return TargetMechanoid;
            }
        }

        public override void SpawnSetup()
        {
            targetAnimal = new CremationTarget(animalLabel, animalPredicate, 2, Parent);
            targetHumanLike = new CremationTarget(humanLabel, humanPredicate, 1, Parent);
            targetMechanoid = new CremationTarget(mechanoidLabel, mechanoidPredicate, 3, Parent);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            Command_Toggle c = new Command_Toggle();
            c.isActive = () => stripBodies;
            c.toggleAction = () => stripBodies = !stripBodies;
            c.icon = ShirtIcon;
            c.defaultLabel = stripBodies ? "DontStripBodies".Translate() : "StripBodies".Translate();
            c.defaultDesc = stripBodies ? "DontStripBodiesDesc".Translate() : "StripBodiesDesc".Translate();
            c.groupKey = 1500055;
            yield return c;
        }

        public override void ExposeData()
        {
            Scribe_Values.LookValue(ref stripBodies, "stripBodies");
            Scribe_Deep.LookDeep(ref this.targetAnimal, "animalLike", new object[] { animalLabel, animalPredicate, 2, this });
            Scribe_Deep.LookDeep(ref this.targetHumanLike, "humanLike", new object[] { humanLabel, humanPredicate, 1, this });
            Scribe_Deep.LookDeep(ref this.targetMechanoid, "mechanoid", new object[] { mechanoidLabel, mechanoidPredicate, 3, this });
        }
    }
}
