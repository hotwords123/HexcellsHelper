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
                        return true;
                    }
                }
                else
                {
                    // now it must be a black cell
                    if (cell.tag != "Clue Hex Blank" && TrySolveBlack(cell))
                    {
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
            foreach (var otherCoord in coords)
            {
                if (!MapManager.IsHidden(otherCoord))
                {
                    continue;
                }
                if (MapManager.GridAt(otherCoord)?.tag == "Blue")
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
                foreach (var otherCoord in coords)
                {
                    if (MapManager.IsHidden(otherCoord))
                    {
                        MapManager.SetBlue(otherCoord);
                    }
                }
                return true;
            }
            else if (hiddenBlue == 0)
            {
                // all hidden cells are black, we can solve it
                foreach (var otherCoord in coords)
                {
                    if (MapManager.IsHidden(otherCoord))
                    {
                        MapManager.SetBlack(otherCoord);
                    }
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
            if (!cell.tag.StartsWith("Clue Hex ("))
            {
                return false;
            }

            if (cell.tag.StartsWith("Clue Hex (Sequential)"))
            {

            }
            else if (cell.tag.StartsWith("Clue Hex (NOT Sequential)"))
            {

            }
            // otherwise, we cannot solve it
            return false;
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
            if (tr.tag == "Column Number")
            {
                return false;
            }

            if (tr.tag == "Column Sequential")
            {

            }
            else if (tr.tag == "Column NOT Sequential")
            {

            }
            // otherwise, we cannot solve it
            return false;
        }
    }
}
