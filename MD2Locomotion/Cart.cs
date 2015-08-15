using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace MD2
{
    public class Cart : ThingWithComps
    {
        public Cart_Pather pather;
        public Cart_DrawerTracker drawer;
        public Cart_GraphicSet graphics;

        public Cart()
        {
            //Placeholders, will be moved to cart generator.
            pather = new Cart_Pather(this);
            drawer = new Cart_DrawerTracker(this);

            graphics = new Cart_GraphicSet(this);
        }
        
        public CartDef Def
        {
            get
            {
                return this.def as CartDef;
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach(var g in base.GetGizmos())
            {
                yield return g;
            }

            Command_Toggle com = new Command_Toggle();
            if(pather.Stopped)
            {
                com.defaultLabel="Continue";
                com.defaultDesc = "Click to continue the cart moving";
            }
            else
            {
                com.defaultLabel = "Stop";
                com.defaultDesc = "Click to stop the cart";
            }
            com.groupKey = 13008961;
            //com.icon=
            com.isActive = () => { return pather.Stopped; };
            com.toggleAction = () => { pather.ToggleStopped(); };
            yield return com;
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            graphics.ResolveGraphics();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.LookDeep(ref this.pather, "pather", this);
            Scribe_Deep.LookDeep(ref this.drawer, "drawer", this);
        }

        public override Vector3 DrawPos
        {
            get
            {
                return this.drawer.DrawPos;
            }
        }

        public override void DrawAt(Vector3 pos)
        {
            drawer.DrawAt(pos);
        }

        public int TicksPerMove(bool diagonal = false)
        {
            float cellsPerSecond = this.GetStatValue(StatDefOf.MoveSpeed, false);
            float num = cellsPerSecond / 60f;
            float num2 = 1f / num;
            if (!Find.RoofGrid.Roofed(base.Position))
            {
                num2 /= Find.WeatherManager.CurMoveSpeedMultiplier;
            }
            int value = Mathf.RoundToInt(num2);
            return Mathf.Clamp(value, 1, 450);
        }

        public override void Tick()
        {
            base.Tick();
            pather.Tick();
            drawer.Tick();
        }
    }
}
