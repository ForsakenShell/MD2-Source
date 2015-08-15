using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace MD2
{
    public static class CorpseFinderUtility
    {
        public static Corpse FindClosestCorpseFor(Predicate<Thing> pred, Pawn getter)
        {
            Predicate<Thing> predicate1 = (Thing c) => !c.IsForbidden(Faction.OfColony) && getter.AwareOf(c) && getter.CanReserve(c);
            Predicate<Thing> predicate = (Thing t) => pred(t) && predicate1(t);
            ThingRequest thingReq = ThingRequest.ForGroup(ThingRequestGroup.Corpse);
            return (Corpse)GenClosest.ClosestThingReachable(getter.Position, thingReq, PathEndMode.ClosestTouch, TraverseParms.For(getter), 9999f, predicate, null);
        }
    }
}
