using Verse;

namespace MD2
{
    public static class ThingExtension
    {
        public static void TakeFrom(this Thing thing, int count)
        {
            if (count > thing.stackCount)
            {

                Log.Error(string.Concat(new object[]
                        {
                            "Tried to take ",
                            count,
                            "from",
                            thing,
                            ", but there are only",
                            thing.stackCount
                        }));

                return;
            }
            thing.stackCount -= count;
            if (thing.stackCount <= 0)
            {
                thing.DeSpawn();
            }
        }
    }
}
