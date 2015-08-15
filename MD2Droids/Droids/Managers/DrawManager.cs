using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace MD2
{
    public class DrawManager : IExposable
    {
        private Droid parent;
        private DroidUIOverlay uiOverlay;

        private GraphicDef headGraphicDef;
        private GraphicDef bodyGraphicDef;

        private Graphic headGraphic;
        private Graphic bodyGraphic;

        public DrawManager(Droid droid)
        {
            this.parent = droid;
        }

        public DrawManager(GraphicDef bodyGraphicPath, GraphicDef headGraphicPath, Droid droid)
        {
            this.bodyGraphicDef = bodyGraphicPath;
            this.headGraphicDef = headGraphicPath;
            this.parent = droid;
        }

        public void SpawnSetup()
        {
            uiOverlay = new DroidUIOverlay(parent);
            parent.drawer.renderer.graphics.ResolveGraphics();
            parent.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved", true);


            try
            {
                if (headGraphicDef != null && bodyGraphicDef.supportsHead)
                {
                    headGraphic = headGraphicDef.graphicData.Graphic;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return;
            }
            try
            {
                bodyGraphic = bodyGraphicDef.graphicData.Graphic;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            if (headGraphic != null)
            {
                parent.drawer.renderer.graphics.headGraphic = this.headGraphic;
                parent.drawer.renderer.graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(parent.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, parent.story.hairColor);
            }
            parent.drawer.renderer.graphics.nakedGraphic = this.bodyGraphic;
        }

        internal void DrawGUIOverlay()
        {
            uiOverlay.DrawGUIOverlay();
        }

        public void ExposeData()
        {
            Scribe_Defs.LookDef(ref this.headGraphicDef, "headGraphicDef");
            Scribe_Defs.LookDef(ref this.bodyGraphicDef, "bodyGraphicDef");
        }
    }
}
