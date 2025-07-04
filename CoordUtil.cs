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
    }
}
