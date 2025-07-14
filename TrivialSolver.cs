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
                    MapManager.musicDirector.PlayWrongNote(0.0f);
                }
            }
        }

        bool SolveTrivial()
        {
            foreach (var coord in CoordUtil.IterGrid())
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
                    if (cell.name != "Blue Hex (Flower)")
                    {
                        continue;
                    }
                    if (TrySolveFlower(cell))
                    {
                        return true;
                    }
                }

                // now it must be a black cell
                if (cell.name == "Clue Hex Blank")
                {
                    continue;
                }
                if (TrySolveBlack(cell))
                {
                    return true;
                }

                // now try to solve columns
                var columnParent = GameObject.Find("Columns Parent");
                if (columnParent == null)
                {
                    return false;
                }

                foreach (Transform tr in columnParent.transform)
                {
                    if (TrySolveColumn(tr))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool TrySolveBlack(GameObject cell)
        {

            return false;
        }

        bool TrySolveFlower(GameObject cell)
        {
            var coord = CoordUtil.WorldToGrid(cell.transform.position);
            var otherCoords = CoordUtil.FlowerCoords(coord);
            int hiddenBlack = 0;
            int hiddenBlue = 0;
            // 
            foreach (var othercoord in otherCoords)
            {
                if (!CoordUtil.IsValidCoord(othercoord) || !MapManager.IsHidden(othercoord))
                {
                    continue;
                }
                if (MapManager.GridAt(othercoord)?.tag == "Blue")
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
                foreach (var othercoord in otherCoords)
                {
                    if (MapManager.IsHidden(othercoord))
                    {
                        MapManager.SetBlue(othercoord);
                    }
                }
                return true;
            }
            else if (hiddenBlue == 0)
            {
                // all hidden cells are black, we can solve it
                foreach (var othercoord in otherCoords)
                {
                    if (MapManager.IsHidden(othercoord))
                    {
                        MapManager.SetBlack(othercoord);
                    }
                }
                return true;
            }
            // otherwise, we cannot solve it
            return false;
        }

        bool TrySolveColumn(Transform tr)
        {
            return false;
        }
    }
}
