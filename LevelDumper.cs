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
            if (EventManager.IsLevelLoaded && Input.GetKeyDown(KeyCode.F2))
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
            char[,] grid = new char[Coordinate.Height, Coordinate.Width * 2];

            foreach (var coord in Coordinate.AllCoords())
            {
                var cell = MapManager.GridAt(coord);
                var cellType = MapUtil.GetCellType(cell);
                char kind = '.', info = '.';

                if (cellType == CellType.Black)
                {
                    kind = 'o';
                    if (!MapUtil.IsBlackHexBlank(cell))
                    {
                        info = MapUtil.GetBlackHexModifier(cell) switch
                        {
                            Clue.Modifier.Consecutive => 'c',
                            Clue.Modifier.NonConsecutive => 'n',
                            _ => '+',
                        };
                    }
                }
                else if (cellType == CellType.Blue)
                {
                    kind = 'x';
                    info = MapUtil.IsBlueHexFlower(cell) ? '+' : '.';
                }

                if (!MapManager.IsHidden(coord))
                {
                    kind = char.ToUpper(kind);
                }

                grid[coord.Y, coord.X * 2] = kind;
                grid[coord.Y, coord.X * 2 + 1] = info;
            }

            foreach (var column in MapManager.Columns)
            {
                var coord = Coordinate.FromGameObject(column);

                char kind = MapUtil.GetColumnType(column) switch
                {
                    ColumnType.DiagonalLeft => '/',
                    ColumnType.DiagonalRight => '\\',
                    _ => '|',
                };
                char info = MapUtil.GetColumnModifier(column) switch
                {
                    Clue.Modifier.Consecutive => 'c',
                    Clue.Modifier.NonConsecutive => 'n',
                    _ => '+',
                };

                grid[coord.Y, coord.X * 2] = kind;
                grid[coord.Y, coord.X * 2 + 1] = info;
            }

            var sb = new StringBuilder();
            string seed = GameObjectUtil.GetGameManager().seedNumber;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sb.AppendLine(Header);
            sb.AppendLine($"Seed {seed}");  // title
            sb.AppendLine($"Hexcells Infinite (dumped at {timestamp})");  // author
            sb.AppendLine("");
            sb.AppendLine("");

            for (int y = Coordinate.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Coordinate.Width * 2; x++)
                {
                    sb.Append(grid[y, x]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
