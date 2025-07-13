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
            char[,] grid = new char[CoordUtil.Width * 2, CoordUtil.Height];
            for (int x = 0; x < CoordUtil.Width * 2; x++)
            {
                for (int y = 0; y < CoordUtil.Height; y++)
                {
                    grid[x, y] = '.';
                }
            }

            for (int x = 0; x < CoordUtil.Width; x++)
            {
                for (int y = 0; y < CoordUtil.Height; y++)
                {
                    var tr = MapManager.grid[x, y];
                    if (tr == null)
                    {
                        continue;
                    }

                    char kind, info;
                    if (tr.tag == "Blue")
                    {
                        kind = 'x';
                        info = tr.name switch
                        {
                            "Blue Hex (Flower)" => '+',
                            _ => '.'
                        };
                    }
                    else
                    {
                        kind = 'o';
                        info = tr.tag switch
                        {
                            "Clue Hex Blank" => '.',
                            "Clue Hex (Sequential)" => 'c',
                            "Clue Hex (NOT Sequential)" => 'n',
                            _ => '+',
                        };
                    }

                    if (!MapManager.Hidden(x, y))
                    {
                        kind = char.ToUpper(kind);
                    }

                    grid[x * 2, y] = kind;
                    grid[x * 2 + 1, y] = info;
                }
            }

            foreach (Transform tr in GameObject.Find("Columns Parent").transform)
            {
                var coordinate = CoordUtil.WorldToGrid(tr.position);
                if (!CoordUtil.IsValidCoord(coordinate))
                {
                    continue;
                }

                char kind = tr.name switch
                {
                    "Column Number Diagonal Right" => '\\',
                    "Column Number Diagonal Left" => '/',
                    _ => '|',
                };
                char info = tr.tag switch
                {
                    "Column Sequential" => 'c',
                    "Column NOT Sequential" => 'n',
                    _ => '+'
                };
                var x = coordinate.x;
                var y = coordinate.y;
                grid[x * 2, y] = kind;
                grid[x * 2 + 1, y] = info;
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
                {
                    sb.Append(grid[x, y]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
