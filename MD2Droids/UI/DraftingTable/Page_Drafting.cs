using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Page_Drafting : Layer_Window
    {
        public static readonly Vector2 WinSize = new Vector2(694f, 701f);
        public static readonly Vector2 ButtonSize = new Vector2(120f, 30f);

        public const float Margin = 18f;
        public const float HalfMargin = Margin / 2f;

        private Blueprint _bp;

        private Vector2 baseScrollPos = default(Vector2);
        private Vector2 specialScrollPos = default(Vector2);
        private Vector2 costListScrollPos = default(Vector2);

        private Rect titleRect = new Rect(0f, 0f, 250f, 66f);
        private Rect texturesRect = new Rect(0f, 66f, 200f, 272f);
        private Rect batteryRect = new Rect(0f, 338f, 200f, 66f);
        private Rect costListRect = new Rect(0f, 404f, 292f, 195f);
        private Rect saveLoadRect = new Rect(0f, 599f, WinSize.x - 36f, 66f);
        private Rect powerRect;
        private Rect extrasRect = new Rect(200f, 188f, 206f, 216f);
        private Rect baseWorkRect = new Rect(406f, 66f, 252f, 198f);
        private Rect specialWorkRect = new Rect(406f, 264f, 252f, 198f);

        private List<GraphicDef> bodyGraphicDefs = new List<GraphicDef>();
        private List<GraphicDef> headGraphicDefs = new List<GraphicDef>();
        private int _bIndex;
        private int _hIndex;

        public Page_Drafting(Blueprint? bp = null)
        {
            _bp = bp ?? Blueprint.Default;

            drawPriority = 2000;
            clearNonEditWindows = true;
            absorbAllInput = true;
            forcePause = true;
            closeOnEscapeKey = true;
            doCloseX = true;
            SetCentered(WinSize.x, WinSize.y);

            headGraphicDefs = DefDatabase<GraphicDef>.AllDefs.Where((d) => d.isHead).OrderBy((a) => a.label).ToList();
            bodyGraphicDefs = DefDatabase<GraphicDef>.AllDefs.Where(d => !d.isHead).OrderBy(a => a.label).ToList();

            try
            {
                BodyIndex = bodyGraphicDefs.IndexOf(_bp.BodyGraphicDef);
                if (_bp.HeadGraphicDef != null)
                    HeadIndex = headGraphicDefs.IndexOf(_bp.HeadGraphicDef);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public Blueprint Blueprint
        {
            get { return _bp; }
            set { _bp = value; }
        }

        protected override void FillWindow(Rect inRect)
        {
            base.FillWindow(inRect);
            GUI.BeginGroup(inRect);

            Rect titleBgRect = titleRect.ContractedBy(HalfMargin);
            //GUI.DrawTexture(titleBgRect, SolidColorMaterials.NewSolidColorTexture(Color.white));
            DrawTitleLabel(titleBgRect.ContractedBy(HalfMargin));

            Rect texturesBgRect = texturesRect.ContractedBy(HalfMargin);
            GUI.DrawTexture(texturesBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawTextureChooser(texturesBgRect.ContractedBy(HalfMargin));

            Rect batteryBgRect = batteryRect.ContractedBy(HalfMargin);
            //GUI.DrawTexture(batteryBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawBatteryChooser(batteryBgRect.ContractedBy(HalfMargin));

            Rect costListBgRect = costListRect.ContractedBy(HalfMargin);
            GUI.DrawTexture(costListBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawCostList(costListBgRect.ContractedBy(HalfMargin));

            Rect saveLoadBgRect = saveLoadRect.ContractedBy(HalfMargin);
            //GUI.DrawTexture(saveLoadBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawSaveLoadButton(saveLoadBgRect.ContractedBy(HalfMargin));

            Rect extrasBgRect = extrasRect.ContractedBy(HalfMargin);
            GUI.DrawTexture(extrasBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawExtras(extrasBgRect.ContractedBy(HalfMargin));

            Rect baseWorkBgRect = baseWorkRect.ContractedBy(HalfMargin);
            GUI.DrawTexture(baseWorkBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawBaseWorkChooser(baseWorkBgRect.ContractedBy(HalfMargin));

            Rect specialWorkBgRect = specialWorkRect.ContractedBy(HalfMargin);
            GUI.DrawTexture(specialWorkBgRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawSpecialWorkChooser(specialWorkBgRect.ContractedBy(HalfMargin));

            powerRect = new Rect(inRect.xMax - 200f, inRect.height - 66f - 18f - 30f, 200f, 30f);
            GUI.DrawTexture(powerRect, SolidColorMaterials.NewSolidColorTexture(Color.black));
            DrawPowerRequirement(powerRect);

            GUI.EndGroup();
        }

        private void DrawSpecialWorkChooser(Rect rect)
        {
            Widget.ScrollBoxWithButton(rect, "Specialisations".Translate(),
                (from t in _bp.WorkPackages
                 where t.specialist
                 orderby t.label
                 select t).ToList(), ref specialScrollPos, "Edit".Translate(), true,
                delegate { Find.LayerStack.Add(new Dialog_WorkPackageSelection(ref _bp, (p) => p.specialist)); },
                null, "BlueprintEditTooltip".Translate());
        }

        private void DrawBaseWorkChooser(Rect rect)
        {
            Widget.ScrollBoxWithButton(rect, "WorkPackages".Translate(), (from t in _bp.WorkPackages
                                                                          where !t.specialist
                                                                          orderby t.label
                                                                          select t).ToList(), ref baseScrollPos, "Edit".Translate(), true,
                delegate
                {
                    Find.LayerStack.Add(new Dialog_WorkPackageSelection(ref _bp, (p) => !p.specialist));
                }, null, "BlueprintEditTooltip".Translate());
        }

        private void DrawExtras(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Listing_Standard listing = new Listing_Standard(rect.ContractedBy(5f));

            listing.DoLabelCheckbox("ExplodeOnDeath".Translate(), ref _bp.ExplodeOnDeath);
            if (_bp.ExplodeOnDeath)
            {
                listing.DoLabel("ExplosionRadius".Translate(_bp.ExplosionRadius.ToString("0.0")));
                _bp.ExplosionRadius = listing.DoSlider(_bp.ExplosionRadius, 0.9f, 3.9f);
            }

            listing.DoLabel("StartingSkill".Translate(_bp.StartingSkillLevel));
            _bp.StartingSkillLevel = (int)listing.DoSlider(_bp.StartingSkillLevel, 1, 20);

            if (listing.DoTextButtonLabeled("SkillPassion".Translate(), _bp.SkillPassion.ToString()))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();

                foreach (Passion p in Enum.GetValues(typeof(Passion)))
                {
                    if (p != _bp.SkillPassion)
                    {
                        FloatMenuOption option = new FloatMenuOption(p.ToString(), () => _bp.SkillPassion = p);
                        list.Add(option);
                    }
                }
                Find.LayerStack.Add(new Layer_FloatMenu(list));
            }
            listing.End();
        }

        private void DrawCostList(Rect rect)
        {
            try
            {
                GUI.BeginGroup(rect);

                Rect labelRect = new Rect(0f, 0f, rect.width, 25f);
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(labelRect, "CostList".Translate());

                Rect workAmountRect = new Rect(0f, rect.height - 25f, rect.width, 25f);
                Widgets.Label(workAmountRect, "MD2WorkAmount".Translate(_bp.WorkAmount.ToString("0")));
                Text.Anchor = TextAnchor.UpperLeft;

                Rect outRect = new Rect(0f, labelRect.yMax, rect.width,
                    rect.height - labelRect.height - workAmountRect.height);
                List<ThingCount> list = _bp.CostList.OrderByDescending(t => t.count).ToList();
                float height = list.Count * 30f;
                Rect viewRect = new Rect(0f, outRect.y, outRect.width - 16f, height);

                float curY = outRect.y;
                bool alternate = false;

                Widgets.DrawMenuSection(outRect);
                Widgets.BeginScrollView(outRect, ref costListScrollPos, viewRect);
                foreach (var count in list)
                {
                    #region Draw entry
                    Rect entryRect = new Rect(0f, curY, viewRect.width, 30f);

                    if (entryRect.Contains(Event.current.mousePosition) && outRect.Contains(Event.current.mousePosition))
                        Widgets.DrawHighlight(entryRect);
                    else if (alternate)
                        Widgets.DrawAltRect(entryRect);
                    GUI.BeginGroup(entryRect);

                    Vector2 iconSize = new Vector2(25f, 25f);
                    Rect iconRect = new Rect(0f, entryRect.height / 2f - iconSize.x / 2f, iconSize.x, iconSize.y);
                    GUI.DrawTexture(iconRect, count.thingDef.uiIcon);

                    Rect countRect = new Rect(entryRect.width - 25f, 0f, 25f, 30f);
                    Text.Anchor = TextAnchor.MiddleRight;
                    Widgets.Label(countRect, count.count.ToString());

                    Text.Anchor = TextAnchor.MiddleLeft;
                    Rect countLabelRect = new Rect(iconSize.x + 5f, 0f, entryRect.width - 30f - 30f, 30f);
                    Widgets.Label(countLabelRect, count.thingDef.LabelCap);
                    Text.Anchor = TextAnchor.UpperLeft;

                    GUI.EndGroup();

                    curY += 30f;
                    alternate = !alternate;
                    #endregion
                }
            }
            catch (Exception e)
            {
                Log.Error("Error drawing cost list " + e.ToString());
            }
            finally
            {
                Widgets.EndScrollView();
                GUI.EndGroup();
            }
        }

        private void DrawPowerRequirement(Rect rect)
        {
            string s = "PowerUsage".Translate(_bp.PowerUsage.ToString("0.#"));
            if (rect.Contains(Event.current.mousePosition))
                Widgets.DrawHighlight(rect);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, s);
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect, "PowerUsageTooltip".Translate());
        }

        private void DrawTitleLabel(Rect rect)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Medium;
            Widgets.Label(rect, "ConfigureBlueprint".Translate());
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        private void DrawTextureChooser(Rect rect)
        {
            try
            {
                GUI.BeginGroup(rect);

                float midPoint = rect.width / 2f;

                Vector2 texSize = new Vector2(128f, 128f);
                Vector2 texPos = new Vector2(midPoint - texSize.x / 2f, 0f);
                Rect texAreaRect = new Rect(0f, texPos.y, rect.width,
                    texSize.y + 30f);
                Rect bodyTexRect = new Rect(texPos.x, texPos.y + 30f, texSize.x, texSize.y);
                Rect headTexRect = new Rect(texPos.x, texPos.y, texSize.x, texSize.y);

                //Draw texture area background and surrounding box
                GUI.DrawTexture(texAreaRect, Widget.TextureChooserBg);
                Widgets.DrawBox(texAreaRect);

                //Draw body texture
                GUI.DrawTexture(bodyTexRect, _bp.BodyGraphicDef.graphicData.Graphic.MatFront.mainTexture);
                //Draw head texture
                if (_bp.BodyGraphicDef.supportsHead && _bp.HeadGraphicDef != null)
                    GUI.DrawTexture(headTexRect, _bp.HeadGraphicDef.graphicData.Graphic.MatFront.mainTexture);

                Rect headSelectorRect = new Rect(0f, texAreaRect.yMax + HalfMargin,
                    rect.width, 30f);
                Rect bodySelectorRect = new Rect(headSelectorRect.x, headSelectorRect.yMax + HalfMargin, headSelectorRect.width, 30f);

                //Body selector
                Widget.LeftRightSelector(bodySelectorRect, _bp.BodyGraphicDef.label, delegate
                {
                    int index = BodyIndex--;
                    _bp.BodyGraphicDef = bodyGraphicDefs[index];
                }, delegate
                {
                    int index = BodyIndex++;
                    _bp.BodyGraphicDef = bodyGraphicDefs[index];
                });

                //Head selector
                string headLabel = _bp.BodyGraphicDef.supportsHead ? _bp.HeadGraphicDef.label : "NoHead".Translate();
                Widget.LeftRightSelector(headSelectorRect, headLabel,
                    delegate
                    {
                        int index = HeadIndex--;
                        _bp.HeadGraphicDef = headGraphicDefs[index];
                    }, delegate
                    {
                        int index = HeadIndex++;
                        _bp.HeadGraphicDef = headGraphicDefs[index];
                    },
                    !_bp.BodyGraphicDef.supportsHead);
            }
            catch (Exception ex)
            {
                Log.Error("Error drawing droid texture: " + ex.ToString());
            }
            finally
            {
                GUI.EndGroup();
            }

        }

        private void DrawSaveLoadButton(Rect rect)
        {
            GUI.BeginGroup(rect);

            Rect loadButtonRect = new Rect(rect.width / 2f - ButtonSize.x - HalfMargin, 0f, ButtonSize.x, ButtonSize.y);
            if (Widgets.TextButton(loadButtonRect, "BlueprintLoad".Translate()))
            {
                //TODO:: this
                Find.LayerStack.Add(new Dialog_LoadBlueprint(this));
            }

            Rect saveButtonRect = new Rect(rect.width / 2f + HalfMargin, 0f, ButtonSize.x, ButtonSize.y);
            if (Widgets.TextButton(saveButtonRect, "BlueprintSave".Translate()))
            {
                //TODO:: this
                Find.LayerStack.Add(new Dialog_SaveBlueprint(ref _bp));
            }

            GUI.EndGroup();
        }

        private void DrawBatteryChooser(Rect rect)
        {
            GUI.BeginGroup(rect);
            Rect batteryButtonRect = new Rect(0f, 0f, rect.width, ButtonSize.y);

            if (Widgets.TextButton(batteryButtonRect, _bp.BatteryDef.LabelCap))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach (var def in (from d in DefDatabase<ThingDef>.AllDefs
                                     where d != _bp.BatteryDef && d.statBases != null && d.statBases.Any((s) => s.stat == StatDef.Named("MD2PowerCellMaxStorage"))
                                     orderby d.label
                                     select d).ToList())
                {
                    ThingDef tDef = def;
                    FloatMenuOption o = new FloatMenuOption(def.label, delegate { _bp.BatteryDef = tDef; });
                    options.Add(o);
                }
                Find.LayerStack.Add(new Layer_FloatMenu(options, true));
            }
            GUI.EndGroup();

        }

        private int HeadIndex
        {
            get { return _hIndex; }
            set
            {
                if (value < 0)
                    _hIndex = headGraphicDefs.Count - 1;
                else if (value > headGraphicDefs.Count - 1)
                    _hIndex = 0;
                else
                    _hIndex = value;
            }
        }

        private int BodyIndex
        {
            get { return _bIndex; }
            set
            {
                if (value < 0)
                    _bIndex = bodyGraphicDefs.Count - 1;
                else if (value > bodyGraphicDefs.Count - 1)
                    _bIndex = 0;
                else
                    _bIndex = value;
            }
        }



    }
}
