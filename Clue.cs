using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class Clue
    {
        public enum Kind
        {
            Surround,
            Flower,
            Column,
            WholeLevel,
        }

        public enum Modifier
        {
            None,
            Consecutive,
            NonConsecutive,
        }

        public GameObject sourceGO;
        public Coordinate[] coords;
        public Kind kind;
        public Modifier modifier;

        public Coordinate[] hiddenCoords;
        public int hiddenBlacks;
        public int hiddenBlues;

        public bool IsComplete => hiddenCoords?.Length == 0;

        public static Clue FromHex(GameObject hexGO)
        {
            return MapUtil.GetCellType(hexGO) switch
            {
                CellType.Black => FromBlackHex(hexGO),
                CellType.Blue => FromBlueHex(hexGO),
                _ => null,
            };
        }

        public static Clue FromBlackHex(GameObject hexGO)
        {
            if (hexGO == null || MapUtil.IsBlackHexBlank(hexGO))
            {
                return null;
            }

            return new Clue
            {
                sourceGO = hexGO,
                coords = Coordinate.FromGameObject(hexGO).SurroundCoords().ToArray(),
                kind = Kind.Surround,
                modifier = MapUtil.GetBlackHexModifier(hexGO),
            };
        }

        public static Clue FromBlueHex(GameObject hexGO)
        {
            if (hexGO == null || !MapUtil.IsBlueHexFlower(hexGO))
            {
                return null;
            }

            return new Clue
            {
                sourceGO = hexGO,
                coords = Coordinate.FromGameObject(hexGO).FlowerCoords().ToArray(),
                kind = Kind.Flower,
                modifier = Modifier.None,
            };
        }

        public static Clue FromColumn(GameObject columnGO)
        {
            if (columnGO == null)
            {
                return null;
            }

            return new Clue
            {
                sourceGO = columnGO,
                coords = Coordinate.FromGameObject(columnGO)
                    .ColumnCoords(MapUtil.GetColumnType(columnGO))
                    .Where(MapManager.IsNonEmpty)
                    .ToArray(),
                kind = Kind.Column,
                modifier = MapUtil.GetColumnModifier(columnGO),
            };
        }

        public static Clue FromWholeLevel()
        {
            return new Clue
            {
                sourceGO = null,
                coords = MapManager.Hexes.Select(Coordinate.FromGameObject).ToArray(),
                kind = Kind.WholeLevel,
                modifier = Modifier.None,
            };
        }

        public void CountHidden()
        {
            hiddenCoords = coords.Where(MapManager.IsHidden).ToArray();
            hiddenBlacks = 0;
            hiddenBlues = 0;

            foreach (var coord in hiddenCoords)
            {
                if (MapManager.CellTypeAt(coord) == CellType.Blue)
                {
                    hiddenBlues++;
                }
                else
                {
                    hiddenBlacks++;
                }
            }
        }

        public bool TrySolve()
        {
            return TrySolveReallyTrivial() || TrySolveWithConstraint();
        }

        public bool TrySolveReallyTrivial()
        {
            if (hiddenCoords.Length == 0)
            {
                // All cells are revealed, nothing to solve
                return false;
            }

            if (hiddenBlacks == 0)
            {
                // All hidden cells are blue, we can solve it
                foreach (var coord in hiddenCoords)
                {
                    MapManager.SetBlue(coord);
                }
                return true;
            }
            if (hiddenBlues == 0)
            {
                // All hidden cells are black, we can solve it
                foreach (var coord in hiddenCoords)
                {
                    MapManager.SetBlack(coord);
                }
                return true;
            }

            // Otherwise, we can't solve it trivially
            return false;
        }

        public bool TrySolveWithConstraint()
        {
            if (hiddenCoords.Length == 0 || modifier == Modifier.None)
            {
                return false;
            }

            var isBlue = new bool[coords.Length];
            var canBeBlack = new bool[coords.Length];
            var canBeBlue = new bool[coords.Length];

            void Dfs(int index, int remainingBlacks, int remainingBlues)
            {
                if (index == coords.Length)
                {
                    // Check if the constraints are satisfied
                    bool shouldBeConsecutive = modifier == Modifier.Consecutive;
                    bool isConsecutive = AreBluesConsecutive(isBlue, kind == Kind.Surround);
                    if (shouldBeConsecutive == isConsecutive)
                    {
                        // Found a valid configuration
                        for (int i = 0; i < coords.Length; i++)
                        {
                            if (isBlue[i])
                            {
                                canBeBlue[i] = true;
                            }
                            else
                            {
                                canBeBlack[i] = true;
                            }
                        }
                    }
                    return;
                }

                var coord = coords[index];
                if (MapManager.IsNonEmpty(coord))
                {
                    // Skip already revealed cells
                    if (!MapManager.IsHidden(coord))
                    {
                        isBlue[index] = MapManager.CellTypeAt(coord) == CellType.Blue;
                        Dfs(index + 1, remainingBlacks, remainingBlues);
                        return;
                    }
                }
                else
                {
                    // Skip empty cells
                    isBlue[index] = false;
                    Dfs(index + 1, remainingBlacks, remainingBlues);
                    return;
                }

                // Try placing a black cell
                if (remainingBlacks > 0)
                {
                    isBlue[index] = false;
                    Dfs(index + 1, remainingBlacks - 1, remainingBlues);
                }

                // Try placing a blue cell
                if (remainingBlues > 0)
                {
                    isBlue[index] = true;
                    Dfs(index + 1, remainingBlacks, remainingBlues - 1);
                }
            }

            Dfs(0, hiddenBlacks, hiddenBlues);

            bool success = false;
            for (int i = 0; i < coords.Length; i++)
            {
                var coord = coords[i];
                if (MapManager.IsHidden(coord))
                {
                    if (!canBeBlue[i])
                    {
                        MapManager.SetBlack(coord);
                        success = true;
                    }
                    else if (!canBeBlack[i])
                    {
                        MapManager.SetBlue(coord);
                        success = true;
                    }
                }
            }
            return success;
        }

        public static bool AreBluesConsecutive(bool[] isBlue, bool isCircular)
        {
            int blueCount = 0, count = 0, maxCount = 0, prefixCount = 0;
            for (int i = 0; i < isBlue.Length; i++)
            {
                if (isBlue[i])
                {
                    blueCount++;
                    count++;
                    maxCount = Mathf.Max(maxCount, count);
                }
                else
                {
                    if (count == i)
                    {
                        prefixCount = count;
                    }
                    count = 0;
                }
            }
            if (isCircular)
            {
                maxCount = Mathf.Max(maxCount, prefixCount + count);
            }
            return maxCount == blueCount;
        }

        public bool IsSourceVisible()
        {
            if (kind == Kind.Surround || kind == Kind.Flower)
            {
                var coord = Coordinate.FromGameObject(sourceGO);
                return !MapManager.IsHidden(coord);
            }
            return true;
        }

        public void MarkSourceAsComplete()
        {
            if (kind == Kind.Flower)
            {
                var flower = sourceGO.GetComponent<BlueHexFlower>();
                if (flower != null && !flower.playerHasMarkedComplete)
                {
                    UndoManager.Instance.AddAction(new MarkFlowerAsCompleteUndoAction(flower));
                    flower.ToggleMarkComplete();
                }
            }
            else if (kind == Kind.Column)
            {
                var column = sourceGO.GetComponent<ColumnNumber>();
                if (column != null && !column.playerHasMarkedComplete)
                {
                    UndoManager.Instance.AddAction(new MarkColumnAsCompleteUndoAction(column));
                    column.ToggleMarkComplete();
                }
            }
        }
    }
}
