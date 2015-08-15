using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Building_MechWallExtended : Building_MechWall
    {

        public override IEnumerable<Gizmo> GetGizmos()
        {
            var com = new Command_Action()
            {
                action = () =>
                {
                    ToggleStateOne();
                },
                defaultDesc = "Click to recess the wall into the ground",
                defaultLabel = "Recess Wall",
                activateSound = SoundDef.Named("Click"),
                hotKey = Keys.Named("RecessMechWall"),
                disabled = false,
                groupKey = 313740009,
                icon = RecessIcon
            };
            yield return com;
            com = new Command_Action()
            {
                action = () =>
                {
                    ToggleStateTwo();
                },
                defaultDesc = "Click to open the wall as an embrasure",
                defaultLabel = "Open embrasure",
                activateSound = SoundDef.Named("Click"),
                hotKey = Keys.Named("ExtendMechEmbrasure"),
                disabled = false,
                groupKey = 313740010,
                icon = EmbrasureIcon
            };
            yield return com;
            if (base.GetGizmos() != null)
            {
                foreach (Command c in base.GetGizmos())
                {
                    yield return c;
                }
            }
        }

        public override void ToggleStateOne()
        {
            //Lowers the wall
            if (HasPower)
            {
                IntVec3 pos = base.Position;
                Thing floor = ThingMaker.MakeThing(MechWallRecessed, this.Stuff);
                floor.SetFactionDirect(Faction.OfColony);
                floor.HitPoints = this.HitPoints;
                this.Destroy();
                DoDustPuff();
                GenSpawn.Spawn(floor, pos);
            }
        }

        public override void ToggleStateTwo()
        {
            //Opens an embrasure
            if (HasPower)
            {
                IntVec3 pos = base.Position;
                Thing emb = ThingMaker.MakeThing(MechWallEmbrasure, this.Stuff);
                emb.SetFactionDirect(Faction.OfColony);
                emb.HitPoints = this.HitPoints;
                this.Destroy();
                DoDustPuff();
                GenSpawn.Spawn(emb, pos);
            }
        }
    }
}
