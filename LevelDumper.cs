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
                    GameObjectUtil.GetMusicDirector().PlayWrongNote(0.0f);
                    return;
                }

                GUIUtility.systemCopyBuffer = levelText;
                Debug.Log($"[LevelDumper] Level text copied to clipboard");

                // Play a sound to indicate success
                GameObjectUtil.GetMusicDirector().PlayNoteB(0.0f);
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

            foreach (var coord in CoordUtil.AllCoords())
            {
                var cell = MapManager.GridAt(coord);
                if (cell == null)
                {
                    continue;
                }

                char kind, info;
                if (cell.tag == "Blue")
                {
                    kind = 'x';
                    info = cell.name switch
                    {
                        "Blue Hex (Flower)" => '+',
                        _ => '.'
                    };
                }
                else
                {
                    kind = 'o';
                    info = cell.tag switch
                    {
                        "Clue Hex Blank" => '.',
                        "Clue Hex (Sequential)" => 'c',
                        "Clue Hex (NOT Sequential)" => 'n',
                        _ => '+',
                    };
                }

                if (!MapManager.IsHidden(coord))
                {
                    kind = char.ToUpper(kind);
                }

                grid[coord.X * 2, coord.Y] = kind;
                grid[coord.X * 2 + 1, coord.Y] = info;
            }

            foreach (var column in MapManager.Columns)
            {
                var coord = CoordUtil.WorldToGrid(column.transform.position);

                char kind = column.name switch
                {
                    "Column Number Diagonal Right" => '\\',
                    "Column Number Diagonal Left" => '/',
                    _ => '|',
                };
                char info = column.tag switch
                {
                    "Column Sequential" => 'c',
                    "Column NOT Sequential" => 'n',
                    _ => '+'
                };
                var x = coord.X;
                var y = coord.Y;
                grid[x * 2, y] = kind;
                grid[x * 2 + 1, y] = info;
            }

            var sb = new StringBuilder();
            string seed = GameObjectUtil.GetGameManager().seedNumber;
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
