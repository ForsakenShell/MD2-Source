using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class RecipeListGenerator
    {
        public static IEnumerable<RecipeDef> AllRecipes()
        {
            List<RecipeDef> list = new List<RecipeDef>();

            foreach (var def in DefDatabase<ManufacturingPlantRecipesDef>.AllDefs)
            {
                foreach (var recipe in def.recipes)
                {
                    list.Add(recipe);
                }
            }
            foreach (var def in DefDatabase<RecipeDef>.AllDefs.Where(d => d.workSkill != null && (d.workSkill == SkillDefOf.Crafting || d.workSkill == SkillDefOf.Cooking || d.workSkill == SkillDefOf.Artistic)))
            {
                if(!list.Contains(def))
                    list.Add(def);
            }
            foreach (var def in DefDatabase<ManufacturingPlantRecipesDef>.AllDefs)
            {
                if (def.blackList != null)
                {
                    foreach (var recipe in def.blackList)
                    {
                        if (list.Contains(recipe))
                        {
                            list.Remove(recipe);
                        }
                    }
                }
            }
            return list.AsEnumerable();
        }
    }
}
