using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Dialog_SaveBlueprint : Dialog_Blueprint
    {
        protected string FileName;
        private bool focusedBlueprintNameField = false;
        protected Blueprint _bp;

        public Dialog_SaveBlueprint(ref Blueprint bp)
        {
            FileName = string.Empty;
            this.interactButLabel = "BlueprintSave".Translate();
            bottomAreaHeight = 85f;
            FileName = BlueprintFiles.UnusedDefaultName();
            _bp = bp;
        }

        protected override void DoMapEntryInteraction(string blueprintName)
        {
            FileName = blueprintName;
            BlueprintFiles.SaveToFile(ref _bp, FileName);
            Messages.Message("SavedAs".Translate(FileName), MessageSound.Standard);
            Close();
        }

        protected override void DoSpecialSaveLoadGUI(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;
            float top = inRect.height - 52f;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.SetNextControlName("BlueprintNameField");
            Rect rect = new Rect(5f, top, 400f, 35f);
            string text = Widgets.TextField(rect, FileName);
            if (GenText.IsValidFilename(text))
            {
                FileName = text;
            }
            if (!this.focusedBlueprintNameField)
            {
                GUI.FocusControl("BlueprintNameField");
                this.focusedBlueprintNameField = true;
            }
            Rect rect2 = new Rect(420f, top, inRect.width - 400f - 20f, 35f);
            if (Widgets.TextButton(rect2, "BlueprintSave".Translate(), true, false) || flag)
            {
                if (FileName.Length == 0)
                {
                    Messages.Message("BlueprintNeedFileName".Translate(), MessageSound.RejectInput);
                }
                else
                {
                    BlueprintFiles.SaveToFile(ref _bp, FileName);
                    Messages.Message("SavedAs".Translate(new object[]
					{
						FileName
					}), MessageSound.Standard);
                    base.Close(true);
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
        }
    }
}
