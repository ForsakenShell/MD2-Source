using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MD2
{
    public static class RecipeDefExtension
    {
        public static IEnumerable<Thing> MakeProducts(this RecipeDef def, Order order, int cycles)
        {
            if (cycles <= 0)
            {
                Log.Error(string.Concat(new Object[]{
                        "Order ",
                        def.defName,
                        " tried to make products using invalid number of cycles: ",
                        cycles.ToString()
                    }));
                yield break;
            }
            foreach (var current in def.products)
            {
                //Log.Message("Starting to make things");
                int count = current.count*cycles;
                while (count > 0)
                {
                    if (count <= 0)
                        break;
                    //Log.Message("Making things");
                    Thing thing;
                    if (current.thingDef.MadeFromStuff)
                    {
                        ThingDef stuffDef = order.ShoppingList.AcquiredMats.Where((ListItem i) => i.thing.stuffProps != null).First().thing;
                        if(stuffDef==null)
                        {
                            Log.Error(string.Concat(new object[]{
                                "Order ",
                                order.Recipe.LabelCap,
                                " tried to produce a thing with stuff, but no ingredients had stuffprops!"
                            }));
                        }
                        thing = ThingMaker.MakeThing(current.thingDef,stuffDef);
                    }
                    else
                    {
                        thing = ThingMaker.MakeThing(current.thingDef);
                    }
                    CompArt art = thing.TryGetComp<CompArt>();
                    if(art!=null)
                    {
                        art.GenerateTaleRef(ArtGenerationSource.Colony);
                    }
                    int num = UnityEngine.Mathf.Min(count, thing.def.stackLimit);
                    thing.stackCount = num;
                    count -= num;
                    //TODO
                    //Check to see if it has a quality level. If yes, then overrite it with the quality able to be produced by the line.
                    yield return thing;
                }
            }
        }

        public static IEnumerable<Thing> MakeProducts(this RecipeDef def, Order order)
        {
            return MakeProducts(def, order, 1);
        }
    }
}
