using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Dialog_UpgradeManager : Dialog_ManufacturingPlant
    {
        private float padding = 4f;

        public Dialog_UpgradeManager(AssemblyLine parent)
            : base(parent, "Upgrades".Translate(), "UpgradeManagerHelp".Translate(), 600, 600)
        {
            this.doCloseButton = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);

            Rect infoPaneRect = new Rect(0, currentY, inRect.width, 60f);
            Rect infoLabelRect = infoPaneRect.ContractedBy(4f);

            base.currentY += infoPaneRect.yMax + 10f;
            Widgets.DrawMenuSection(infoPaneRect);
            //Hardcoded because I can't think how to do it :L
            string info = string.Concat(new object[]{
                parent.Speed.Label,
                ": ",
                (parent.Speed.Value).ToString("0.0"),
                "x\n",
                parent.Efficiency.Label,
                ": ",
                (parent.Efficiency.Value).ToString("0.0"),
                "x"
            });

            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(infoLabelRect, info);
            Text.Anchor = TextAnchor.UpperLeft;

            Rect rect = new Rect(0, base.currentY, inRect.width, inRect.height - base.currentY - (CloseButSize.y + padding));
            base.currentY += rect.yMax;
            parent.UpgradeManager.OnGUI(rect);

            base.ResetVariables();
        }
    }
}
