using UnityEngine;
using UnityEngine.SceneManagement;

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
            EventManager.HighlightClicked += (_) => UpdateHexNumbers();
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= UpdateHexNumbers;
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

        public void UpdateHexNumbers()
        {
            for (int x = 0; x < CoordUtil.Width; x++)
            {
                for (int y = 0; y < CoordUtil.Height; y++)
                {
                    var cell = MapManager.grid[x, y];
                    if (cell == null) continue;

                    Coordinate[] coords;
                    if (cell.tag == "Blue")
                    {
                        // Skip non-flower blue hexes
                        if (cell.name != "Blue Hex (Flower)") continue;
                        // Use flower coordinates for blue hexes
                        coords = CoordUtil.flowerCoords;
                    }
                    else
                    {
                        // Skip blank black hexes
                        if (cell.tag.StartsWith("Clue Hex Blank")) continue;
                        // Use surrounding coordinates for clue hexes
                        coords = CoordUtil.surroundCoords;
                    }

                    // Count the number of blue hexes in the surrounding coordinates
                    int blueCount = 0;
                    foreach (var coord in coords)
                    {
                        int xi = x + coord.x;
                        int yi = y + coord.y;
                        if (CoordUtil.IsValidCoord(xi, yi)
                            && MapManager.grid[xi, yi]?.tag == "Blue"
                            && (!countRemainingOnly || MapManager.Hidden(xi, yi)))
                        {
                            blueCount++;
                        }
                    }

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
            }

            var columnParent = GameObject.Find("Columns Parent");
            if (columnParent != null)
            {
                foreach (Transform tr in columnParent.transform)
                {
                    var coordinate = CoordUtil.WorldToGrid(tr.position);
                    var xi = coordinate.x;
                    var yi = coordinate.y;
                    switch (tr.name)
                    {
                        case "Hello":
                        default:
                            break;
                    }

                    Coordinate direction = tr.name switch
                    {
                        "Column Number Diagonal Left" => CoordUtil.diagonalLeftCoord,
                        "Column Number Diagonal Right" => CoordUtil.diagonalRightCoord,
                        _ => CoordUtil.verticalCoord,
                    };

                    int blueCount = 0;
                    while (true)
                    {
                        xi += direction.x;
                        yi += direction.y;
                        if (!CoordUtil.IsValidCoord(xi, yi)) break;

                        if (MapManager.grid[xi, yi]?.tag == "Blue" && (!countRemainingOnly || MapManager.Hidden(xi, yi)))
                        {
                            blueCount++;
                        }
                    }

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
}
