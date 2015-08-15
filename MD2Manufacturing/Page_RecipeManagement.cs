using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Page_RecipeManagement : Page_ManufacturingPlant
    {
        private Layer previousPage;
        private List<RecipeDef> recipesList = RecipeListGenerator.AllRecipes().ToList();
        private OrderConfig config;

        public Page_RecipeManagement(AssemblyLine line, Page_LineManagementUI previousPage)
            : base(line, "RecipeManagementHelp".Translate())
        {
            this.previousPage = previousPage;
            if (recipesList != null)
            {
                recipesList.Sort((o, y) => o.label.CompareTo(y.label));
                selectedDef = recipesList.First();
                config = new OrderConfig(selectedDef);
            }
        }

        private float currentYMaxLeft = 0f;
        private float currentYMaxRight = 0f;

        private RecipeDef selectedDef;
        private List<RecipeDef> list;

        private float topPaddingHeight = 50f;
        private float bottomPaddingHeight = 100f;
        private float centrePadding = 15f;
        private float horizontalMargin = 8f;
        private float rightSidePosition;
        private float bottom = 0f;

        private string enteredText = "";

        private Vector2 availableRecipeMainRectSize;
        private Vector2 recipeEntrySize;
        private Vector2 recipeAddButtonSize;
        private Vector2 searchBoxSize;
        private Vector2 infoBoxSize;
        private Vector2 billConfigBoxSize;
        private Vector2 buttonSize = new Vector2(120f, 40f);
        private Vector2 scrollPosition = Vector2.zero;
        private static readonly Texture2D AltTexture = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.05f));

        protected static Color InactiveButtonColor = new Color(1, 1, 1, 0.5f);
        protected static Color MouseHoverColor = new Color(0.2588f, 0.2588f, 0.2588f);
        protected override void FillWindow(Rect inRect)
        {
            base.FillWindow(inRect);
            ////
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperCenter;
            float num = inRect.width / 2 - 100f;
            Widgets.Label(new Rect(num, 0f, 200f, 200f), "AddNewOrder".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (Widgets.TextButton(new Rect(0f, inRect.height - buttonSize.y, buttonSize.x, buttonSize.y), "Back"))
            {
                this.GoBack();
            }
            if (this.selectedDef == null)
            {
                GUI.color = InactiveButtonColor;
                Widgets.TextButton(new Rect(inRect.width - buttonSize.x, inRect.height - buttonSize.y, buttonSize.x, buttonSize.y), "AddOrder".Translate());
                GUI.color = Color.white;
            }
            else
            {
                if (Widgets.TextButton(new Rect(inRect.width - buttonSize.x, inRect.height - buttonSize.y, buttonSize.x, buttonSize.y), "AddOrder".Translate()))
                {
                    config.CompleteSetup();
                    this.line.OrderStack.AddNewOrder(new Order(line, config));
                    this.GoBack(true);
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
            this.availableRecipeMainRectSize = new Vector2(inRect.width / 2 - centrePadding, inRect.height - bottomPaddingHeight - topPaddingHeight);
            rightSidePosition = inRect.width / 2 + centrePadding;

            currentYMaxLeft += topPaddingHeight;
            currentYMaxRight = currentYMaxLeft;

            Widgets.DrawLineHorizontal(0f, currentYMaxLeft - 15f, WinSize.x);

            DrawSearchBox(inRect);

            DrawRecipeList(inRect);

            DrawInfoBox(inRect);

            DrawBillConfigBox(inRect);


            currentYMaxLeft = 0f;
            currentYMaxRight = 0f;

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        private void DrawBillConfigBox(Rect inRect)
        {
            this.billConfigBoxSize = new Vector2(inRect.width - rightSidePosition, inRect.height - topPaddingHeight - currentYMaxRight);
            Rect billConfigBoxRect = new Rect(rightSidePosition, currentYMaxRight, billConfigBoxSize.x, bottom - currentYMaxRight);
            Widgets.DrawMenuSection(billConfigBoxRect);
            if (config != null)
            {
                Rect rect = billConfigBoxRect.ContractedBy(8f);
                config.OnGUI(rect);
            }
        }

        private void DrawInfoBox(Rect inRect)
        {

            this.infoBoxSize = new Vector2(inRect.width - rightSidePosition, 150f);
            Rect infoBoxRect = new Rect(rightSidePosition, currentYMaxRight, infoBoxSize.x, infoBoxSize.y);
            currentYMaxRight += horizontalMargin + infoBoxSize.y;
            Widgets.DrawMenuSection(infoBoxRect);
            Rect infoInnerRect = infoBoxRect.ContractedBy(8f);
            try
            {
                GUI.BeginGroup(infoInnerRect);
                if (selectedDef != null)
                {
                    float curY = 0f;
                    float margin = 2f;
                    Vector2 labelSize = new Vector2(infoInnerRect.width, 30f);
                    Vector2 workAmountSize = new Vector2(infoInnerRect.width, 30f);
                    Vector2 descriptionSize = new Vector2(infoInnerRect.width, infoInnerRect.height - 64f);
                    Text.Font = GameFont.Medium;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    GUI.color = Color.white;
                    Rect labelRect = new Rect(0, curY, labelSize.x, labelSize.y);
                    curY += labelSize.y + margin;
                    //Widgets.Label(labelRect, selectedDef.defName);
                    Widgets.Label(labelRect, selectedDef.LabelCap);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.UpperLeft;
                    Rect descriptionRect = new Rect(margin, curY, descriptionSize.x, descriptionSize.y);
                    curY += descriptionSize.y + margin;
                    string description = selectedDef.description;
                    if (description == null)
                    {
                        StringBuilder str = new StringBuilder();
                        str.Append("Makes".Translate());
                        if (selectedDef.products.First().count > 1)
                            str.Append(selectedDef.products.First().count.ToString());
                        else
                            str.Append("a".Translate());
                        str.Append(selectedDef.products.First().thingDef.label + ".");
                        description = str.ToString();
                    }
                    Widgets.Label(descriptionRect, description);
                    Rect workAmountRect = new Rect(margin, descriptionRect.yMax + margin, workAmountSize.x, workAmountSize.y);
                    string text = "CycleCompleteTime".Translate() + config.GetTimeForSingleCycle;
                    Widgets.Label(workAmountRect, text);
                }
                else
                {
                    Text.Font = GameFont.Medium;
                    GUI.color = Color.white;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(new Rect(0f, 0f, infoInnerRect.width, infoInnerRect.height), "NoRecipeSelected".Translate());
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                }
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        private void DrawSearchBox(Rect inRect)
        {
            this.searchBoxSize = new Vector2(this.availableRecipeMainRectSize.x, 36f);
            Rect searchBoxRect = new Rect(0f, currentYMaxLeft, searchBoxSize.x, searchBoxSize.y);

            currentYMaxLeft += this.searchBoxSize.y + horizontalMargin;

            enteredText = Widgets.TextField(searchBoxRect, enteredText);
        }

        private void DrawRecipeList(Rect inRect)
        {
            bool isTopLayer = Find.LayerStack.TopLayer == this;

            Rect availableRecipeMainRect = new Rect(0f, currentYMaxLeft, availableRecipeMainRectSize.x, availableRecipeMainRectSize.y);
            this.bottom = availableRecipeMainRect.yMax;
            this.recipeEntrySize = new Vector2(availableRecipeMainRect.width - 16f, 48f);
            this.recipeAddButtonSize = new Vector2(120f, this.recipeEntrySize.y - 12f);

            Widgets.DrawMenuSection(availableRecipeMainRect);

            GUI.BeginGroup(availableRecipeMainRect);

            if (enteredText != "")
            {
                list = (
                    from t in this.recipesList
                    where t.label.ToLower().Contains(enteredText.ToLower())
                    select t).ToList();
            }
            else
            {
                list = this.recipesList;
            }
            float entryHeight = this.recipeEntrySize.y;

            float height = (float)list.Count * entryHeight;

            Rect viewRect = new Rect(0f, 0f, availableRecipeMainRect.width - 16f, height);
            Rect outRect = new Rect(availableRecipeMainRect.AtZero());

            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);

            float currentY = 0f;
            float num3 = 0f;
            if (recipesList.Count == 0)
            {
                Rect rect = new Rect(0f, currentY, this.recipeEntrySize.x, this.recipeEntrySize.y);
                Text.Font = GameFont.Small;
                Widgets.Label(rect, "NoRecipesFound".Translate());
            }
            else
            {
                foreach (RecipeDef def in list)
                {
                    Rect rect2 = new Rect(0f, currentY, this.recipeEntrySize.x, this.recipeEntrySize.y);
                    if ((rect2.Contains(Event.current.mousePosition) && isTopLayer) || (selectedDef != null && def == selectedDef))
                    {
                        GUI.color = MouseHoverColor;
                        GUI.DrawTexture(rect2, BaseContent.WhiteTex);
                    }
                    else if (num3 % 2 == 0)
                    {
                        GUI.DrawTexture(rect2, AltTexture);
                    }
                    Rect innerRect = rect2.ContractedBy(3f);
                    GUI.BeginGroup(innerRect);
                    string recipeName = def.label.CapitalizeFirst();
                    GUI.color = Color.white;
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect rect = new Rect(15f, 0f, innerRect.width, innerRect.height);
                    Widgets.Label(rect, recipeName);
                    float buttonX = recipeEntrySize.x - 6f - recipeAddButtonSize.x;
                    Rect butRect = new Rect(buttonX, 0f, recipeAddButtonSize.x, recipeAddButtonSize.y);
                    if (Widgets.InvisibleButton(rect))
                    {
                        this.selectedDef = def;
                        this.config = new OrderConfig(def);
                        //Log.Message("Clicked " + def.defName);
                    }
                    GUI.EndGroup();
                    currentY += entryHeight;
                    num3++;
                }
            }
            GUI.EndGroup();
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.EndScrollView();
        }

        public void GoBack(bool reload = false)
        {
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            this.Close();
            Find.LayerStack.Add(previousPage);
            if (reload)
            {
                Page_LineManagementUI page = previousPage as Page_LineManagementUI;
                if (page != null)
                {
                    page.Reload();
                }
            }
        }

    }
}
