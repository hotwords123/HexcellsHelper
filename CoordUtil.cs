using UnityEngine;

namespace HexcellsHelper
{
    public static class CoordUtil
    {
        public const int Width = 33;
        public const int Height = 33;

        public static bool IsValidCoord(int x, int y)
        {
            return 0 <= x && x < Width && 0 <= y && y < Height;
        }
        public static bool IsValidCoord(Coordinate coordinate)
        {
            return IsValidCoord(coordinate.x, coordinate.y);
        }

        public static Coordinate WorldToGrid(Vector3 vec)
        {
            return new Coordinate(
                Mathf.RoundToInt(vec.x / 0.88f) + 15,
                Mathf.RoundToInt(vec.y / 0.5f) + 15
            );
        }


        public static readonly Coordinate[] surroundCoords =
        [
            new Coordinate(0, 2),
            new Coordinate(1, 1),
            new Coordinate(1, -1),
            new Coordinate(0, -2),
            new Coordinate(-1, -1),
            new Coordinate(-1, 1),
        ];

        public static readonly Coordinate[] flowerCoords =
        [
            new Coordinate(0, 2),
            new Coordinate(1, 1),
            new Coordinate(1, -1),
            new Coordinate(0, -2),
            new Coordinate(-1, -1),
            new Coordinate(-1, 1),
            new Coordinate(0, 4),
            new Coordinate(1, 3),
            new Coordinate(2, 2),
            new Coordinate(2, 0),
            new Coordinate(2, -2),
            new Coordinate(1, -3),
            new Coordinate(0, -4),
            new Coordinate(-1, -3),
            new Coordinate(-2, -2),
            new Coordinate(-2, 0),
            new Coordinate(-2, 2),
            new Coordinate(-1, 3),
        ];

        public static readonly Coordinate diagonalLeftCoord = new(-1, -1);
        public static readonly Coordinate diagonalRightCoord = new(1, -1);
        public static readonly Coordinate verticalCoord = new(0, -2);
    }
}
