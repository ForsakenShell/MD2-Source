using Backstories;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class BackstoryManager
    {
        private Droid parent;
        private BackstoryDef backstoryDef;

        public BackstoryManager(Droid droid)
        {
            this.parent = droid;
        }

        public void SpawnSetup()
        {
            backstoryDef = new BackstoryDef();
            backstoryDef.defName = ListerDroids.AllDroids.Count + "DroidBS";
            backstoryDef.bodyTypeFemale = BodyType.Male;
            backstoryDef.bodyTypeMale = BodyType.Male;
            backstoryDef.title = "Droid";
            backstoryDef.titleShort = "Droid";
            backstoryDef.baseDescription = "A droid.";
            backstoryDef.shuffleable = false;
            backstoryDef.saveKeyIdentifier = "MD2";
            backstoryDef.slot = BackstorySlot.Childhood;
            backstoryDef.spawnCategories = new List<string>() { "Civil" };

            List<WorkTags> workTags = new List<WorkTags>();
            foreach(var t in parent.work.AllRequiredWorkTags)
            {
                if (!workTags.Contains(t))
                    workTags.Add(t);
            }
            backstoryDef.workAllows = workTags;

            backstoryDef.ResolveReferences();
            parent.story.adulthood = BackstoryDatabase.GetWithKey(backstoryDef.UniqueSaveKeyFor());
            parent.story.childhood = BackstoryDatabase.GetWithKey(backstoryDef.UniqueSaveKeyFor());
        }
    }
}
