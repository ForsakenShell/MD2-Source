using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MD2
{
    public abstract class Dialog_Blueprint : Layer_Window
    {
        protected const float BoxMargin = 20f;
        protected const float MapEntrySpacing = 3f;
        protected const float MapEntryMargin = 1f;
        protected const float MapNameExtraLeftMargin = 15f;
        protected const float MapInfoExtraLeftMargin = 270f;
        protected const float DeleteButtonSpace = 5f;
        protected const float MapEntryHeight = 36f;
        protected string interactButLabel = "Error";
        protected float bottomAreaHeight;
        private Vector2 scrollPosition = Vector2.zero;
        private static readonly Color ManualSaveTextColor = new Color(1f, 1f, 0.6f);
        public Dialog_Blueprint()
        {
            base.SetCentered(600f, 700f);
            this.drawPriority = 2000;
            this.closeOnEscapeKey = true;
            this.doCloseButton = true;
            this.doCloseX = true;
            this.absorbAllInput = true;
            this.clearNonEditWindows = false;
            this.forcePause = true;
        }

        protected override void FillWindow(Rect inRect)
        {
            Vector2 vector = new Vector2(inRect.width - 16f, 36f);
            Vector2 vector2 = new Vector2(100f, vector.y - 2f);
            inRect.height -= 45f;
            float num = vector.y + 3f;
            List<FileInfo> list = BlueprintFiles.AllFiles.ToList();
            float height = (float)list.Count * num;
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, height);
            Rect outRect = new Rect(inRect.AtZero());
            outRect.height -= this.bottomAreaHeight;
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
            float num2 = 0f;
            int num3 = 0;
            foreach (FileInfo current in list)
            {
                Rect rect = new Rect(0f, num2, vector.x, vector.y);
                if (num3 % 2 == 0)
                {
                    Widgets.DrawAltRect(rect);
                }
                Rect position = rect.ContractedBy(1f);
                GUI.BeginGroup(position);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(current.Name);
                GUI.color = Dialog_Blueprint.ManualSaveTextColor;
                Rect rect2 = new Rect(15f, 0f, position.width, position.height);
                Text.Anchor = TextAnchor.MiddleLeft;
                Text.Font = GameFont.Small;
                Widgets.Label(rect2, fileNameWithoutExtension);
                GUI.color = Color.white;
                Rect rect3 = new Rect(270f, 0f, 200f, position.height);
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
                float num4 = vector.x - 2f - vector2.x - vector2.y;
                Rect rect4 = new Rect(num4, 0f, vector2.x, vector2.y);
                if (Widgets.TextButton(rect4, this.interactButLabel, true, false))
                {
                    this.DoMapEntryInteraction(Path.GetFileNameWithoutExtension(current.Name));
                }
                Rect rect5 = new Rect(num4 + vector2.x + 5f, 0f, vector2.y, vector2.y);
                if (Widgets.ImageButton(rect5, TexButton.DeleteX))
                {
                    FileInfo localFile = current;
                    Find.LayerStack.Add(new Dialog_Confirm("ConfirmDelete".Translate(new object[]
					{
						fileNameWithoutExtension
					}), delegate
                    {
                        localFile.Delete();
                    }, true));
                }
                TooltipHandler.TipRegion(rect5, "DeleteThisSavegame".Translate());
                GUI.EndGroup();
                num2 += vector.y + 3f;
                num3++;
            }
            Widgets.EndScrollView();
            this.DoSpecialSaveLoadGUI(inRect.AtZero());
        }
        public override void PostRemove()
        {
        }
        protected virtual void DoSpecialSaveLoadGUI(Rect inRect)
        {
        }
        protected abstract void DoMapEntryInteraction(string mapName);

    }
}
