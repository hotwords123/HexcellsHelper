using System.Collections.Generic;
using UnityEngine;

namespace HexcellsHelper
{
    public record struct Coordinate(int X, int Y);
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

        public static IEnumerable<Coordinate> IterGrid()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return new(x, y);
                }
            }
        }

        public static IEnumerable<Coordinate> SurroundCoords(Coordinate coord)
        {
            yield return new(coord.X + 0, coord.Y + 2);
            yield return new(coord.X + 1, coord.Y + 1);
            yield return new(coord.X + 1, coord.Y - 1);
            yield return new(coord.X + 0, coord.Y - 2);
            yield return new(coord.X + -1, coord.Y - 1);
            yield return new(coord.X + -1, coord.Y + 1);
        }
        public static IEnumerable<Coordinate> FlowerCoords(Coordinate coord)
        {
            yield return new(coord.X + 0, coord.Y + 2);
            yield return new(coord.X + 1, coord.Y + 1);
            yield return new(coord.X + 1, coord.Y - 1);
            yield return new(coord.X + 0, coord.Y - 2);
            yield return new(coord.X - 1, coord.Y - 1);
            yield return new(coord.X - 1, coord.Y + 1);
            yield return new(coord.X + 0, coord.Y + 4);
            yield return new(coord.X + 1, coord.Y + 3);
            yield return new(coord.X + 2, coord.Y + 2);
            yield return new(coord.X + 2, coord.Y + 0);
            yield return new(coord.X + 2, coord.Y - 2);
            yield return new(coord.X + 1, coord.Y - 3);
            yield return new(coord.X + 0, coord.Y - 4);
            yield return new(coord.X - 1, coord.Y - 3);
            yield return new(coord.X - 2, coord.Y - 2);
            yield return new(coord.X - 2, coord.Y + 0);
            yield return new(coord.X - 2, coord.Y + 2);
            yield return new(coord.X - 1, coord.Y + 3);
        }
        public static IEnumerable<Coordinate> DiagonalLeftCoord(Coordinate coord)
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
        public static IEnumerable<Coordinate> DiagonalRightCoord(Coordinate coord)
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

        public static IEnumerable<Coordinate> VerticalCoord(Coordinate coord)
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
