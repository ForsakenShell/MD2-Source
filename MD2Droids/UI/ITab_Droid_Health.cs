using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class ITab_Droid_Health : ITab
    {
        private const float TopPadding = 20f;
        private const int HideBloodLossTicksThreshold = 60000;
        public ITab_Droid_Health()
        {
            this.size = new Vector2(630f, 430f);
            this.labelKey = "TabHealth";
        }

        public override bool IsVisible
        {
            get
            {
                if(base.SelThing is DeactivatedDroid)
                {
                    DeactivatedDroid droid = base.SelThing as DeactivatedDroid;
                    return (droid !=null && droid.InnerDroid != null);
                }

                if(base.SelThing is Building_RepairStation)
                {
                    Building_RepairStation rps = base.SelThing as Building_RepairStation;
                    return rps != null && !rps.IsAvailableForReactivation;
                }
                return base.SelThing is Corpse || base.SelThing is Pawn;
            }
        }

        protected override void FillTab()
        {
            Pawn pawn = null;
            if (base.SelPawn != null)
            {
                pawn = base.SelPawn;
            }
            else if (base.SelThing is Corpse)
            {
                Corpse corpse = base.SelThing as Corpse;
                if (corpse != null)
                {
                    pawn = corpse.innerPawn;
                }
            }
            else if (base.SelThing is DeactivatedDroid)
            {
                DeactivatedDroid droid = base.SelThing as DeactivatedDroid;
                if (droid != null)
                    pawn = droid.InnerDroid;
            }
            else if (base.SelThing is Building_RepairStation)
            {
                Building_RepairStation rps = base.SelThing as Building_RepairStation;
                if (rps != null)
                    pawn = rps.InnerDroid.InnerDroid;
            }
            if (pawn == null)
            {
                Log.Error("Health tab found no selected pawn to display.");
                return;
            }
            Corpse corpse2 = base.SelThing as Corpse;
            bool showBloodLoss = corpse2 == null || corpse2.Age < 60000;
            bool flag = base.SelThing.def.AllRecipes.Any<RecipeDef>();
            bool flag2 = !pawn.RaceProps.Humanlike && pawn.Downed;
            bool allowOperations = flag && !pawn.Dead && (pawn.IsColonist || pawn.HostFaction == Faction.OfColony || flag2);
            Rect outRect = new Rect(0f, 20f, this.size.x, this.size.y - 20f);
            HealthCardUtility.DrawPawnHealthCard(outRect, pawn, allowOperations, showBloodLoss, base.SelThing);
        }

    }
}
