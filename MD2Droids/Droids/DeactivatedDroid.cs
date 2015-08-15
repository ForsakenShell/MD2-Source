using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class DeactivatedDroid : ThingWithComps
    {
        private Droid innerDroid;

        public Droid InnerDroid
        {
            get
            {
                return innerDroid;
            }
            set
            {
                innerDroid = value;
            }
        }

        public override string Label
        {
            get
            {
                if (InnerDroid != null)
                    return InnerDroid.LabelBase;
                return base.Label;
            }
        }

        public override string LabelBase
        {
            get
            {
                return Label;
            }
        }

        public override void DrawGUIOverlay()
        {
            if (Find.CameraMap.CurrentZoom == CameraZoomRange.Closest)
            {
                GenWorldUI.DrawThingLabel(this, "Deactivated".Translate() + ": " + this.Label);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.LookDeep(ref this.innerDroid, "innerDroid");
        }
    }
}
