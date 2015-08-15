using System.Collections.Generic;
using Verse;

namespace MD2
{
    public class ManufacturingPlantRecipesDef : Def
    {
        public List<RecipeDef> recipes = new List<RecipeDef>();
        public List<RecipeDef> blackList = new List<RecipeDef>();
    }
}
