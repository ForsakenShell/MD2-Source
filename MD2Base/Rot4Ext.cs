using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class Rot4Ext
    {
        public static Direction ToDirection(this Rot4 rot)
        {
                if(rot== Rot4.North)
                    return Direction.Up;
                if(rot== Rot4.South)
                    return Direction.Down;
                if(rot== Rot4.East)
                    return Direction.Left;
                if(rot== Rot4.West)
                    return Direction.Right;
                throw new ArgumentOutOfRangeException();
        }
    }
}
