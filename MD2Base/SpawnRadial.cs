using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class SpawnRadial
    {
        private static int count = 0;

        public static bool Spawn(ThingDef thingDef, int numberToSpawn, IntVec3 initialPos)
        {
            return Spawn(thingDef, numberToSpawn, initialPos, 35);
        }

        public static bool Spawn(ThingDef thingDef, int numberToSpawn, IntVec3 initialPos, int radiusMax)
        {
            int num = numberToSpawn;
            while (num > 0)
            {
                IntVec3 pos = initialPos + GenRadial.RadialPattern[count];
                if (pos.Standable() && pos.InBounds())
                {
                    List<Thing> list = Find.ThingGrid.ThingsListAt(pos);
                    if (list.Count != 0)
                    {
                        foreach (Thing thing in list)
                        {
                            //Thing thing = Find.ThingGrid.ThingAt(pos, thingDef);
                            if (thing.def.defName == thingDef.defName)
                            {
                                if (!(thing.stackCount == thing.def.stackLimit))
                                {
                                    int remainder = thing.def.stackLimit - thing.stackCount;
                                    if (num >= remainder)
                                    {
                                        AddToExistingStack(thing, remainder);
                                        num -= remainder;
                                    }
                                    else
                                    {
                                        AddToExistingStack(thing, num);
                                        num = 0;
                                    }
                                }
                            } //If foundThing not the same, leave
                        }
                    }
                    else
                    {
                        MakeNewStack(thingDef, num, pos);
                        num = 0;
                    }
                }
                count++;
                if (count > radiusMax)
                {
                    break;
                }
            } //End of loop
            count = 0;
            if (num == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void MakeNewStack(ThingDef def, int num, IntVec3 pos)
        {

            Thing thing = ThingMaker.MakeThing(def);
            thing.stackCount = num;
            GenSpawn.Spawn(thing, pos);
        }

        public static void AddToExistingStack(Thing thing, int num)
        {
            thing.stackCount += num;
            if (thing.stackCount > thing.def.stackLimit)
            {
                thing.stackCount = thing.def.stackLimit;
            }
        }
    }
}
