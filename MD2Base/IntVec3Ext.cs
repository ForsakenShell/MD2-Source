using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class IntVec3Ext
    {
        public static IntVec3 AdjacentInDirection(this IntVec3 pos, Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return pos += IntVec3.North;
                case Direction.Down:
                    return pos += IntVec3.South;
                case Direction.Left:
                    return pos += IntVec3.West;
                case Direction.Right:
                    return pos += IntVec3.East;
                default:
                    throw new ArgumentException();
            }
        }

        public static Direction GetDirectionFromTo(this IntVec3 basePos, IntVec3 targetPos)
        {
            if (basePos.x == targetPos.x)
            {
                if (targetPos.y > basePos.y)
                    return Direction.Up;
                else
                    return Direction.Down;
            }
            else if (basePos.z == targetPos.z)
            {
                if (targetPos.x > basePos.x)
                    return Direction.Left;
                else
                    return Direction.Right;
            }
            else if (basePos == targetPos)
                throw new ArgumentOutOfRangeException("Cannot to get direction to same cell");
            else
                throw new ArgumentOutOfRangeException("targetPos", targetPos, "Exception getting direction: basePos and targetPos are not in cardinal directions");
        }
    }
}
