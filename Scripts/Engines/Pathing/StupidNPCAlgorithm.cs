using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.PathAlgorithms;
using CalcMoves = Server.Movement.Movement;
using MoveImpl = Server.Movement.MovementImpl;

namespace Server.PathAlgorithms
{
    public class StupidNPCPathing : PathAlgorithm
    {
        public static readonly PathAlgorithm Instance = new StupidNPCPathing();

        private static void Offset(Direction d, ref Point3D p)
        {
            Offset( ((int)(d&Direction.Mask)), ref p );
        }

        private static int GetDirection(IPoint2D from, IPoint2D to)
        {
            int dx = from.X - to.X;
            int dy = from.Y - to.Y;

            int rx = (dx - dy) * 44;
            int ry = (dx + dy) * 44;

            int ax = Math.Abs(rx);
            int ay = Math.Abs(ry);

            Direction ret;

            if (((ay >> 1) - ax) >= 0)
                ret = (ry > 0) ? Direction.Up : Direction.Down;
            else if (((ax >> 1) - ay) >= 0)
                ret = (rx > 0) ? Direction.Left : Direction.Right;
            else if (rx >= 0 && ry >= 0)
                ret = Direction.West;
            else if (rx >= 0 && ry < 0)
                ret = Direction.South;
            else if (rx < 0 && ry < 0)
                ret = Direction.East;
            else
                ret = Direction.North;

            return (int)ret;
        }

        private static void Offset(int d, ref Point3D p)
        {
            int x, y;
            switch (d)
            {
                default:
                case 0: x = 0; y = -1; break;
                case 1: x = 1; y = -1; break;
                case 2: x = 1; y = 0; break;
                case 3: x = 1; y = 1; break;
                case 4: x = 0; y = 1; break;
                case 5: x = -1; y = 1; break;
                case 6: x = -1; y = 0; break;
                case 7: x = -1; y = -1; break;
            }

            p.X += x; p.Y += y;
        }

        public override bool CheckCondition(Mobile m, Map map, Point3D start, Point3D goal)
        {
            return true;
        }

        private static int[][] tries = new int[][]{ 
            new int[]{ 0, 1, 7, 4, 2, 6, 3, 5 },
            new int[]{ 0, 1, 7, 4, 3, 5, 2, 6 },
            new int[]{ 0, 7, 1, 3, 4, 2, 5, 6 },
            new int[]{ 0, 7, 1, 2, 4, 3, 6, 5 },
            new int[]{ 0, 4, 1, 7, 2, 6, 3, 5 },
            new int[]{ 0, 4, 7, 1, 6, 2, 5, 3 },
            new int[]{ 0, 4, 1, 7, 3, 5, 6, 2 },
            new int[]{ 0, 4, 7, 1, 5, 3, 2, 6 },
            new int[]{ 0, 2, 6, 1, 7, 3, 5, 4 },
            new int[]{ 0, 6, 2, 7, 1, 5, 3, 4 },
        };

        public override Direction[] Find(Mobile m, Map map, Point3D start, Point3D goal)
        {
            int z = start.Z;
            int dBase = 0;
            Point3D pos = new Point3D( start );
            List<Direction> path = new List<Direction>();

            //Console.WriteLine("StupidPathing from {0} to {1}", start, goal);
            for (int step = 0; step < 3; step++)
            {
                bool ok = false;
                int t, i, d = 0;

                if ((step % 2) == 0)
                    dBase = GetDirection( pos, goal );

                t = Utility.Random( tries.Length );
                for (i = 0; i < tries[t].Length; i++)
                {
                    d = (dBase + tries[t][i]) % 8;
                    z = pos.Z;

                    ok = CalcMoves.CheckMovement( m, map, pos, (Direction)d, out z );
                    if ( ok )
                        break;
                }

                if ( !ok )
                    break;

                //Console.WriteLine("Direction {0} at Try[{1}][{2}] worked.", d, t, i);

                if (path.Count == 0) path.Add( (Direction)dBase ); // always make the first rule to try and go in the proper direction
                path.Add( (Direction)d );
                
                Offset( d, ref pos );
                dBase = d;
                pos.Z = z;
            }

            //Console.WriteLine("Done pathing, got {0} steps.", path.Count);

            if (path.Count == 0)
                return null;
            else
                return path.ToArray();
        }
    }
}
