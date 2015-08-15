using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MD2
{
    public class Page_ManufacturingPlantMainUI : Page_ManufacturingPlant
    {
        public Page_ManufacturingPlantMainUI()
            : base("MainUIHelp".Translate())
        {
        }
        private Vector2 scrollPosition = default(Vector2);
        public List<AssemblyLine> assemblyLines = MPmanager.manager.AssemblyLines;
        private Vector2 lineEntrySize;
        private Vector2 interactButtonSize;
        private Vector2 lineManagerButtonSize = new Vector2(120f, 40f);

        protected override void FillWindow(Rect inRect)
        {
            base.FillWindow(inRect);

            //First we draw the header
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(new Rect(0f, 0f, 300f, 300f), "ManufacturingPlant".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            //This float is used to set the padding at the bottom of the window.
            float bottomPaddingHeight = 50f;

            //The mainRect is the rectangle which contains the listbox
            Rect mainRect = new Rect(0f, bottomPaddingHeight, inRect.width, inRect.height - bottomPaddingHeight - 50f);

            //We initialise the entry and button sizes according to the main rect, which is sized depending on the window size. This lets the window be resized easily without (hoepfully) creating too many problems.
            this.lineEntrySize = new Vector2(mainRect.width - 16f, 100f);
            this.interactButtonSize = new Vector2(100f, this.lineEntrySize.y / 2 - 14f);

            //We begin the group to tell the system that the following elements are grouped together.
            GUI.BeginGroup(mainRect);

            //This float is the height of the entry with a small margin added to it.
            float entryHeightWithMargin = this.lineEntrySize.y + 8f;

            //This float is the height of the entire list with all included entries and margins. This is used for the rect which houses all the entries, even though we wont see all of them at once.
            float height = (float)assemblyLines.Count * entryHeightWithMargin;

            //This rect is used inside the scroll view, and is moved up and down to show the entries.
            Rect viewRect = new Rect(0f, 0f, mainRect.width - 16f, height);

            //This is the rect on the screen to be used for the ScrollView
            Rect outRect = new Rect(mainRect.AtZero());

            //This function starts the scrolling view, and uses the scrollPosition variable to record where in the scrollview we are looking
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);

            //This float is the current y position in the scrollview, we use this for drawing each entry under each other.
            float currentY = 0f;

            //We first check to see if there are any entries. If no, then give a small message
            if (assemblyLines.Count == 0)
            {
                Rect rect2 = new Rect(0f, currentY, this.lineEntrySize.x, this.lineEntrySize.y);
                Text.Font = GameFont.Small;
                Widgets.Label(rect2, "NoAssemblyLines".Translate());
            }

                //If there are entries, then we draw the entry for each element in the list, calling our Draw...() function.
            else
            {
                foreach (AssemblyLine line in assemblyLines)
                {
                    line.OnGUI(currentY, this.lineEntrySize, this.interactButtonSize, this);
                    //Increment the current y position for the next entry to be drawn
                    currentY += entryHeightWithMargin;
                }
            }
            //Must remember to end the view once it has been drawn.
            Widgets.EndScrollView();
            //Likewise, remember to end the group
            GUI.EndGroup();

            //This draws the button in the bottom right corner of the window. It uses the same size as the Close button
            Rect lineManagerButRect = new Rect(inRect.width - this.lineManagerButtonSize.x, inRect.height - this.lineManagerButtonSize.y, this.lineManagerButtonSize.x, this.lineManagerButtonSize.y);
            if (Widgets.TextButton(lineManagerButRect, "ConstructNewAssemblyLine".Translate()))
            {
                if (MPmanager.manager.CanAddAssemblyLine)
                {
                    string costString = "";
                    if (Game.GodMode && AssemblyLine.Settings.instaBuild)
                        costString = "Nothing".Translate();
                    else
                    {
                        foreach (var item in AssemblyLine.Settings.BuildingCost)
                        {
                            costString += string.Format("{1} {0}\n", item.thing.label, item.amount);
                        }
                    }
                    Find.LayerStack.Add(new Dialog_Confirm("BuildNewAssemblyLineDialog".Translate(costString, TicksToTime.GetTime((float)AssemblyLine.ConstructionTicksRequired)), delegate
                    {
                        MPmanager.manager.AddNewAssemblyLine((Game.GodMode && AssemblyLine.Settings.instaBuild));
                    }));
                }
                else
                {
                    Dialog_Message m = new Dialog_Message("MaximumAssemblyLines".Translate());
                    Find.LayerStack.Add(m);
                    SoundDefOf.ClickReject.PlayOneShot(SoundInfo.OnCamera());
                }
            }
        }


    }
}
