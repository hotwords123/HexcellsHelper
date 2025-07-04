using UnityEngine;

namespace HexcellsHelper
{
    public static class CoordUtil
    {
        public const int Width = 33;
        public const int Height = 33;

        public static bool IsValidCoord(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public static int WorldToGridX(float worldX)
        {
            return Mathf.RoundToInt(worldX / 0.88f) + 15;
        }

        public static int WorldToGridY(float worldY)
        {
            return Mathf.RoundToInt(worldY / 0.5f) + 15;
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

        public static readonly Coordinate diagonalLeftCoord = new Coordinate(-1, -1);
        public static readonly Coordinate diagonalRightCoord = new Coordinate(1, -1);
        public static readonly Coordinate verticalCoord = new Coordinate(0, -2);
    }
}
