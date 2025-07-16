using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public class DisplayModeManager : MonoBehaviour
    {
        public static DisplayModeManager Instance { get; private set; }

        bool countRemainingOnly = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        void Update()
        {
            if (!EventManager.IsLevelLoaded)
            {
                return;
            }

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
            EventManager.LevelLoaded += OnLevelLoaded;
            EventManager.HighlightClicked += OnHighlightClicked;
        }

        void OnDisable()
        {
            EventManager.LevelLoaded -= OnLevelLoaded;
            EventManager.HighlightClicked -= OnHighlightClicked;
        }

        void ToggleDisplayMode()
        {
            countRemainingOnly = !countRemainingOnly;
            UpdateHexNumbers();

            if (countRemainingOnly)
            {
                GameObjectUtil.GetMusicDirector().PlayNoteA(0.0f);
            }
            else
            {
                GameObjectUtil.GetMusicDirector().PlayNoteB(0.0f);
            }
        }

        void OnLevelLoaded()
        {
            UpdateHexNumbers();
        }

        void OnHighlightClicked(HexBehaviour hexBehaviour)
        {
            // Update hex numbers when a hex is highlighted
            UpdateHexNumbers();
        }

        int CalculateNumberForCoords(IEnumerable<Coordinate> coords)
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
                int blueCount = CalculateNumberForCoords(otherCoords);

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

            foreach (var column in MapManager.Columns)
            {
                var coord = CoordUtil.WorldToGrid(column.transform.position);

                IEnumerable<Coordinate> otherCoords = column.name switch
                {
                    "Column Number Diagonal Left" => CoordUtil.DiagonalLeftCoords(coord),
                    "Column Number Diagonal Right" => CoordUtil.DiagonalRightCoords(coord),
                    _ => CoordUtil.VerticalCoords(coord),
                };

                int blueCount = CalculateNumberForCoords(otherCoords);

                string text = blueCount.ToString();
                if (column.tag == "Column Sequential")
                {
                    text = "{" + text + "}";
                }
                else if (column.tag == "Column NOT Sequential")
                {
                    text = "-" + text + "-";
                }
                column.GetComponent<TextMesh>().text = text;
            }
        }
    }
}
