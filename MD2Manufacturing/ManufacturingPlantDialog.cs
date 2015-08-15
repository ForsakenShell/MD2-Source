using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class ManufacturingPlantDialog : Layer_Window
    {
        protected AssemblyLine parent;

        public ManufacturingPlantDialog(AssemblyLine parent, string title = "", string helpMessage="", float width=600, float height=700)
        {
            base.SetCentered(width, height);
            this.absorbAllInput = true;
            this.drawPriority = 2000;
            this.forcePause = false;
            this.clearNonEditWindows = false;
            this.closeOnEscapeKey = true;
            this.doCloseX = true;
            this.message = helpMessage;
            this.title = title;
            this.parent = parent;
        }

        protected string title = "";
        protected string message = "";
        protected float currentY = 0f;
        protected override void FillWindow(Rect inRect)
        {
            if (!title.NullOrEmpty())
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Text.Font = GameFont.Medium;
                Rect labelRect = new Rect(0f, 0f, inRect.width, 40f);
                Widgets.Label(labelRect, title);
                currentY += 45f;
                Widgets.DrawLineHorizontal(0f, currentY, inRect.width);
                currentY += 10f;
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
            if (!message.NullOrEmpty())
            {
                Rect helpRect = new Rect(0, 0, 20, 20);
                if (Widgets.TextButton(helpRect, "?"))
                {
                    Find.LayerStack.Add(new Dialog_Message(message, "Help".Translate()));
                }
                TooltipHandler.TipRegion(helpRect, "DialogHelp".Translate());
            }
        }
        protected virtual void ResetVariables()
        {
            currentY = 0f;
        }
    }
}
