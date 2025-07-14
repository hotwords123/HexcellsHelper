using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class DisplayModeManager : MonoBehaviour
    {
        bool countRemainingOnly = false;

        void Update()
        {
            // Toggle display mode when T is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleDisplayMode();
            }

            // Toggle display mode when Tab is pressed or released
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyUp(KeyCode.Tab))
            {
                ToggleDisplayMode();
            }
        }

        void OnEnable()
        {
            EventManager.LevelLoaded += UpdateHexNumbers;
            EventManager.HighlightClicked += UpdateHexNumbers;
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= UpdateHexNumbers;
            EventManager.HighlightClicked -= UpdateHexNumbers;
        }

        void ToggleDisplayMode()
        {
            countRemainingOnly = !countRemainingOnly;
            UpdateHexNumbers();

            if (countRemainingOnly)
            {
                MapManager.musicDirector.PlayNoteA(0.0f);
            }
            else
            {
                MapManager.musicDirector.PlayNoteB(0.0f);
            }
        }

        void UpdateHexNumbers(HexBehaviour hexBehaviour)
        {
            // Update hex numbers based on the current display mode
            UpdateHexNumbers();
        }

        int CountBlueHexesInCoords(IEnumerable<Coordinate> coords)
        {
            return coords.Count(coord =>
                MapManager.GridAt(coord)?.tag == "Blue" &&
                (!countRemainingOnly || MapManager.IsHidden(coord))
            );
        }

        public void UpdateHexNumbers()
        {
            foreach (var coord in CoordUtil.AllCoords())
            {
                var cell = MapManager.GridAt(coord);
                if (cell == null)
                {
                    continue;
                }

                IEnumerable<Coordinate> otherCoords;
                if (cell.tag == "Blue")
                {
                    // Skip non-flower blue hexes
                    if (cell.name != "Blue Hex (Flower)") continue;
                    // Use flower coordinates for blue hexes
                    otherCoords = CoordUtil.FlowerCoords(coord);
                }
                else
                {
                    // Skip blank black hexes
                    if (cell.tag.StartsWith("Clue Hex Blank")) continue;
                    // Use surrounding coordinates for clue hexes
                    otherCoords = CoordUtil.SurroundCoords(coord);
                }

                // Count the number of blue hexes in the surrounding coordinates
                int blueCount = CountBlueHexesInCoords(otherCoords);

                // Update the hex number text
                string text = blueCount.ToString();
                if (cell.tag.StartsWith("Clue Hex (Sequential)"))
                {
                    text = "{" + text + "}";
                }
                else if (cell.tag.StartsWith("Clue Hex (NOT Sequential)"))
                {
                    text = "-" + text + "-";
                }
                cell.transform.Find("Hex Number").GetComponent<TextMesh>().text = text;
            }


            var columnParent = GameObject.Find("Columns Parent");
            if (columnParent == null)
            {
                return;
            }
            foreach (Transform tr in columnParent.transform)
            {
                var coord = CoordUtil.WorldToGrid(tr.position);

                IEnumerable<Coordinate> otherCoords = tr.name switch
                {
                    "Column Number Diagonal Left" => CoordUtil.DiagonalLeftCoords(coord),
                    "Column Number Diagonal Right" => CoordUtil.DiagonalRightCoords(coord),
                    _ => CoordUtil.VerticalCoords(coord),
                };

                int blueCount = CountBlueHexesInCoords(otherCoords);

                string text = blueCount.ToString();
                if (tr.tag == "Column Sequential")
                {
                    text = "{" + text + "}";
                }
                else if (tr.tag == "Column NOT Sequential")
                {
                    text = "-" + text + "-";
                }
                tr.GetComponent<TextMesh>().text = text;
            }
        }
    }
}
