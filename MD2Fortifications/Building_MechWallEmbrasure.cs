using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace MD2
{
    public class Building_MechWallEmbrasure : Building_MechWall
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
                defaultDesc = "Click to recess the wall into the ground",
                defaultLabel = "Recess Wall",
                activateSound = SoundDef.Named("Click"),
                hotKey = Keys.Named("RecessMechWall"),
                disabled = false,
                groupKey = 313740009,
                icon = RecessIcon
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
    }
}
