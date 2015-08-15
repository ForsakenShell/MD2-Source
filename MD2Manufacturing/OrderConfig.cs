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
    public class OrderConfig : IExposable
    {
        private BillRepeatMode repeatMode = BillRepeatMode.RepeatCount;
        private Order order;
        private RecipeDef recipe;
        private ThingFilter ingredientsFilter;
        private int cycles = 1;
        private int targetCount = 1;
        private Vector2 scrollPosition;
        private bool completeAll = false;
        private bool suspended = false;
        private bool setupStage = true;

        public static int repeatCountMaxCycles = 15;
        public static int targetCountMaxCycles = 5;

        public OrderConfig(RecipeDef def)
        {
            this.order = null;
            this.recipe = def;
            this.ingredientsFilter = new ThingFilter();
            this.ingredientsFilter.CopyFrom(def.fixedIngredientFilter);
            this.scrollPosition = default(Vector2);
        }

        public OrderConfig(Order order)
        {
            this.order = order;
        }

        Vector2 buttonSize = new Vector2(100f, 30f);
        float margin = 3f;

        public void OnGUI(Rect rect)
        {
            float curY = 0f;
            float repeatModeButX = 0f;
            float adjustButHeight = 30f;
            try
            {
                GUI.BeginGroup(rect);
                //Do the top buttons. If the parent is not null, then do the suspend button.
                if (!setupStage)
                {
                    float suspendButRightEdge = rect.width / 2f - 5f;
                    Rect suspendButRect = new Rect(0f, 0f, suspendButRightEdge, buttonSize.y);
                    if (Suspended)
                    {
                        if (Widgets.TextButton(suspendButRect, "Suspended".Translate()))
                        {
                            Suspended = false;
                        }
                    }
                    else
                    {
                        if (Widgets.TextButton(suspendButRect, "NotSuspended".Translate()))
                        {
                            Suspended = true;
                        }
                    }
                    repeatModeButX = rect.width / 2f + 5f;
                }
                string label = "But";
                if (repeatMode == BillRepeatMode.RepeatCount)
                {
                    label = "DoXTimes".Translate();
                }
                if (repeatMode == BillRepeatMode.TargetCount)
                {
                    label = "DoUntilYouHaveX".Translate();
                }
                if (repeatMode == BillRepeatMode.Forever)
                {
                    label = "DoForever".Translate();
                }
                Vector2 repeatButSize = new Vector2(rect.width - repeatModeButX, buttonSize.y);
                Rect repeatButRect = new Rect(repeatModeButX, curY, repeatButSize.x, repeatButSize.y);
                if (Widgets.TextButton(repeatButRect, label))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    list.Add(new FloatMenuOption("DoXTimes".Translate(), delegate
                        {
                            this.repeatMode = BillRepeatMode.RepeatCount;
                        }));
                    list.Add(new FloatMenuOption("DoUntilYouHaveX".Translate(), delegate
                        {
                            this.repeatMode = BillRepeatMode.TargetCount;
                        }));
                    list.Add(new FloatMenuOption("DoForever".Translate(), delegate
                        {
                            this.repeatMode = BillRepeatMode.Forever;
                        }));
                    Find.LayerStack.Add(new Layer_FloatMenu(list));
                }
                curY += repeatButSize.y + margin;

                //Do the target count label
                Rect countRect = new Rect(0, curY, rect.width, 30f);
                curY += countRect.height + margin;
                string text = string.Empty;
                if (this.repeatMode == BillRepeatMode.RepeatCount)
                {
                    text = "RepeatCount".Translate(new object[]{
                        Cycles.ToString()
                    });
                    float butPadding = 2f;
                    float butWidth = (rect.width - 8 * butPadding) / 5f;
                    float xPos = 0f;
                    Rect but1Rect = new Rect(xPos, curY, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but1Rect, "-25"))
                    {
                        if (Cycles == 0)
                            SoundStarter.PlayOneShot(SoundDefOf.ClickReject, SoundInfo.OnCamera());
                        else
                        {
                            Cycles -= 25;
                        }
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but2Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but2Rect, "-1"))
                    {
                        if (Cycles == 0)
                            SoundStarter.PlayOneShot(SoundDefOf.ClickReject, SoundInfo.OnCamera());
                        else
                        {
                            Cycles -= 1;
                        }
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but3Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but3Rect, "1"))
                    {
                        Cycles = 1;
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but4Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but4Rect, "+1"))
                    {
                        Cycles += 1;
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but5Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but5Rect, "+25"))
                    {
                        Cycles += 25;
                    }
                }
                else if (repeatMode == BillRepeatMode.TargetCount)
                {
                    text = "TargetCount".Translate(new object[]{
                        (TargetCount>=999999)?"Infinite".Translate() : TargetCount.ToString()
                    });
                    float butPadding = 2f;
                    float butWidth = (rect.width - 12 * butPadding) / 7f;
                    float xPos = 0f;
                    Rect but1Rect = new Rect(xPos, curY, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but1Rect, "-250"))
                    {
                        if (TargetCount == 0)
                            SoundStarter.PlayOneShot(SoundDefOf.ClickReject, SoundInfo.OnCamera());
                        else
                        {
                            TargetCount -= 250;
                        }
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but2Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but2Rect, "-25"))
                    {
                        if (TargetCount == 0)
                            SoundStarter.PlayOneShot(SoundDefOf.ClickReject, SoundInfo.OnCamera());
                        else
                        {
                            TargetCount -= 25;
                        }
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but3Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but3Rect, "-1"))
                    {
                        if (TargetCount == 0)
                            SoundStarter.PlayOneShot(SoundDefOf.ClickReject, SoundInfo.OnCamera());
                        else
                        {
                            TargetCount -= 1;
                        }
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but4Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but4Rect, "1"))
                    {
                        TargetCount = 1;
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but5Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but5Rect, "+1"))
                    {
                        TargetCount += 1;
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but6Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but6Rect, "+25"))
                    {
                        TargetCount += 25;
                    }
                    xPos += butWidth + butPadding * 2;
                    Rect but7Rect = new Rect(xPos, but1Rect.y, butWidth, adjustButHeight);
                    if (Widgets.TextButton(but7Rect, "+250"))
                    {
                        TargetCount += 250;
                    }
                }
                else if (repeatMode == BillRepeatMode.Forever)
                {
                    text = "Do Forever";
                }
                else
                {
                    throw new InvalidOperationException();
                }
                curY += adjustButHeight + margin + 6f;

                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(countRect, text);
                //Now for the ThingFilter box (shiver)
                Rect thingFilterLabelRect = new Rect(0f, curY, rect.width, 20f);
                curY += thingFilterLabelRect.height + margin;
                Widgets.Label(thingFilterLabelRect, "PermittedIngredients".Translate());
                Text.Anchor = TextAnchor.UpperLeft;

                float tfheight = rect.height - curY - adjustButHeight - margin;
                Rect thingFilterRect = new Rect(0f, curY, rect.width, tfheight);
                curY += tfheight + margin;
                ThingFilterUI.DoThingFilterConfigWindow(thingFilterRect, ref this.scrollPosition, this.ingredientsFilter, this.recipe.fixedIngredientFilter);
                //Well that wasn't so hard after all
                float checkWidth = rect.width / 2 - 30f;
                float checkXPos = rect.width / 2 - (checkWidth / 2);
                Rect checkRect = new Rect(checkXPos, curY, checkWidth, adjustButHeight);
                Widgets.LabelCheckbox(checkRect, "Complete all", ref this.completeAll);
                string s = "Activating this will set the Assembly line to send down the completed products once all cycles of the order have been completed, or when it has completed 15 cycles. This helps reduce the number of incoming drops when the completion time of the recipe is very quick.";
                if (RepeatMode == BillRepeatMode.TargetCount)
                {
                    s = "Activating this will set the Assembly line to send down the completed products once all cycles of the order have been completed, or when it has completed 5 cycles. This helps reduce the number of incoming drops when the completion time of the recipe is very quick.";
                }
                TipSignal tip = new TipSignal(s);
                TooltipHandler.TipRegion(checkRect, tip);
            }
            finally
            {
                GUI.EndGroup();
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
            }
        }


        public int Cycles
        {
            get
            {
                return this.cycles;
            }
            set
            {
                this.cycles = value;
                if (cycles < 0)
                {
                    cycles = 0;
                }
            }
        }

        public int WorkAmount
        {
            get
            {
                if (Recipe.workAmount >= 0f)
                {
                    return (int)(this.Recipe.workAmount * order.Line.Speed);
                }
                return (int)(Recipe.products[0].thingDef.GetStatValueAbstract(StatDefOf.WorkToMake) * order.Line.Speed);
            }
        }

        public bool SetupStage
        {
            get
            {
                return this.setupStage;
            }
        }

        public void CompleteSetup()
        {
            this.setupStage = false;
        }

        public ThingFilter IngredientsFilter
        {
            get
            {
                return this.ingredientsFilter;
            }
        }

        public int TargetCount
        {
            get
            {
                return targetCount;
            }
            set
            {
                this.targetCount = value;
                if (targetCount < 0)
                {
                    targetCount = 0;
                }
            }
        }

        public bool CompleteAll
        {
            get
            {
                return completeAll;
            }
            set
            {
                this.completeAll = value;
            }
        }

        public bool Suspended
        {
            get
            {
                return suspended;
            }
            set
            {
                this.suspended = value;
            }
        }

        public BillRepeatMode RepeatMode
        {
            get
            {
                return this.repeatMode;
            }
        }

        public string GetTimeForSingleCycle
        {
            get
            {
                if(recipe.workAmount>=0f)
                {
                        return TicksToTime.GetTime(recipe.workAmount);
                }
                else
                {
                    return TicksToTime.GetTime(recipe.products[0].thingDef.GetStatValueAbstract(StatDefOf.WorkToMake));
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.repeatMode, "repeatMode");
            Scribe_Defs.LookDef(ref this.recipe, "recipe");
            Scribe_Deep.LookDeep(ref this.ingredientsFilter, "fixedIngredientFilter");
            Scribe_Values.LookValue(ref this.cycles, "cycles");
            Scribe_Values.LookValue(ref this.targetCount, "targetCount");
            Scribe_Values.LookValue(ref this.completeAll, "completeAll");
            Scribe_Values.LookValue(ref this.suspended, "suspended");
            Scribe_Values.LookValue<bool>(ref this.setupStage, "setupStage");
        }

        public void SetParentDirect(Order order)
        {
            this.order = order;
        }

        public Order Order
        {
            get
            {
                return order;
            }
        }

        public RecipeDef Recipe
        {
            get
            {
                return this.recipe;
            }
        }
    }
}
