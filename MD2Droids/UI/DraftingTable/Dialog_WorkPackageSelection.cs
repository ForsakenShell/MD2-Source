using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Dialog_WorkPackageSelection : Layer_Window
    {
        private Vector2 WinSize = new Vector2(700f, 500f);
        private Blueprint _bp;
        private readonly Func<WorkPackageDef, bool> _displayableFunc;

        private const float Margin = 18f;
        private Vector2 _entrySize;

        private Vector2 _availableScrollPosition = default(Vector2);
        private Vector2 _selectedScrollPosition = default(Vector2);

        public Dialog_WorkPackageSelection(ref Blueprint bp, Func<WorkPackageDef, bool> displayableFunc)
        {
            _bp = bp;
            _displayableFunc = displayableFunc;
            drawPriority = 2000;
            clearNonEditWindows = false;
            absorbAllInput = true;
            forcePause = true;
            closeOnEscapeKey = true;
            doCloseX = true;
            base.SetCentered(WinSize.x, WinSize.y);
        }

        protected override void FillWindow(Rect inRect)
        {
            Rect innerRect = inRect.ContractedBy(Margin);
            try
            {
                GUI.BeginGroup(innerRect);
                float midPoint = innerRect.width / 2f;
                Vector2 labelSize = new Vector2(innerRect.width / 2 - 40f, 30f);
                Vector2 menuSize = new Vector2(innerRect.width / 2 - 40f, innerRect.height - 30f - 10f - labelSize.y - 4f);

                Rect availableLabelRect = new Rect(0f, 0f, labelSize.x, labelSize.y);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(availableLabelRect, "AvailablePackages".Translate());

                Rect selectedLabelRect = new Rect(midPoint + 40f, 0f, labelSize.x, labelSize.y);
                Widgets.Label(selectedLabelRect, "SelectedPackages".Translate());
                Text.Anchor = TextAnchor.UpperLeft;

                Rect availableRect = new Rect(0f, labelSize.y + 4f, menuSize.x, menuSize.y);
                _entrySize = new Vector2(availableRect.width - 16f, 30f);
                float availableHeight = (from t in DefDatabase<WorkPackageDef>.AllDefs
                                         where t.displayInMenu && _displayableFunc(t) && !_bp.WorkPackages.Contains(t)
                                         select t).Count() * _entrySize.y;
                Rect availableViewRect = new Rect(0f, availableRect.y, menuSize.x - 16f, availableHeight);
                float availableY = availableRect.y;
                bool alternate = false;
                Widgets.DrawMenuSection(availableRect);
                Widgets.BeginScrollView(availableRect, ref _availableScrollPosition, availableViewRect);
                foreach (var p in (from t in DefDatabase<WorkPackageDef>.AllDefs
                                   where t.displayInMenu && _displayableFunc(t) && !_bp.WorkPackages.Contains(t)
                                   orderby t.label
                                   select t))
                {
                    Rect entryRect = new Rect(0f, availableY, _entrySize.x, _entrySize.y);
                    DrawAvailableEntry(p, entryRect, alternate);
                    availableY += _entrySize.y;
                    alternate = !alternate;
                }
                Widgets.EndScrollView();


                Rect selectedRect = new Rect(midPoint + 40f, labelSize.y + 4f, menuSize.x, menuSize.y);
                float selectedHeight = (from t in _bp.WorkPackages
                                        where t.displayInMenu && _displayableFunc(t)
                                        orderby t.label
                                        select t).Count() * _entrySize.y;
                availableY = selectedRect.y;
                alternate = false;
                Rect selectedViewRect = new Rect(selectedRect.x, selectedRect.y, menuSize.x - 16f, selectedHeight);
                Widgets.DrawMenuSection(selectedRect);

                Widgets.BeginScrollView(selectedRect, ref _selectedScrollPosition, selectedViewRect);
                foreach (var p in (from t in _bp.WorkPackages
                                   where t.displayInMenu && _displayableFunc(t)
                                   select t).ToList())
                {
                    Rect entryRect = new Rect(selectedRect.x, availableY, _entrySize.x, _entrySize.y);
                    DrawSelectedEntry(p, entryRect, alternate);
                    availableY += _entrySize.y;
                    alternate = !alternate;
                }
                Widgets.EndScrollView();

                Rect buttonRect = new Rect(midPoint - 60f, innerRect.height - 30f, 120f, 30f);
                if (Widgets.TextButton(buttonRect, "Accept".Translate()))
                {
                    Close();
                }

                string powerUsage = "PowerRequirement".Translate((from t in _bp.WorkPackages
                    select t.PowerRequirement).Sum().ToString("0.#"));
                Vector2 powerRequirementSize = Text.CalcSize(powerUsage);
                Rect powerUsageRect = new Rect(innerRect.width - powerRequirementSize.x,
                    innerRect.height - powerRequirementSize.y, powerRequirementSize.x, powerRequirementSize.y);
                Widgets.Label(powerUsageRect, powerUsage);
            }
            finally
            {
                GUI.EndGroup();
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        private void DrawAvailableEntry(WorkPackageDef p, Rect entryRect, bool alternate)
        {
            try
            {
                if (entryRect.Contains(Event.current.mousePosition))
                {
                    Widgets.DrawHighlight(entryRect);
                }
                else if (alternate)
                {
                    Widgets.DrawAltRect(entryRect);
                }
                TooltipHandler.TipRegion(entryRect, p.Tooltip);

                GUI.BeginGroup(entryRect);

                Rect labelRect = new Rect(0f, 0f, entryRect.width - 25f, entryRect.height);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, p.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;

                Rect buttonRect = new Rect(entryRect.width - 25f, entryRect.height / 2 - 12.5f, 25f, 25f);
                if (Widgets.ImageButton(buttonRect, Widget.ArrowRightTex, Color.white))
                {
                    _bp.WorkPackages.Add(p);
                }
            }
            finally
            {
                GUI.EndGroup();
            }

        }

        private void DrawSelectedEntry(WorkPackageDef p, Rect entryRect, bool alternate)
        {
            try
            {
                if (entryRect.Contains(Event.current.mousePosition))
                {
                    Widgets.DrawHighlight(entryRect);
                }
                else if (alternate)
                {
                    Widgets.DrawAltRect(entryRect);
                }
                TooltipHandler.TipRegion(entryRect, p.Tooltip);

                GUI.BeginGroup(entryRect);

                Rect labelRect = new Rect(0f, 0f, entryRect.width - 25f, entryRect.height);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, p.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;

                Rect buttonRect = new Rect(entryRect.width - 25f, entryRect.height / 2 - 12.5f, 25f, 25f);
                if (Widgets.ImageButton(buttonRect, Widget.ArrowLeftTex, Color.white))
                {
                    _bp.WorkPackages.Remove(p);
                }
            }
            finally
            {
                GUI.EndGroup();
            }

        }

    }
}
