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
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("[DisplayModeManager] DisplayModeManager enabled. Listening for scene loads.");
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Debug.Log("[DisplayModeManager] DisplayModeManager disabled. Stopped listening for scene loads.");
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateHexNumbers();
        }

        void ToggleDisplayMode()
        {
            countRemainingOnly = !countRemainingOnly;
            UpdateHexNumbers();

            var musicDirector = GameObject.Find("Music Director(Clone)").GetComponent<MusicDirector>();
            if (countRemainingOnly)
            {
                musicDirector.PlayNoteA(0.0f);
            }
            else
            {
                musicDirector.PlayNoteB(0.0f);
            }
        }

        public void UpdateHexNumbers()
        {
            var hexGrid = GameObject.Find("Hex Grid");
            var hexGridOverlay = GameObject.Find("Hex Grid Overlay");
            if (hexGrid == null || hexGridOverlay == null)
            {
                return;
            }

            GameObject[,] grid = new GameObject[CoordUtil.Height, CoordUtil.Width];
            foreach (Transform tr in hexGrid.transform)
            {
                int xi = CoordUtil.WorldToGridX(tr.position.x);
                int yi = CoordUtil.WorldToGridY(tr.position.y);
                if (!CoordUtil.IsValidCoord(xi, yi)) continue;
                grid[yi, xi] = tr.gameObject;
            }

            bool[,] hiddenHexes = new bool[CoordUtil.Height, CoordUtil.Width];
            foreach (Transform tr in hexGridOverlay.transform)
            {
                int xi = CoordUtil.WorldToGridX(tr.position.x);
                int yi = CoordUtil.WorldToGridY(tr.position.y);
                if (!CoordUtil.IsValidCoord(xi, yi)) continue;

                // The overlay hex still exists during the reveal animation, and only
                // gets destroyed after the animation ends. The MeshCollider is disabled
                // at the start of the animation, so we can use it to check if the hex
                // is actually revealed.
                if (tr.GetComponent<MeshCollider>().enabled)
                {
                    hiddenHexes[yi, xi] = true;
                }
            }

            for (int x = 0; x < CoordUtil.Width; x++)
            {
                for (int y = 0; y < CoordUtil.Height; y++)
                {
                    var cell = grid[y, x];
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
                            && grid[yi, xi]?.tag == "Blue"
                            && (!countRemainingOnly || hiddenHexes[yi, xi]))
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
                    int xi = CoordUtil.WorldToGridX(tr.position.x);
                    int yi = CoordUtil.WorldToGridY(tr.position.y);
                    if (!CoordUtil.IsValidCoord(xi, yi)) continue;

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

                        if (grid[yi, xi]?.tag == "Blue" && (!countRemainingOnly || hiddenHexes[yi, xi]))
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
