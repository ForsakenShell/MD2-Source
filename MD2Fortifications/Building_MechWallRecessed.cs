using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Building_MechWallRecessed : Building_MechWall
    {

        public override IEnumerable<Gizmo> GetGizmos()
        {
            var com = new Command_Action()
            {
                action = () =>
                {
                    ToggleStateOne();
                },
                defaultDesc = "Click to extend the wall",
                defaultLabel = "Extend Wall",
                activateSound = SoundDef.Named("Click"),
                hotKey = Keys.Named("ExtendMechWall"),
                disabled = false,
                groupKey = 313740010,
                icon = ExtendIcon
            };
            yield return com;
            com = new Command_Action()
            {
                action = () =>
                {
                    ToggleStateTwo();
                },
                defaultDesc = "Click to extend the wall into an embrasure",
                defaultLabel = "Open embrasure",
                activateSound = SoundDef.Named("Click"),
                hotKey = Keys.Named("ExtendMechEmbrasure"),
                disabled = false,
                groupKey = 313740011,
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
            //Extends the wall
            if (HasPower)
            {
                IntVec3 pos = base.Position;
                Thing wall = ThingMaker.MakeThing(MechWallExtended, this.Stuff);
                wall.SetFactionDirect(Faction.OfColony);
                wall.HitPoints = this.HitPoints;
                this.Destroy();
                DoDustPuff();
                GenSpawn.Spawn(wall, pos);
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
