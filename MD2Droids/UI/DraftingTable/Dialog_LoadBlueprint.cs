using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Dialog_LoadBlueprint : Dialog_Blueprint
    {
        protected readonly Page_Drafting _oldPage;
        public Dialog_LoadBlueprint(Page_Drafting page)
        {
            this.interactButLabel = "BlueprintLoad".Translate();
            _oldPage = page;
        }
        protected override void DoMapEntryInteraction(string fileName)
        {
            Blueprint bp = BlueprintFiles.LoadFromFile(fileName);
            Messages.Message("BlueprintLoaded".Translate(fileName), MessageSound.Standard);
            _oldPage.Blueprint = bp;
            Close();
        }
    }
}
