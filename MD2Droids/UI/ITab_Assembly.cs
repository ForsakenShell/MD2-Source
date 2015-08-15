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
    public class ITab_Assembly : ITab
    {
        protected readonly Vector2 WinSize = new Vector2(370f, 480f);
        public static readonly Vector2 ButtonSize = new Vector2(120f, 30f);
        private const float Margin = 18f;
        private const float EntrySpacing = 8f;
        private const float EntryHeight = 60f;
        private Vector2 scrollPos = default(Vector2);

        public ITab_Assembly()
        {
            this.size = WinSize;
            this.labelKey = "ITab_Assembly";
        }

        protected override void FillTab()
        {
            Rect mainRect = new Rect(0f, 0f, WinSize.x, WinSize.y).ContractedBy(10f);
            try
            {
                GUI.BeginGroup(mainRect);

                #region Button
                Rect buttonRect = new Rect(0f, 0f, 150f, 29f);
                if (Widgets.TextButton(buttonRect, "AddBill".Translate()))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    foreach (var f in BlueprintFiles.AllFiles)
                    {
                        string s = Path.GetFileNameWithoutExtension(f.Name);
                        FloatMenuOption o = new FloatMenuOption(s, delegate
                        {
                            Find.LayerStack.Add(new Dialog_AddBill(Assembly, s));
                        });
                        options.Add(o);
                    }
                    Find.LayerStack.Add(new Layer_FloatMenu(options));
                }
                #endregion

                Rect scrollRect = new Rect(0f, buttonRect.yMax + Margin, mainRect.width,
                    mainRect.height - buttonRect.yMax - Margin);
                #region Scroll View

                try
                {
                    GUI.BeginGroup(scrollRect);

                    Rect outRect = new Rect(0f, 0f, scrollRect.width, scrollRect.height);
                    if (Assembly.AssemblyBillStack.Count == 0)
                    {
                        Rect labelRect = new Rect(0f, 0f, outRect.width, outRect.height);
                        Text.Font = GameFont.Medium;
                        Text.Anchor = TextAnchor.MiddleCenter;
                        Widgets.Label(labelRect, "NoBlueprintBills".Translate());
                        Text.Font = GameFont.Small;
                        Text.Anchor = TextAnchor.UpperLeft;
                    }
                    else
                    {
                        float EntryHeightWithMargin = EntryHeight + EntrySpacing;
                        float height = Assembly.AssemblyBillStack.Count * EntryHeightWithMargin;
                        Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, height);
                        float curY = 0f;
                        Widgets.BeginScrollView(outRect, ref scrollPos, viewRect);

                        List<AssemblyBill> bills = Assembly.AssemblyBillStack.Bills;
                        bool alternate = true;
                        foreach (var bill in bills)
                        {
                            Rect entryRect = new Rect(0f, curY, viewRect.width, EntryHeight);
                            DrawEntry(entryRect, bill, alternate);
                            curY += EntryHeightWithMargin;
                            alternate = !alternate;
                        }
                        Widgets.EndScrollView();
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception drawing the scroll box: \n" + e.ToString() + "\n" + e.StackTrace);
                }
                finally
                {
                    GUI.EndGroup();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.Error("Error drawing assembly ITab: \n" + ex.ToString());
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        private void DrawEntry(Rect entryRect, AssemblyBill bill, bool alternate)
        {
            try
            {
                Rect inRect = entryRect.ContractedBy(5f);
                if (alternate) Widgets.DrawAltRect(entryRect);
                GUI.BeginGroup(inRect);

                #region Label
                Rect labelRect = new Rect(0f, 0f, inRect.width / 2f, inRect.height);
                string label = "Blueprint".Translate() + ": " + bill.Blueprint.BpName + "\n" + "Name".Translate() + ": " +
                               bill.Blueprint.Name;
                Text.Anchor=TextAnchor.MiddleCenter;
                Widgets.Label(labelRect, label);
                Text.Anchor=TextAnchor.UpperLeft;
                #endregion
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        public AssemblyStation Assembly
        {
            get { return SelThing as AssemblyStation; }
        }
    }
}
