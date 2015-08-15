using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class OTab_Droid_Work : OTab
    {
        private static List<WorkTypeDef> VisibleWorkTypeDefsInPriorityOrder;

        protected Vector2 scrollPosition = Vector2.zero;

        private const float TopAreaHeight = 40f;
        protected const float LabelRowHeight = 50f;
        private float workColumnSpacing = -1f;

        public OTab_Droid_Work()
        {
            this.title = "DroidWorkOTab".Translate();
            this.orderPriority = 999;
            Reinit();
        }

        public void Reinit()
        {
            OTab_Droid_Work.VisibleWorkTypeDefsInPriorityOrder = DroidWorkTypeDefs.DroidWorkTypeDefsInPriorityOrder.ToList();
        }

        public override void OTabOnGUI(Rect fillRect)
        {
            Rect position = fillRect.ContractedBy(10f);
            GUI.BeginGroup(position);
            Rect position2 = new Rect(0f, 0f, position.width, 40f);
            GUI.BeginGroup(position2);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect = new Rect(5f, 5f, 140f, 30f);
            Widgets.LabelCheckbox(rect, "ManualPriorities".Translate(), ref Find.Map.playSettings.useWorkPriorities, false);
            float num = position2.width / 3f;
            float num2 = position2.width * 2f / 3f;
            Rect rect2 = new Rect(num - 50f, 5f, 160f, 30f);
            Rect rect3 = new Rect(num2 - 50f, 5f, 160f, 30f);
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect2, "<= " + "HigherPriority".Translate());
            Widgets.Label(rect3, "LowerPriority".Translate() + " =>");
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
            Rect position3 = new Rect(0f, 40f, position.width, position.height - 40f);
            GUI.BeginGroup(position3);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Rect outRect = new Rect(0f, 50f, position3.width, position3.height - 50f);
            this.workColumnSpacing = (position3.width - 16f - 175f) / (float)OTab_Droid_Work.VisibleWorkTypeDefsInPriorityOrder.Count;
            float num3 = 175f;
            int num4 = 0;
            foreach (WorkTypeDef current in OTab_Droid_Work.VisibleWorkTypeDefsInPriorityOrder)
            {
                Vector2 vector = Text.CalcSize(current.labelShort);
                float num5 = num3 + 15f;
                Rect rect4 = new Rect(num5 - vector.x / 2f, 0f, vector.x, vector.y);
                if (num4 % 2 == 1)
                {
                    rect4.y += 20f;
                }
                if (rect4.Contains(Event.current.mousePosition))
                {
                    Widgets.DrawHighlight(rect4);
                }
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect4, current.labelShort);
                WorkTypeDef localDef = current;
                TooltipHandler.TipRegion(rect4, new TipSignal(() => localDef.gerundLabel + "\n\n" + localDef.description, localDef.GetHashCode()));
                GUI.color = new Color(1f, 1f, 1f, 0.3f);
                Widgets.DrawLineVertical(num5, rect4.yMax - 3f, 50f - rect4.yMax + 3f);
                Widgets.DrawLineVertical(num5 + 1f, rect4.yMax - 3f, 50f - rect4.yMax + 3f);
                GUI.color = Color.white;
                num3 += this.workColumnSpacing;
                num4++;
            }
            DrawRows(outRect);
            GUI.EndGroup();
            GUI.EndGroup();

            foreach(Droid droid in ListerDroids.AllDroids)
            {
                if(droid.work.workGiversInOrderEmergencyCache!=null || droid.work.workGiversInOrderCache!=null)
                {
                    droid.work.RefreshWorkGiverCaches();
                }
            }
        }

        protected virtual void DrawPawnRow(Rect r, Droid p)
        {
            float num = 175f;
            Text.Font = GameFont.Medium;
            for (int i = 0; i < OTab_Droid_Work.VisibleWorkTypeDefsInPriorityOrder.Count; i++)
            {
                WorkTypeDef workTypeDef = OTab_Droid_Work.VisibleWorkTypeDefsInPriorityOrder[i];
                Vector2 topLeft = new Vector2(num, r.y + 2.5f);
                if (p.story != null && p.work.Contains(workTypeDef))
                {
                    WidgetsWork.DrawWorkBoxFor(topLeft, p, workTypeDef);
                }
                Rect rect2 = new Rect(topLeft.x, topLeft.y, 25f, 25f);
                TooltipHandler.TipRegion(rect2, WidgetsWork.TipForPawnWorker(p, workTypeDef));
                num += this.workColumnSpacing;
            }
        }

        protected void DrawRows(Rect outRect)
        {
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, (float)ListerDroids.AllDroids.Count * 30f);
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
            float num = 0f;
            foreach (Droid current in ListerDroids.AllDroids)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.2f);
                Widgets.DrawLineHorizontal(0f, num, viewRect.width);
                GUI.color = Color.white;
                Rect rect = new Rect(0f, num, viewRect.width, 30f);
                this.PreDrawPawnRow(rect, current);
                this.DrawPawnRow(rect, current);
                this.PostDrawPawnRow(rect, current);
                num += 30f;
            }
            Widgets.EndScrollView();
        }

        private void PreDrawPawnRow(Rect rect, Pawn p)
        {
            Rect position = new Rect(0f, rect.y, rect.width, 30f);
            if (position.Contains(Event.current.mousePosition))
            {
                GUI.DrawTexture(position, TexUI.HighlightTex);
            }
            Rect rect2 = new Rect(0f, rect.y, 175f, 30f);
            Rect position2 = rect2.ContractedBy(3f);
            if (p.health.summaryHealth.SummaryHealthPercent < 0.99f)
            {
                Rect screenRect = new Rect(rect2);
                screenRect.xMin -= 4f;
                screenRect.yMin += 4f;
                screenRect.yMax -= 6f;
                Widgets.FillableBar(screenRect, p.health.summaryHealth.SummaryHealthPercent, PawnUIOverlay.HealthTex, BaseContent.ClearTex, false);
            }
            if (rect2.Contains(Event.current.mousePosition))
            {
                GUI.DrawTexture(position2, TexUI.HighlightTex);
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect rect3 = new Rect(rect2);
            rect3.xMin += 15f;
            Widgets.Label(rect3, p.LabelCap);
            if (Widgets.InvisibleButton(rect2))
            {
                Find.LayerStack.TopLayerOfType<Dialog_Overview>().Close(true);
                Find.CameraMap.JumpTo(p.Position);
                Find.Selector.ClearSelection();
                Find.Selector.Select(p, true);
                return;
            }
            TipSignal tooltip = p.GetTooltip();
            tooltip.text = "ClickToJumpTo".Translate() + "\n\n" + tooltip.text;
            TooltipHandler.TipRegion(rect2, tooltip);
        }
        private void PostDrawPawnRow(Rect rect, Droid p)
        {
            if (!p.Active)
            {
                GUI.color = new Color(1f, 1f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(rect.x, rect.center.y, rect.width);
                GUI.color = Color.white;
            }
        }

    }
}
