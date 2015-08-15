using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public static partial class Widget
    {
        public static readonly Color BgColor = new Color(0.1647f, 0.1647f, 0.1647f, 1f);
        public static readonly Texture2D BgTex = SolidColorMaterials.NewSolidColorTexture(BgColor);
        public static readonly Texture2D TextureChooserBg = ContentFinder<Texture2D>.Get("UI/TextureChooserBG");

        public static void LabelTextField(Rect rect, string labelText, ref string textBoxText)
        {
            try
            {
                GUI.BeginGroup(rect);
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect labelRect = new Rect(0f, 0f, 60f, rect.height);
                Widgets.Label(labelRect, labelText);
                Text.Anchor = TextAnchor.UpperLeft;
                Rect textBoxRect = new Rect(labelRect.width + 4f, 0f, rect.width - labelRect.width - 4f,
                    rect.height);
                textBoxText = Widgets.TextField(textBoxRect, textBoxText);
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.EndGroup();
            }
        }

        public static void ScrollBoxWithButton(Rect inRect, string labelText, List<WorkPackageDef> workPackages, ref Vector2 scrollPosition, string buttonText,
    bool doButton, Action buttonAction, string labelTooltip = null, string buttonTooltip = null, TextAnchor textPosition = TextAnchor.MiddleCenter)
        {
            try
            {
                GUI.BeginGroup(inRect);

                //Draw the label
                Rect labelRect = new Rect(0f, 0f, inRect.width, 30f);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(labelRect, labelText);
                Text.Anchor = TextAnchor.UpperLeft;
                if (labelTooltip != null) TooltipHandler.TipRegion(labelRect, labelTooltip);

                //Draw the button
                if (doButton)
                {
                    Rect buttonRect = new Rect(inRect.width / 2f - 60f, inRect.height - 30f, 120f, 30f);
                    if (Widgets.TextButton(buttonRect, buttonText))
                    {
                        buttonAction();
                    }
                    if (buttonTooltip != null) TooltipHandler.TipRegion(buttonRect, buttonTooltip);
                }

                //Draw the scroll box
                Vector2 scrollBoxSize = new Vector2(inRect.width, inRect.height - 30f - 4f);
                if (doButton) scrollBoxSize.y -= 34f;

                Rect outRect = new Rect(0f, labelRect.height + 4f, scrollBoxSize.x, scrollBoxSize.y);
                Widgets.DrawMenuSection(outRect);
                float curY = outRect.y;
                bool alternate = false;
                float height = workPackages.Count * 30f;
                Rect viewRect = new Rect(outRect.x, outRect.y, outRect.width - 16f, height);
                if (workPackages.Count == 0)
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(outRect, "NoWorkPackagesSelected".Translate());
                    Text.Anchor = TextAnchor.UpperLeft;
                }
                else
                {
                    Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
                    foreach (var p in workPackages)
                    {
                        Rect entryRect = new Rect(viewRect.x, curY, viewRect.width, 30f);
                        DrawScrollBoxEntry(entryRect, outRect, p, alternate);

                        alternate = !alternate;
                        curY += 30f;
                    }
                    Widgets.EndScrollView();
                }
            }
            finally
            {
                //End inRect group
                GUI.EndGroup();
            }
        }

        private static void DrawScrollBoxEntry(Rect inRect, Rect outRect, WorkPackageDef def, bool alternate)
        {
            try
            {
                if (inRect.Contains(Event.current.mousePosition) && outRect.Contains(Event.current.mousePosition))
                {
                    Widgets.DrawHighlight(inRect);
                    TooltipHandler.TipRegion(inRect, def.Tooltip);
                }
                else if (alternate)
                    Widgets.DrawAltRect(inRect);

                GUI.BeginGroup(inRect);
                Rect labelRect = new Rect(0f, 0f, inRect.width, inRect.height);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(labelRect, def.label);
                Text.Anchor = TextAnchor.UpperLeft;
            }
            finally
            {
                GUI.EndGroup();
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }
    }
}
