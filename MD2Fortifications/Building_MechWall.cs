using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MD2
{
    public abstract class Building_MechWall : Building
    {
        public static readonly ThingDef MechWallExtended = DefDatabase<ThingDef>.GetNamed("MechWallExtended");
        public static readonly ThingDef MechWallRecessed = DefDatabase<ThingDef>.GetNamed("MechWallRecessed");
        public static readonly ThingDef MechWallEmbrasure = DefDatabase<ThingDef>.GetNamed("MD2MechEmbrasure");
        public static readonly Texture2D ExtendIcon = ContentFinder<Texture2D>.Get("UI/Commands/DumpContentsUI");
        public static readonly Texture2D RecessIcon = ContentFinder<Texture2D>.Get("UI/Commands/RecessWall");
        public static readonly Texture2D EmbrasureIcon = ContentFinder<Texture2D>.Get("UI/Icons/Metal");
        private static readonly SoundDef sound = SoundDef.Named("ChunkRock_Drop");
        public CompPowerTrader power;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            power = base.GetComp<CompPowerTrader>();
        }
        public virtual void DoDustPuff()
        {
            MoteThrower.ThrowDustPuff(this.Position, 1f);
            sound.PlayOneShot(this.Position);
        }
        public abstract void ToggleStateOne();

        public abstract void ToggleStateTwo();


        public bool HasPower
        {
            get
            {
                return power != null && power.PowerOn;
            }
        }

    }
}
