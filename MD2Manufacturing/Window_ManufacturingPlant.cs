using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public abstract class Window_ManufacturingPlant : Window
    {
        protected string message = "";
        protected AssemblyLine line;
        public static readonly Vector2 WinSize = GenUI.MaxWinSize;

        public Window_ManufacturingPlant(string message)
            : this(null, message)
        {

        }
        public Window_ManufacturingPlant(AssemblyLine line, string message)
        {
            this.message = message;
            this.line = line;
            this.absorbInputAroundWindow = true;
            this.closeOnEscapeKey = true;
            this.forcePause = false;
            this.doCloseButton = true;
            this.doCloseX = true;
            this.draggable = true;
            this.resizeable = true;
        }

        public override Vector2 InitialWindowSize
        {
            get
            {
                return WinSize;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (!message.NullOrEmpty())
            {
                Rect helpRect = new Rect(0, 0, 20, 20);
                if (Widgets.TextButton(helpRect, "?"))
                {
                    Find.WindowStack.Add(new Dialog_Message(message, "Help"));
                }
                TooltipHandler.TipRegion(helpRect, "DialogHelp".Translate());
            }
        }
    }
}
