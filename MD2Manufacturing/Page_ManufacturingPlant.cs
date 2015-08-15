using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public abstract class Page_ManufacturingPlant : Layer_Window
    {
        protected string message = "";
        protected AssemblyLine line;
        public static readonly Vector2 WinSize = GenUI.MaxWinSize;

        public Page_ManufacturingPlant(string message):this(null,message)
        {

        }
        public Page_ManufacturingPlant(AssemblyLine line, string message)
        {
            this.message = message;
            this.line = line;
            this.drawPriority = 2000;
            base.SetCentered(WinSize);
            this.clearNonEditWindows = true;
            this.absorbAllInput = true;
            this.closeOnEscapeKey = true;
            this.forcePause = false;
            this.doCloseButton = true;
            this.doCloseX = true;

        }

        protected override void FillWindow(Rect inRect)
        {
            if (!message.NullOrEmpty())
            {
                Rect helpRect = new Rect(0, 0, 20, 20);
                if (Widgets.TextButton(helpRect, "?"))
                {
                    Find.LayerStack.Add(new Dialog_Message(message, "Help"));
                }
                TooltipHandler.TipRegion(helpRect, "DialogHelp".Translate());
            }
        }
    }
}
