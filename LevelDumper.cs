using UnityEngine;
using System;
using System.Text;

namespace HexcellsHelper
{
    public class LevelDumper : MonoBehaviour
    {
        const string Header = "Hexcells level v1";

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                string levelText = SerializeCurrentLevel();
                if (string.IsNullOrEmpty(levelText))
                {
                    Debug.LogError("[LevelDumper] Failed to serialize the current level.");
                    GameObject.Find("Music Director(Clone)").GetComponent<MusicDirector>().PlayWrongNote(0.0f);
                    return;
                }

                GUIUtility.systemCopyBuffer = levelText;
                Debug.Log($"[LevelDumper] Level text copied to clipboard");

                // Play a sound to indicate success
                GameObject.Find("Music Director(Clone)").GetComponent<MusicDirector>().PlayNoteB(0.0f);
            }
        }

        string SerializeCurrentLevel()
        {
            var hexGrid = GameObject.Find("Hex Grid");
            var hexGridOverlay = GameObject.Find("Hex Grid Overlay");
            if (hexGrid == null || hexGridOverlay == null)
            {
                Debug.LogError("[LevelDumper] Hex Grid or Hex Grid Overlay not found!");
                return null;
            }

            char[,] grid = new char[CoordUtil.Height, CoordUtil.Width * 2];
            for (int y = 0; y < CoordUtil.Height; y++)
                for (int x = 0; x < CoordUtil.Width * 2; x++)
                    grid[y, x] = '.';

            bool[,] hiddenHexes = new bool[CoordUtil.Height, CoordUtil.Width];
            foreach (Transform tr in hexGridOverlay.transform)
            {
                int xi = CoordUtil.WorldToGridX(tr.position.x);
                int yi = CoordUtil.WorldToGridY(tr.position.y);
                if (!CoordUtil.IsValidCoord(xi, yi)) continue;
                hiddenHexes[yi, xi] = true;
            }

            foreach (Transform tr in hexGrid.transform)
            {
                int xi = CoordUtil.WorldToGridX(tr.position.x);
                int yi = CoordUtil.WorldToGridY(tr.position.y);
                if (!CoordUtil.IsValidCoord(xi, yi)) continue;

                char kind, info;

                if (tr.tag == "Blue")
                {
                    kind = 'x';
                    info = tr.name == "Blue Hex (Flower)" ? '+' : '.';
                }
                else
                {
                    kind = 'o';
                    switch (tr.tag)
                    {
                        case "Clue Hex Blank":
                            info = '.';
                            break;
                        case "Clue Hex (Sequential)":
                            info = 'c';
                            break;
                        case "Clue Hex (NOT Sequential)":
                            info = 'n';
                            break;
                        default:
                            info = '+';
                            break;
                    }
                }

                if (!hiddenHexes[yi, xi]) kind = char.ToUpper(kind);

                grid[yi, xi * 2] = kind;
                grid[yi, xi * 2 + 1] = info;
            }

            foreach (Transform tr in GameObject.Find("Columns Parent").transform)
            {
                int xi = CoordUtil.WorldToGridX(tr.position.x);
                int yi = CoordUtil.WorldToGridY(tr.position.y);
                if (!CoordUtil.IsValidCoord(xi, yi)) continue;

                char kind = '|', info = '+';
                switch (tr.name)
                {
                    case "Column Number Diagonal Right":
                        kind = '\\';
                        break;
                    case "Column Number Diagonal Left":
                        kind = '/';
                        break;
                }
                if (tr.tag == "Column Sequential") info = 'c';
                else if (tr.tag == "Column NOT Sequential") info = 'n';

                grid[yi, xi * 2] = kind;
                grid[yi, xi * 2 + 1] = info;
            }

            var sb = new StringBuilder();
            string seed = GameObject.Find("Game Manager(Clone)").GetComponent<GameManagerScript>().seedNumber;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sb.AppendLine(Header);
            sb.AppendLine($"Seed {seed}");  // title
            sb.AppendLine($"Hexcells Infinite (dumped at {timestamp})");  // author
            sb.AppendLine("");
            sb.AppendLine("");

            for (int y = CoordUtil.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < CoordUtil.Width * 2; x++)
                    sb.Append(grid[y, x]);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
