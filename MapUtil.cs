using UnityEngine;

namespace HexcellsHelper
{
    public static class MapUtil
    {
        public static readonly int PreRevealedLayer = LayerMask.NameToLayer("Pre-Revealed");

        public const string BlueTag = "Blue";
        public const string BlackHexBlankTag = "Clue Hex Blank";
        public const string ClueHexSequentialTag = "Clue Hex (Sequential)";
        public const string ClueHexNonSequentialTag = "Clue Hex (NOT Sequential)";
        public const string BlueHexFlowerName = "Blue Hex (Flower)";
        public const string ColumnNumberDiagonalLeftName = "Column Number Diagonal Left";
        public const string ColumnNumberDiagonalRightName = "Column Number Diagonal Right";
        public const string ColumnConsecutiveTag = "Column Sequential";
        public const string ColumnNonConsecutiveTag = "Column NOT Sequential";

        public static string NormalizeName(string name)
        {
            return name.Replace("(Clone)", "").Trim();
        }

        public static CellType GetCellType(GameObject hexGO)
        {
            if (hexGO == null)
            {
                return CellType.Empty;
            }

            return hexGO.tag == BlueTag ? CellType.Blue : CellType.Black;
        }

        public static bool IsPreRevealed(GameObject hexGO)
        {
            return hexGO.layer == PreRevealedLayer;
        }

        public static bool IsBlackHexBlank(GameObject hexGO)
        {
            return hexGO.tag == BlackHexBlankTag;
        }

        public static Clue.Modifier GetBlackHexModifier(GameObject hexGO)
        {
            return hexGO.tag switch
            {
                ClueHexSequentialTag => Clue.Modifier.Consecutive,
                ClueHexNonSequentialTag => Clue.Modifier.NonConsecutive,
                _ => Clue.Modifier.None,
            };
        }

        public static bool IsBlueHexFlower(GameObject hexGO)
        {
            return NormalizeName(hexGO.name) == BlueHexFlowerName;
        }

        public static ColumnType GetColumnType(GameObject columnGO)
        {
            return NormalizeName(columnGO.name) switch
            {
                ColumnNumberDiagonalLeftName => ColumnType.DiagonalLeft,
                ColumnNumberDiagonalRightName => ColumnType.DiagonalRight,
                _ => ColumnType.Vertical,
            };
        }

        public static Clue.Modifier GetColumnModifier(GameObject columnGO)
        {
            return columnGO.tag switch
            {
                ColumnConsecutiveTag => Clue.Modifier.Consecutive,
                ColumnNonConsecutiveTag => Clue.Modifier.NonConsecutive,
                _ => Clue.Modifier.None,
            };
        }
    }
}
