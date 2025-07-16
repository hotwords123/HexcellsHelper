using UnityEngine;
using System.Collections.Generic;

namespace HexcellsHelper
{

    public static class MapManager
    {
        static GameObject[,] grid;
        static GameObject[,] gridOverlay;
        static GameObject hexGrid;
        static GameObject hexGridOverlay;
        static GameObject columnsParent;
        static EditorFunctions editorFunctions;
        static HexScoring score;
        static TextMesh remainingText;

        public static GameObject HexGrid => hexGrid;
        public static GameObject HexGridOverlay => hexGridOverlay;
        public static GameObject ColumnsParent => columnsParent;

        public static IEnumerable<GameObject> Hexes => GameObjectUtil.GetChildren(hexGrid);
        public static IEnumerable<GameObject> OverlayHexes => GameObjectUtil.GetChildren(hexGridOverlay);
        public static IEnumerable<GameObject> Columns => GameObjectUtil.GetChildren(columnsParent);

        public static void Init()
        {
            EventManager.LevelLoaded += OnLevelLoaded;
            EventManager.LevelUnloaded += OnLevelUnloaded;
            EventManager.DestroyClicked += OnHexRevealed;
            EventManager.HighlightClicked += OnHexRevealed;
        }

        public static bool IsCompleted => score != null && score.levelIsComplete;
        public static GameObject OrangeHex => editorFunctions?.orangeHex;

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

            // Restore the foreground hex at the saved position
            overlayCell = Object.Instantiate(
                OrangeHex,
                cell.transform.position,
                OrangeHex.transform.rotation,
                hexGridOverlay.transform
            );
            gridOverlay[coord.X, coord.Y] = overlayCell;

            HypothesisManager.Instance?.SetupHexHypothesis(overlayCell);

            if (cell.tag == "Blue")
            {
                // Undo score.CorrectTileFound()
                score.numberOfCorrectTilesFound--;
                overlayCell.GetComponent<HexBehaviour>().containsShapeBlock = true;
                remainingText.text = (score.numberOfBlueTiles - score.numberOfCorrectTilesFound).ToString();

                // Update the hex number text
                DisplayModeManager.Instance.UpdateHexNumbers();
            }

            // Undo score.TileRemoved()
            score.tilesRemoved--;

            // UX feedback
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
            foreach (var hex in Hexes)
            {
                var coord = CoordUtil.WorldToGrid(hex.transform.position);
                if (!CoordUtil.IsValidCoord(coord))
                {
                    continue;
                }
                grid[coord.X, coord.Y] = hex;
            }

            gridOverlay = new GameObject[CoordUtil.Width, CoordUtil.Height];
            foreach (var hex in OverlayHexes)
            {
                var coord = CoordUtil.WorldToGrid(hex.transform.position);
                if (!CoordUtil.IsValidCoord(coord))
                {
                    continue;
                }
                gridOverlay[coord.X, coord.Y] = hex;
            }
        }

        static void OnLevelUnloaded()
        {
            // Clear GameObject references
            grid = null;
            gridOverlay = null;
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
