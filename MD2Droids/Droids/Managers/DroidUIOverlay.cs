using UnityEngine;
using Verse;

namespace MD2
{
    public class DroidUIOverlay
    {
        private const float PawnLabelOffsetY = -0.6f;
        private const int PawnStatBarWidth = 32;
        private const float ActivityIconSize = 13f;
        private const float ActivityIconOffsetY = 12f;
        private const float NameUnderlineDist = 11f;
        private const float MinNameWidth = 20f;
        private Droid droid;
        public static readonly Texture2D HealthTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0f, 0f, 0.25f));

        public DroidUIOverlay(Droid droid)
        {
            this.droid = droid;
        }

        public void DrawGUIOverlay()
        {
            if (!this.droid.SpawnedInWorld || Find.FogGrid.IsFogged(this.droid.Position))
            {
                return;
            }

            Vector3 vector = GenWorldUI.LabelDrawPosFor(this.droid, -0.6f);
            float num = vector.y;
            if (DroidUIOverlay.ShouldDrawOverlayOnMap(this.droid))
            {
                Text.Font = GameFont.Tiny;
                float num2 = Text.CalcSize(this.droid.Nickname).x;
                if (num2 < 20f)
                {
                    num2 = 20f;
                }
                Rect rect = new Rect(vector.x - num2 / 2f - 4f, vector.y, num2 + 8f, 12f);
                GUI.DrawTexture(rect, TexUI.GrayTextBG);
                if (this.droid.health.summaryHealth.SummaryHealthPercent < 0.999f)
                {
                    Rect screenRect = rect.ContractedBy(1f);
                    Widgets.FillableBar(screenRect, this.droid.health.summaryHealth.SummaryHealthPercent, PawnUIOverlay.HealthTex, BaseContent.ClearTex, false);
                }
                GUI.color = PawnNameColorUtility.PawnNameColorOf(this.droid);
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(new Rect(vector.x - num2 / 2f, vector.y - 2f, num2, 999f), this.droid.Nickname);
                if (this.droid.playerController != null && this.droid.playerController.Drafted)
                {
                    Widgets.DrawLineHorizontal(vector.x - num2 / 2f, vector.y + 11f, num2);
                }
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
                num += 12f;
            }
        }

        private static bool ShouldDrawOverlayOnMap(Droid droid)
        {
            return true;
        }
    }
}
