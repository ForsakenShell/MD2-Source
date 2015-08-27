using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Window_LineManagementUI : Window_ManufacturingPlant
    {
        public Window previousPage;
        private List<Order> orders;

        public Window_LineManagementUI(AssemblyLine line, Window previous)
            : base(line, "LineManagementHelp".Translate())
        {
            this.previousPage = previous;
            this.selectedOrder = line.CurrentOrder;
            this.orders = line.OrderStack.All.ToList();
        }

        private Order selectedOrder;

        private Vector2 buttonSize = new Vector2(120f, 40f);
        private float margin = 2f;
        private Vector2 scrollPosition = default(Vector2);
        private float bottomLine;

        private float maxXHorizontalLeft;
        private float minXHorizontalRight;
        private Rect orderStackRect;
        private Vector2 orderEntrySize;

        private float curYLeft = 0f;
        private float curYRight = 0f;

        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);

            bottomLine = CloseButSize.y + margin;
            float m = 15f;
            maxXHorizontalLeft = inRect.width / 2 - m;
            minXHorizontalRight = inRect.width / 2 + m;
            if (Widgets.TextButton(new Rect(0f, inRect.height - buttonSize.y, buttonSize.x, buttonSize.y), "Back"))
            {
                this.Close();
                Find.WindowStack.Add(previousPage);
            }

            Rect labelRect = new Rect(0f, 0f, inRect.width, 30f);
            curYLeft += labelRect.yMax + margin;
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(labelRect, line.label);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.DrawLineHorizontal(0, curYLeft, inRect.width);
            curYLeft += 10f;
            curYRight += curYLeft;

            //Draw the list box of all orders
            float rectheight = inRect.height - curYLeft - bottomLine;
            orderStackRect = new Rect(0f, curYLeft, maxXHorizontalLeft, rectheight);
            if (line.OrderStack.Count <= 0)
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(orderStackRect, "NoOrders".Translate());
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                this.orderEntrySize = new Vector2(orderStackRect.width - 16f, 120f);
                try
                {
                    GUI.BeginGroup(orderStackRect);
                    float entryHeightWithMargin = orderEntrySize.y + 8f;
                    float height = (float)line.OrderStack.Count * entryHeightWithMargin;

                    Rect viewRect = new Rect(0f, 0f, orderEntrySize.x, height);
                    Rect outRect = new Rect(orderStackRect.AtZero());
                    try
                    {
                        Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
                        float currentScrollY = 0f;

                        foreach (var current in this.orders)
                        {
                            DrawOrderEntry(current, currentScrollY);
                            currentScrollY += entryHeightWithMargin;
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

            //Draw the options buttons on the right side
            float paddingBetweenButtons = 12f;
            Vector2 optionsRectSize = new Vector2(inRect.width - minXHorizontalRight, CloseButSize.y * 2 + paddingBetweenButtons + 4f);
            Rect optionsButtonsRect = new Rect(minXHorizontalRight, curYRight, optionsRectSize.x, optionsRectSize.y);
            curYRight += optionsRectSize.y + 6f;
            Widgets.DrawMenuSection(optionsButtonsRect);
            Rect innerOptions = optionsButtonsRect.ContractedBy(2f);
            try
            {
                GUI.BeginGroup(innerOptions);
                float buttonXSize = (innerOptions.width / 2) - paddingBetweenButtons / 2;
                //float butY = innerOptions.height / 2 - (this.CloseButSize.y / 2);
                float butY = CloseButSize.y / 2;
                float butY2 = innerOptions.height - (this.CloseButSize.y + (CloseButSize.y / 2));

                //Add recipe button
                Rect recipeButtonRect = new Rect(0f, innerOptions.height / 2 - CloseButSize.y / 2, buttonXSize, CloseButSize.y);
                if (Widgets.TextButton(recipeButtonRect, "AddOrder".Translate()))
                {
                    Find.WindowStack.Add(new Window_RecipeManagement(line, this));
                }

                //Options button
                //Rect optionsButtonRect = new Rect(innerOptions.width - buttonXSize, 0f, buttonXSize, CloseButSize.y);
                //Widgets.TextButton(optionsButtonRect, "MD2Options".Translate());

                //Upgrades button
                Rect upgradesButtonRect = new Rect(innerOptions.width - buttonXSize, innerOptions.height / 2 - CloseButSize.y / 2, buttonXSize, CloseButSize.y);
                if (Widgets.TextButton(upgradesButtonRect, "Upgrades".Translate()))
                {
                    Find.WindowStack.Add(new Dialog_UpgradeManager(this.line));
                }

            }
            finally
            {
                GUI.EndGroup();
            }
            //Draw the box for the selected def's config
            float configHeight = inRect.height - bottomLine - curYRight;
            Rect configBox = new Rect(minXHorizontalRight, curYRight, inRect.width - minXHorizontalRight, configHeight);
            Widgets.DrawMenuSection(configBox);
            Rect innerConfig = configBox.ContractedBy(6f);
            if (this.selectedOrder != null)
            {
                try
                {
                    GUI.BeginGroup(innerConfig);
                    float labelWidth = 30f;
                    float y = 0f;
                    Rect selectedLabelRect = new Rect(0f, 0f, innerConfig.width, labelWidth);
                    y += labelWidth + margin;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(selectedLabelRect, "Selected".Translate() + ": " + selectedOrder.Recipe.LabelCap);
                    Text.Anchor = TextAnchor.UpperLeft;
                    Rect configRect = new Rect(0f, y, innerConfig.width, innerConfig.height - y);
                    this.selectedOrder.Config.OnGUI(configRect);
                }
                finally
                {
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.EndGroup();
                }
            }
            else
            {
                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(innerConfig, "NoCurrentOrder".Translate());
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            curYLeft = 0f;
            curYRight = 0f;
        }

        private void DrawOrderEntry(Order order, float currentScrollY)
        {
            Rect currentEntryRect = new Rect(0f, currentScrollY, this.orderEntrySize.x, this.orderEntrySize.y);
            Widgets.DrawMenuSection(currentEntryRect);
            Rect texRect = currentEntryRect.ContractedBy(2f);
            Rect innerRect = currentEntryRect.ContractedBy(6f);
            Vector2 butSize = new Vector2(30f, 30f);
            float padding = 2f;

            if ((selectedOrder != null && order == selectedOrder) && (currentEntryRect.Contains(Event.current.mousePosition)))
            {
                if (order.Config.Suspended)
                {
                    GUI.color = Widget.SuspendedItemSelectedHoverColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Widget.SelectedItemHoverColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
            }
            else if (currentEntryRect.Contains(Event.current.mousePosition))
            {
                if (order.Config.Suspended)
                {
                    GUI.color = Widget.SuspendedItemHoverColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Widget.ItemHoverColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
            }
            else if (selectedOrder != null && order == selectedOrder)
            {
                if (order.Config.Suspended)
                {
                    GUI.color = Widget.SuspendedItemSelectedColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Widget.SelectedItemColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
            }
            else
            {
                if (order.Config.Suspended)
                {
                    GUI.color = Widget.SuspendedItemColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Widget.ItemColor;
                    GUI.DrawTexture(texRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                }
            }

            try
            {
                GUI.BeginGroup(innerRect);

                float innerRectCentreY = (innerRect.yMax - innerRect.y) / 2;
                float innerRectCentreX = innerRect.width / 2;

                //Do the label
                Rect labelRect = new Rect(0f, 0f, innerRect.width / 3 - padding, innerRect.height);
                Text.Anchor = TextAnchor.MiddleLeft;
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
                Widgets.Label(labelRect, order.Recipe.LabelCap);

                //Do the time remaining label
                Rect timeRect = new Rect(innerRect.width / 3 + padding, 0f, innerRect.width / 3 - padding * 2, innerRect.height);
                string s;
                if (order.ShoppingList.HasAllMats)
                    s = "TimeRemaining".Translate() + ": " + order.GetTimeRemaining;
                else
                    s = "AwaitingMaterials".Translate();
                Widgets.Label(timeRect, s);

                //Do the count label
                Rect countRect = new Rect((innerRect.width / 3) * 2 + padding, 0f, innerRect.width / 3 - butSize.x - padding * 2, innerRect.height);
                string count = string.Empty;
                if (order.Config.RepeatMode == RimWorld.BillRepeatMode.Forever)
                    count = "MD2Infinite".Translate();
                if (order.Config.RepeatMode == RimWorld.BillRepeatMode.RepeatCount)
                    count = order.Config.Cycles.ToString() + "x";
                if (order.Config.RepeatMode == RimWorld.BillRepeatMode.TargetCount)
                    count = "MD2Until".Translate() + " " + order.Config.TargetCount.ToString();
                Widgets.Label(countRect, count);

                //Do the delete button
                Rect deleteButRect = new Rect(innerRect.width - butSize.x, 0f, butSize.x, butSize.y);
                if (Widgets.ImageButton(deleteButRect, ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true)))
                {
                    order.Delete();
                    this.orders = line.OrderStack.All.ToList();
                    this.selectedOrder = line.CurrentOrder;
                }
                TooltipHandler.TipRegion(deleteButRect, "MD2Delete".Translate());

                //Do the suspend button
                Rect suspendButrect = new Rect(innerRect.width - butSize.x, innerRect.height - butSize.y, butSize.x, butSize.y);
                if (Widgets.ImageButton(suspendButrect, Widget.Suspend))
                {
                    order.Config.Suspended = !order.Config.Suspended;
                }
                TooltipHandler.TipRegion(suspendButrect, "MD2Suspend".Translate());

                //Do the move up button
                Rect upButRect = new Rect(innerRectCentreX - butSize.x, 0f, butSize.x, butSize.y);
                if (order.CanMoveUp)
                {
                    if (Widgets.ImageButton(upButRect, Widget.ReorderUp))
                    {
                        line.OrderStack.Reorder(order, MoveDir.Up);
                        this.orders = line.OrderStack.All.ToList();
                    }
                }
                else
                {
                    GUI.color = Widget.InactiveButtonColor;
                    GUI.DrawTexture(upButRect, Widget.ReorderUp);
                    GUI.color = Color.white;
                }
                TooltipHandler.TipRegion(upButRect, "MoveUp".Translate());


                //Do the move down button
                Rect downButRect = new Rect(innerRectCentreX - butSize.x, innerRect.height - butSize.y, butSize.x, butSize.y);
                if (order.CanMoveDown)
                {
                    if (Widgets.ImageButton(downButRect, Widget.ReorderDown))
                    {
                        line.OrderStack.Reorder(order, MoveDir.Down);
                        this.orders = line.OrderStack.All.ToList();
                    }
                }
                else
                {
                    GUI.color = Widget.InactiveButtonColor;
                    GUI.DrawTexture(downButRect, Widget.ReorderDown);
                    GUI.color = Color.white;
                }
                TooltipHandler.TipRegion(downButRect, "MoveDown".Translate());


                //Do the invisible button which selects the order.This needs to go last
                Rect invisButRect = new Rect(0f, 0f, innerRect.width, innerRect.height);
                if (Widgets.InvisibleButton(invisButRect))
                {
                    this.selectedOrder = order;
                }


            }
            finally
            {
                GUI.EndGroup();
                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;
                GUI.color = Color.white;

            }
        }

        public void Reload()
        {
            var page = new Window_LineManagementUI(this.line, this.previousPage);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            this.Close();
            Find.WindowStack.Add(page);
        }
    }
}
