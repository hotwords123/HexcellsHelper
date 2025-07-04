using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace HexcellsHelper
{
    public class LevelDumper : MonoBehaviour
    {
        const int W = 33, H = 33;
        const string Header = "Hexcells level v1";

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                string path = Path.Combine(
                    Application.persistentDataPath,
                    $"dump_{System.DateTime.Now:yyyyMMdd_HHmmss}.hexcells");

                string levelText = SerializeCurrentLevel();

                File.WriteAllText(path, levelText);
                Debug.Log($"[LevelDumper] Level text saved to: {path}");

                GUIUtility.systemCopyBuffer = levelText;
                Debug.Log($"[LevelDumper] Level text copied to clipboard");
            }
        }

        bool IsValidCoord(int x, int y)
        {
            return x >= 0 && x < W && y >= 0 && y < H;
        }

        string SerializeCurrentLevel()
        {
            char[,] grid = new char[H, W * 2];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W * 2; x++)
                    grid[y, x] = '.';

            bool[,] hiddenHexes = new bool[H, W];
            foreach (Transform tr in GameObject.Find("Hex Grid Overlay").transform)
            {
                int xi = Mathf.RoundToInt(tr.position.x / 0.88f) + 15;
                int yi = Mathf.RoundToInt(tr.position.y / 0.5f) + 15;
                if (!IsValidCoord(xi, yi)) continue;
                hiddenHexes[yi, xi] = true;
            }

            foreach (Transform tr in GameObject.Find("Hex Grid").transform)
            {
                int xi = Mathf.RoundToInt(tr.position.x / 0.88f) + 15;
                int yi = Mathf.RoundToInt(tr.position.y / 0.5f) + 15;
                if (!IsValidCoord(xi, yi)) continue;

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
                int xi = Mathf.RoundToInt(tr.position.x / 0.88f) + 15;
                int yi = Mathf.RoundToInt(tr.position.y / 0.5f) + 15;
                if (!IsValidCoord(xi, yi)) continue;

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

            for (int y = H - 1; y >= 0; y--)
            {
                for (int x = 0; x < W * 2; x++)
                    sb.Append(grid[y, x]);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
