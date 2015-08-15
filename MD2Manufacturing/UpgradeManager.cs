using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class UpgradeManager : IExposable
    {
        private AssemblyLine parent;
        private List<AssemblyLineUpgradeDef> CompletedUpgrades = new List<AssemblyLineUpgradeDef>();
        private List<AssemblyLineUpgradeDef> AvailableUpgrades = DefDatabase<AssemblyLineUpgradeDef>.AllDefsListForReading;
        private List<AssemblyLineUpgrade> UpgradesInProgress = new List<AssemblyLineUpgrade>();
        private Vector2 scrollPosition = default(Vector2);
        private Texture2D FillableBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));

        private Vector2 upgradeEntrySize;
        private readonly float margin = 4f;


        public UpgradeManager(AssemblyLine parent)
        {
            this.parent = parent;
        }

        public void SpawnSetup()
        {
        }

        public void Tick()
        {
            if (UpgradesInProgress.Count > 0)
            {
                List<AssemblyLineUpgrade> list = new List<AssemblyLineUpgrade>();
                foreach (var upgrade in UpgradesInProgress)
                {
                    if (upgrade.Tick())
                    {
                        list.Add(upgrade);
                    }
                }
                if (list.Count > 0)
                {
                    foreach (var u in list)
                    {
                        FinishUpgrade(u);
                    }
                }
            }
        }

        private void FinishUpgrade(AssemblyLineUpgrade upgrade)
        {
            Messages.Message("UpgradeCompleted".Translate(parent.label, upgrade.Def.label), MessageSound.Benefit);
            parent.AddUpgrade(upgrade);
            UpgradesInProgress.Remove(upgrade);
            CompletedUpgrades.Add(upgrade.Def);
        }


        public AssemblyLine Parent
        {
            get
            {
                return this.parent;
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.LookList(ref this.CompletedUpgrades, "completedUpgrades", LookMode.DefReference);
            Scribe_Collections.LookList(ref this.UpgradesInProgress, "UpgradesInProgress", LookMode.Deep);
        }

        public void OnGUI(Rect inRect)
        {
            upgradeEntrySize = new Vector2(inRect.width - (margin * 6), 70f);

            //List of available upgrades
            List<AssemblyLineUpgradeDef> availableList = (
                from t in AvailableUpgrades
                where !CompletedUpgrades.Contains(t) && !UpgradesInProgress.DefIsBeingUsed(t) && (t.alwaysDisplay || (t.Prerequisites == null || CompletedUpgrades.ContainsAll(t.Prerequisites)))
                orderby t.label
                select t).ToList();

            if (availableList.Count <= 0 && UpgradesInProgress.Count <= 0)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(inRect, "NoAvailableUpgrades".Translate());
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                try
                {
                    GUI.BeginGroup(inRect);
                    float entryHeightWithMargin = upgradeEntrySize.y + 8f;
                    float currentScrollY = 3f;
                    float height;
                    if (UpgradesInProgress.Count > 0)
                    {
                        height = currentScrollY + (float)availableList.Count * entryHeightWithMargin + (float)UpgradesInProgress.Count * entryHeightWithMargin;
                    }
                    else
                    {
                        height = currentScrollY + (float)availableList.Count * entryHeightWithMargin;
                    }

                    Rect viewRect = new Rect(0, 0, upgradeEntrySize.x, height);
                    Rect outRect = new Rect(inRect.AtZero());
                    try
                    {
                        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);

                        if (UpgradesInProgress.Count > 0)
                        {
                            foreach (var current in UpgradesInProgress)
                            {
                                DrawUpgradeInProgressEntry(current, currentScrollY);
                                currentScrollY += entryHeightWithMargin;
                            }
                        }
                        if (availableList.Count > 0)
                        {
                            foreach (var current in availableList)
                            {
                                DrawAvailableUpgradeEntry(current, currentScrollY);
                                currentScrollY += entryHeightWithMargin;
                            }
                        }
                    }
                    finally
                    {
                        Widgets.EndScrollView();
                    }
                }
                finally
                {
                    GUI.EndGroup();
                }
            }
        }

        private void DrawUpgradeInProgressEntry(AssemblyLineUpgrade upgrade, float currentScrollY)
        {
            Rect currentEntryRect = new Rect(margin, currentScrollY, upgradeEntrySize.x, upgradeEntrySize.y);
            Widgets.DrawMenuSection(currentEntryRect);
            Rect innerRect = currentEntryRect.ContractedBy(4f);
            Vector2 butSize = new Vector2(60f, innerRect.height / 2);
            Widgets.FillableBar(currentEntryRect, upgrade.PercentageComplete, FillableBarTex, null, false);

            try
            {
                GUI.BeginGroup(innerRect);

                Text.Anchor = TextAnchor.MiddleLeft;
                float width = Text.CalcSize(upgrade.Def.LabelCap).x;
                Rect labelRect = new Rect(0f, 0f, width, innerRect.height);
                Widgets.Label(labelRect, upgrade.Def.LabelCap);

                string s;
                if (upgrade.BillOfMaterials.HasMats)
                {
                    s = "MD2Progress".Translate((upgrade.PercentageComplete * 100).ToString("0.0\\%"));
                    TooltipHandler.TipRegion(innerRect, "UpgradeInstallTimeRemaining".Translate(TicksToTime.GetTime(upgrade.TicksRemaining)));
                }
                else
                {
                    s = upgrade.BillOfMaterials.ReportString;
                }
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect progressRect = new Rect(innerRect.width / 2 - (Text.CalcSize(s).x), innerRect.yMin, innerRect.width, innerRect.height);
                Widgets.Label(progressRect, s);
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.EndGroup();
            }

        }

        private void DrawAvailableUpgradeEntry(AssemblyLineUpgradeDef def, float currentScrollY)
        {
            Rect currentEntryRect = new Rect(margin, currentScrollY, upgradeEntrySize.x, upgradeEntrySize.y);
            Widgets.DrawMenuSection(currentEntryRect);
            Rect innerRect = currentEntryRect.ContractedBy(4f);
            string s = "MD2BuildingMaterials".Translate() + "\n";
            foreach (var item in def.RequiredMaterials)
            {
                s += item.amount.ToString() + " " + item.thing.LabelCap + "\n";
            }
            s += "InstallUpgradeTimeToComplete".Translate(TicksToTime.GetTime(def.workTicksAmount));
            TooltipHandler.TipRegion(innerRect, s);
            Vector2 butSize = new Vector2(60f, innerRect.height / 2);
            float padding = 2f;

            try
            {
                GUI.BeginGroup(innerRect);

                Rect textBox = new Rect(0, 0, innerRect.width / 2, innerRect.height);
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(textBox, def.label);
                //Text.Anchor = TextAnchor.UpperLeft;

                Rect butRect = new Rect(innerRect.width - (butSize.x + padding), innerRect.height / 2 - (butSize.y / 2), butSize.x, butSize.y);
                if (Widgets.TextButton(butRect, "UpgradeInstall".Translate()))
                {
                    UpgradesInProgress.Add(new AssemblyLineUpgrade(def));
                }
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.EndGroup();
            }
        }

    }
}
