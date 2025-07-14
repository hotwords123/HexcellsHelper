using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public record struct Coordinate(int X, int Y)
    {
        public static Coordinate operator +(Coordinate a, Coordinate b) =>
            new(a.X + b.X, a.Y + b.Y);
    }

    public static class CoordUtil
    {
        public const int Width = 33;
        public const int Height = 33;

        public static bool IsValidCoord(int x, int y)
        {
            return 0 <= x && x < Width && 0 <= y && y < Height;
        }
        public static bool IsValidCoord(Coordinate coord)
        {
            return IsValidCoord(coord.X, coord.Y);
        }

        public static Coordinate WorldToGrid(Vector3 vec)
        {
            return new(
                Mathf.RoundToInt(vec.x / 0.88f) + 15,
                Mathf.RoundToInt(vec.y / 0.5f) + 15
            );
        }

        public static IEnumerable<Coordinate> AllCoords()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return new(x, y);
                }
            }
        }

        public static readonly Coordinate[] SurroundOffsets =
        [
            new(0, 2), new(1, 1), new(1, -1), new(0, -2),
            new(-1, -1), new(-1, 1)
        ];

        public static readonly Coordinate[] FlowerOffsets =
        [
            new(0, 2), new(1, 1), new(1, -1), new(0, -2),
            new(-1, -1), new(-1, 1), new(0, 4), new(1, 3),
            new(2, 2), new(2, 0), new(2, -2), new(1, -3),
            new(0, -4), new(-1, -3), new(-2, -2), new(-2, 0),
            new(-2, 2), new(-1, 3),
        ];

        public static IEnumerable<Coordinate> SurroundCoords(Coordinate coord)
        {
            return SurroundOffsets.Select(offset => coord + offset);
        }
        public static IEnumerable<Coordinate> FlowerCoords(Coordinate coord)
        {
            return FlowerOffsets.Select(offset => coord + offset);
        }

        public static IEnumerable<Coordinate> DiagonalLeftCoords(Coordinate coord)
        {
            var x = coord.X - 1;
            var y = coord.Y - 1;
            while (IsValidCoord(x, y))
            {
                yield return new(x, y);
                x--;
                y--;
            }
        }
        public static IEnumerable<Coordinate> DiagonalRightCoords(Coordinate coord)
        {
            var x = coord.X + 1;
            var y = coord.Y - 1;
            while (IsValidCoord(x, y))
            {
                yield return new(x, y);
                x++;
                y--;
            }
        }

        public static IEnumerable<Coordinate> VerticalCoords(Coordinate coord)
        {
            var x = coord.X;
            var y = coord.Y - 2;
            while (IsValidCoord(x, y))
            {
                yield return new(x, y);
                y -= 2;
            }
        }
    }
}
