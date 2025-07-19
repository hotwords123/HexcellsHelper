using UnityEngine;

namespace HexcellsHelper
{
    public static class MapUtil
    {
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

            return hexGO.tag == "Blue" ? CellType.Blue : CellType.Black;
        }

        public static bool IsPreRevealed(GameObject hexGO)
        {
            return hexGO.layer == LayerMask.NameToLayer("Pre-Revealed");
        }

        public static bool IsBlackHexBlank(GameObject hexGO)
        {
            return hexGO.tag == "Clue Hex Blank";
        }

        public static Clue.Modifier GetBlackHexModifier(GameObject hexGO)
        {
            return hexGO.tag switch
            {
                "Clue Hex (Sequential)" => Clue.Modifier.Consecutive,
                "Clue Hex (NOT Sequential)" => Clue.Modifier.NonConsecutive,
                _ => Clue.Modifier.None,
            };
        }

        public static bool IsBlueHexFlower(GameObject hexGO)
        {
            return NormalizeName(hexGO.name) == "Blue Hex (Flower)";
        }

        public static ColumnType GetColumnType(GameObject columnGO)
        {
            return NormalizeName(columnGO.name) switch
            {
                "Column Number Diagonal Left" => ColumnType.DiagonalLeft,
                "Column Number Diagonal Right" => ColumnType.DiagonalRight,
                _ => ColumnType.Vertical,
            };
        }

        public static Clue.Modifier GetColumnModifier(GameObject columnGO)
        {
            return columnGO.tag switch
            {
                "Column Sequential" => Clue.Modifier.Consecutive,
                "Column NOT Sequential" => Clue.Modifier.NonConsecutive,
                _ => Clue.Modifier.None,
            };
        }
    }
}
