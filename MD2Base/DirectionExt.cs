using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public static class DirectionExt
    {
        static Random rnd = new Random();
        public static Direction Opposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    throw new ArgumentException();
            }
        }

        public static Direction Random(this Direction dir)
        {
            List<Direction> list = new List<Direction>() { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
            int num = rnd.Next(list.Count);
            return list[num];
        }

        public static Rot4 ToRot4(this Direction dir)
        {
            switch(dir)
            {
                case Direction.Up:
                    return Rot4.North;
                case Direction.Down:
                    return Rot4.South;
                case Direction.Left:
                    return Rot4.East;
                case Direction.Right:
                    return Rot4.West;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
