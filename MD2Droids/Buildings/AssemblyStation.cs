using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace MD2
{
    public class AssemblyStation : Building
    {
        private AssemblyBillStack _assemblyBillStack;

        public AssemblyStation()
        {
            _assemblyBillStack = new AssemblyBillStack(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.LookDeep(ref _assemblyBillStack, "billStack", this);
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        public AssemblyBillStack AssemblyBillStack
        {
            get { return _assemblyBillStack; }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
                yield return g;

            Command_Action action = new Command_Action();
            action.action = () =>
            {
                Droid droid = Blueprint.GenerateDroid(Blueprint.Default, Faction.OfColony);
                droid.parts.ReplacePowerCell(ThingMaker.MakeThing(ThingDef.Named("MD2DroidPowerCell")));
                GenSpawn.Spawn(droid, base.InteractionCell);
            };
            action.defaultLabel = "click";
            action.groupKey = 10008999;
            yield return action;
        }
    }
}
