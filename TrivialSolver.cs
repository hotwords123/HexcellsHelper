using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class TrivialSolver : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!SolveTrivial())
                {
                    GameObjectUtil.GetMusicDirector().PlayWrongNote(0.0f);
                }
            }
        }

        bool SolveTrivial()
        {
            foreach (var coord in CoordUtil.AllCoords())
            {
                // can only solve if the cell is not hidden
                if (MapManager.IsHidden(coord))
                {
                    continue;
                }

                var cell = MapManager.GridAt(coord);
                if (cell == null)
                {
                    continue;
                }

                if (cell.tag == "Blue")
                {
                    if (cell.name == "Blue Hex (Flower)" && TrySolveFlower(cell))
                    {
                        Debug.Log("Solved flower at " + coord);
                        iTween.ShakePosition(cell, new Vector3(0.1f, 0.1f, 0f), 0.3f);
                        return true;
                    }
                }
                else
                {
                    // now it must be a black cell
                    if (cell.tag != "Clue Hex Blank" && TrySolveBlack(cell))
                    {
                        Debug.Log("Solved black cell at " + coord);
                        iTween.ShakePosition(cell, new Vector3(0.1f, 0.1f, 0f), 0.3f);
                        return true;
                    }
                }
            }

            // now try to solve columns
            if (MapManager.ColumnsParent == null)
            {
                return false;
            }
            foreach (Transform tr in MapManager.ColumnsParent.transform)
            {
                if (TrySolveColumn(tr))
                {
                    Debug.Log("Solved column at " + CoordUtil.WorldToGrid(tr.position));
                    iTween.ShakePosition(tr.gameObject, new Vector3(0.1f, 0.1f, 0f), 0.3f);
                    return true;
                }
            }

            // now try to solve using the remaining cells
            var remainingCells = CoordUtil.AllCoords().Where(MapManager.IsHidden);
            return TrySolveReallyTrivial(remainingCells, out int hiddenBlack, out int hiddenBlue);
        }

        bool TrySolveReallyTrivial(IEnumerable<Coordinate> coords, out int hiddenBlack, out int hiddenBlue)
        {
            hiddenBlack = 0;
            hiddenBlue = 0;

            var hiddenCoords = coords.Where(MapManager.IsHidden).ToArray();
            foreach (var otherCoord in hiddenCoords)
            {
                if (MapManager.GridAt(otherCoord).tag == "Blue")
                {
                    hiddenBlue++;
                }
                else
                {
                    hiddenBlack++;
                }
            }

            if (hiddenBlue == 0 && hiddenBlack == 0)
            {
                // all cells are revealed, nothing to solve
                return false;
            }
            else if (hiddenBlack == 0)
            {
                // all hidden cells are blue, we can solve it
                foreach (var otherCoord in hiddenCoords)
                {
                    MapManager.SetBlue(otherCoord);
                }
                return true;
            }
            else if (hiddenBlue == 0)
            {
                // all hidden cells are black, we can solve it
                foreach (var otherCoord in coords)
                {
                    MapManager.SetBlack(otherCoord);
                }
                return true;
            }
            // otherwise, we cannot solve it really trivially
            return false;
        }

        bool TrySolveBlack(GameObject cell)
        {
            // if the hidden cells are all black or all blue, we can solve it
            var coord = CoordUtil.WorldToGrid(cell.transform.position);
            var otherCoords = CoordUtil.SurroundCoords(coord);
            if (TrySolveReallyTrivial(otherCoords, out int hiddenBlack, out int hiddenBlue))
            {
                return true;
            }

            // try use sequential and NOT sequential if provided
            bool consecutive;
            if (cell.tag.StartsWith("Clue Hex (Sequential)"))
            {
                consecutive = true;
            }
            else if (cell.tag.StartsWith("Clue Hex (NOT Sequential)"))
            {
                consecutive = false;
            }
            else
            {
                // not a sequential or NOT sequential clue, cannot solve it
                return false;
            }

            return TrySolveWithConstraint(otherCoords, consecutive, hiddenBlack, hiddenBlue, true);
        }

        bool TrySolveFlower(GameObject cell)
        {
            var coord = CoordUtil.WorldToGrid(cell.transform.position);
            var otherCoords = CoordUtil.FlowerCoords(coord);
            // no non-really-trivial solving for flower
            var reallyTrivialSuccess = TrySolveReallyTrivial(otherCoords, out int hiddenBlack, out int hiddenBlue);
            if (hiddenBlack == 0 || hiddenBlue == 0)
            {
                // mark the flower as complete
                var flower = cell.GetComponent<BlueHexFlower>();
                if (!flower.playerHasMarkedComplete)
                {
                    flower.ToggleMarkComplete();
                }
            }
            return reallyTrivialSuccess;
        }

        bool TrySolveColumn(Transform tr)
        {
            var coord = CoordUtil.WorldToGrid(tr.position);
            IEnumerable<Coordinate> otherCoords = tr.name switch
            {
                "Column Number Diagonal Left" => CoordUtil.DiagonalLeftCoords(coord),
                "Column Number Diagonal Right" => CoordUtil.DiagonalRightCoords(coord),
                _ => CoordUtil.VerticalCoords(coord),
            };
            var reallyTrivialSuccess = TrySolveReallyTrivial(otherCoords, out int hiddenBlack, out int hiddenBlue);
            if (hiddenBlack == 0 || hiddenBlue == 0)
            {
                // mark the column as complete
                var column = tr.GetComponent<ColumnNumber>();
                if (!column.playerHasMarkedComplete)
                {
                    column.ToggleMarkComplete();
                }
            }
            if (reallyTrivialSuccess)
            {
                return true;
            }

            // try use sequential and NOT sequential if provided
            bool consecutive;
            if (tr.tag == "Column Sequential")
            {
                consecutive = true;
            }
            else if (tr.tag == "Column NOT Sequential")
            {
                consecutive = false;
            }
            else
            {
                // not a sequential or NOT sequential column, cannot solve it
                return false;
            }

            var nonEmptyCoords = otherCoords.Where(MapManager.IsNonEmpty);
            return TrySolveWithConstraint(nonEmptyCoords, consecutive, hiddenBlack, hiddenBlue, false);
        }

        bool TrySolveWithConstraint(IEnumerable<Coordinate> coords, bool consecutive, int blackCount, int blueCount, bool isCircular)
        {
            var coordsArray = coords.ToArray();
            var isBlue = new bool[coordsArray.Length];
            var canBeBlack = new bool[coordsArray.Length];
            var canBeBlue = new bool[coordsArray.Length];

            int CountConsecutiveBlues()
            {
                int count = 0, maxCount = 0, prefixCount = 0;
                for (int i = 0; i < isBlue.Length; i++)
                {
                    if (isBlue[i])
                    {
                        if (prefixCount == i)
                        {
                            prefixCount++;
                        }
                        count++;
                        maxCount = Mathf.Max(maxCount, count);
                    }
                    else
                    {
                        count = 0;
                    }
                }
                if (isCircular && (count != isBlue.Length))
                {
                    maxCount = Mathf.Max(maxCount, prefixCount + count);
                }
                return maxCount;
            }

            void Dfs(int index, int remainingBlacks, int remainingBlues)
            {
                if (index == coordsArray.Length)
                {
                    // check if the constraints are satisfied
                    int consecutiveBlues = CountConsecutiveBlues();
                    int totalBlues = isBlue.Count(b => b);
                    if (consecutive == (consecutiveBlues == totalBlues))
                    {
                        // found a valid configuration
                        for (int i = 0; i < coordsArray.Length; i++)
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

                var coord = coordsArray[index];
                if (MapManager.IsNonEmpty(coord))
                {
                    // skip already revealed cells
                    if (!MapManager.IsHidden(coord))
                    {
                        isBlue[index] = MapManager.GridAt(coord).tag == "Blue";
                        Dfs(index + 1, remainingBlacks, remainingBlues);
                        return;
                    }
                }
                else
                {
                    // skip empty cells
                    Dfs(index + 1, remainingBlacks, remainingBlues);
                    return;
                }

                // try placing a black cell
                if (remainingBlacks > 0)
                {
                    isBlue[index] = false;
                    Dfs(index + 1, remainingBlacks - 1, remainingBlues);
                }

                // try placing a blue cell
                if (remainingBlues > 0)
                {
                    isBlue[index] = true;
                    Dfs(index + 1, remainingBlacks, remainingBlues - 1);
                }
            }

            Dfs(0, blackCount, blueCount);

            bool success = false;
            for (int i = 0; i < coordsArray.Length; i++)
            {
                var coord = coordsArray[i];
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
    }
}
