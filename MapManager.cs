using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace HexcellsHelper
{

    public static class MapManager
    {
        public static GameObject[,] grid;
        public static GameObject[,] gridOverlay;
        public static MusicDirector musicDirector;
        static GameObject hexGrid;
        static GameObject hexGridOverlay;
        static EditorFunctions editorFunctions;
        static HexScoring score;
        static TextMesh remainingText;

        public static void Init()
        {
            EventManager.LevelLoaded += InitializeMap;
            EventManager.DestroyClicked += UpdateMap;
            EventManager.HighlightClicked += UpdateMap;
        }

        public static bool IsHidden(int x, int y)
        {
            return gridOverlay[x, y] != null;
        }
        public static bool IsHidden(Coordinate coord)
        {
            return IsHidden(coord.X, coord.Y);
        }

        public static GameObject GridAt(Coordinate coord)
        {
            return grid[coord.X, coord.Y];
        }

        public static GameObject GridOverlayAt(Coordinate coord)
        {
            return gridOverlay[coord.X, coord.Y];
        }

        public static void SetBlack(int x, int y)
        {
            // No need to update gridOverlay.
            // Because UpdateMap is registered when DestroyClick is called.
            gridOverlay[x, y]?.transform.GetComponent<HexBehaviour>().DestroyClick();
        }

        public static void SetBlack(Coordinate coord)
        {
            SetBlack(coord.X, coord.Y);
        }

        public static void SetBlue(int x, int y)
        {
            // No need to update gridOverlay.
            // Because UpdateMap is registered when HighlightClick is called.
            gridOverlay[x, y]?.transform.GetComponent<HexBehaviour>().HighlightClick();
        }

        public static void SetBlue(Coordinate coord)
        {
            SetBlue(coord.X, coord.Y);
        }

        public static bool SetYellow(int x, int y)
        {
            if (grid[x, y] == null || gridOverlay[x, y] != null)
            {
                return false;
            }
            gridOverlay[x, y] = Object.Instantiate(
                editorFunctions.orangeHex,
                grid[x, y].transform.position,
                editorFunctions.orangeHex.transform.rotation,
                hexGridOverlay.transform
            );
            if (grid[x, y].tag == "Blue")
            {
                score.numberOfCorrectTilesFound--;
                gridOverlay[x, y].GetComponent<HexBehaviour>().containsShapeBlock = true;
                remainingText.text = (score.numberOfBlueTiles - score.numberOfCorrectTilesFound).ToString();
                BepInEx.Bootstrap.Chainloader.ManagerObject
                    .GetComponent<DisplayModeManager>()
                    .UpdateHexNumbers();
            }
            score.tilesRemoved--;
            musicDirector.PlayWrongNote(gridOverlay[x, y].transform.position.x / 7.04f);
            iTween.ShakePosition(gridOverlay[x, y], new Vector3(0.1f, 0.1f, 0f), 0.3f);
            return true;
        }

        public static bool SetYellow(Coordinate coord)
        {
            return SetYellow(coord.X, coord.Y);
        }

        static void InitializeMap()
        {
            hexGrid = GameObject.Find("Hex Grid");
            hexGridOverlay = GameObject.Find("Hex Grid Overlay");
            editorFunctions = GameObject.Find("Editor Functions").GetComponent<EditorFunctions>();
            score = GameObject.Find("Score Text").GetComponent<HexScoring>();
            remainingText = score.GetComponent<TextMesh>();
            musicDirector = GameObject.Find("Music Director(Clone)").GetComponent<MusicDirector>();

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
        }

        static void UpdateMap(HexBehaviour hexBehaviour)
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
