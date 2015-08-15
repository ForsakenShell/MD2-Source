using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Designator_CollectClay : Designator
    {
        public Designator_CollectClay()
        {
            this.defaultDesc = "DesignatorCollectClayDesc".Translate();
            this.defaultLabel = "DesignatorCollectClayLabel".Translate();
            this.icon = ContentFinder<Texture2D>.Get("Items/Resources/SoftClay");
            this.soundDragSustain = SoundDefOf.DesignateDragStandard;
            this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
            this.useMouseIcon = true;
            this.soundSucceeded = SoundDefOf.DesignateHarvest;
        }

        public override void SelectedUpdate()
        {
            base.SelectedUpdate();
            GenUI.RenderMouseoverBracket();
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!GenGrid.InBounds(loc))
                return false;
            if (GridsUtility.Fogged(loc))
                return false;
            if (Find.DesignationManager.DesignationAt(loc, DefDatabase<DesignationDef>.GetNamed("MD2CollectClay")) != null)
                return false;
            if (Find.TerrainGrid.TerrainAt(loc) != DefDatabase<TerrainDef>.GetNamed("Mud") && Find.TerrainGrid.TerrainAt(loc) != DefDatabase<TerrainDef>.GetNamed("WaterShallow"))
            {
                //Log.Message(Find.TerrainGrid.TerrainAt(loc).defName);
                return "DesignatorCollectClayReportString".Translate();
            }
            return AcceptanceReport.WasAccepted;
        }

        public override bool DragDrawMeasurements
        {
            get
            {
                return true;
            }
        }

        public override int DraggableDimensions
        {
            get
            {
                return 2;
            }
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Find.DesignationManager.AddDesignation(new Designation(c, DefDatabase<DesignationDef>.GetNamed("MD2CollectClay")));
        }
    }
}
