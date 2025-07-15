using UnityEngine;
using System.Linq;

namespace HexcellsHelper
{

    public static class MapManager
    {
        static GameObject[,] grid;
        static GameObject[,] gridOverlay;
        static GameObject[] columnNumbers;
        static GameObject hexGrid;
        static GameObject hexGridOverlay;
        static GameObject columnsParent;
        static EditorFunctions editorFunctions;
        static HexScoring score;
        static TextMesh remainingText;

        public static GameObject[] Columns => columnNumbers;

        public static void Init()
        {
            EventManager.LevelLoaded += OnLevelLoaded;
            EventManager.LevelUnloaded += OnLevelUnloaded;
            EventManager.DestroyClicked += OnHexRevealed;
            EventManager.HighlightClicked += OnHexRevealed;
        }

        public static bool IsNonEmpty(Coordinate coord)
        {
            return GridAt(coord) != null;
        }

        public static bool IsHidden(Coordinate coord)
        {
            return GridOverlayAt(coord) != null;
        }

        public static GameObject GridAt(Coordinate coord)
        {
            if (!CoordUtil.IsValidCoord(coord))
            {
                return null;
            }
            return grid[coord.X, coord.Y];
        }

        public static GameObject GridOverlayAt(Coordinate coord)
        {
            if (!CoordUtil.IsValidCoord(coord))
            {
                return null;
            }
            return gridOverlay[coord.X, coord.Y];
        }

        public static void SetBlack(Coordinate coord)
        {
            // No need to update gridOverlay.
            // Because UpdateMap is registered when DestroyClick is called.
            GridOverlayAt(coord)?.GetComponent<HexBehaviour>().DestroyClick();
        }

        public static void SetBlue(Coordinate coord)
        {
            // No need to update gridOverlay.
            // Because UpdateMap is registered when HighlightClick is called.
            GridOverlayAt(coord)?.GetComponent<HexBehaviour>().HighlightClick();
        }

        public static bool SetHidden(Coordinate coord)
        {
            var cell = GridAt(coord);
            var overlayCell = GridOverlayAt(coord);
            if (cell == null || overlayCell != null)
            {
                return false;
            }

            overlayCell = Object.Instantiate(
                editorFunctions.orangeHex,
                cell.transform.position,
                editorFunctions.orangeHex.transform.rotation,
                hexGridOverlay.transform
            );
            gridOverlay[coord.X, coord.Y] = overlayCell;

            if (cell.tag == "Blue")
            {
                score.numberOfCorrectTilesFound--;
                overlayCell.GetComponent<HexBehaviour>().containsShapeBlock = true;
                remainingText.text = (score.numberOfBlueTiles - score.numberOfCorrectTilesFound).ToString();
                BepInEx.Bootstrap.Chainloader.ManagerObject
                    .GetComponent<DisplayModeManager>()
                    .UpdateHexNumbers();
            }

            score.tilesRemoved--;
            GameObjectUtil.GetMusicDirector().PlayWrongNote(overlayCell.transform.position.x / 7.04f);
            iTween.ShakePosition(overlayCell, new Vector3(0.1f, 0.1f, 0f), 0.3f);
            return true;
        }

        static void OnLevelLoaded()
        {
            hexGrid = GameObjectUtil.GetHexGrid();
            hexGridOverlay = GameObjectUtil.GetHexGridOverlay();
            columnsParent = GameObject.Find("Columns Parent");
            editorFunctions = GameObject.Find("Editor Functions").GetComponent<EditorFunctions>();
            score = GameObject.Find("Score Text").GetComponent<HexScoring>();
            remainingText = score.GetComponent<TextMesh>();

            grid = new GameObject[CoordUtil.Width, CoordUtil.Height];
            foreach (Transform tr in hexGrid.transform)
            {
                var coord = CoordUtil.WorldToGrid(tr.position);
                if (!CoordUtil.IsValidCoord(coord))
                {
                    continue;
                }
                grid[coord.X, coord.Y] = tr.gameObject;
            }

            gridOverlay = new GameObject[CoordUtil.Width, CoordUtil.Height];
            foreach (Transform tr in hexGridOverlay.transform)
            {
                var coord = CoordUtil.WorldToGrid(tr.position);
                if (!CoordUtil.IsValidCoord(coord))
                {
                    continue;
                }
                gridOverlay[coord.X, coord.Y] = tr.gameObject;
            }

            columnNumbers = columnsParent.transform
                .Cast<Transform>()
                .Select(tr => tr.gameObject)
                .ToArray();
        }

        static void OnLevelUnloaded()
        {
            // Clear GameObject references
            grid = null;
            gridOverlay = null;
            columnNumbers = null;
            hexGrid = null;
            hexGridOverlay = null;
            columnsParent = null;
            editorFunctions = null;
            score = null;
            remainingText = null;
        }

        static void OnHexRevealed(HexBehaviour hexBehaviour)
        {
            if (hexBehaviour == null)
            {
                return;
            }
            var position = hexBehaviour.transform.position;
            var coord = CoordUtil.WorldToGrid(position);
            gridOverlay[coord.X, coord.Y] = null;
        }
    }
}
