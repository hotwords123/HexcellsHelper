using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexcellsHelper
{
    public record struct Coordinate(int X, int Y)
    {
        public const int Width = 33;
        public const int Height = 33;

        public readonly bool IsValid()
        {
            return 0 <= X && X < Width && 0 <= Y && Y < Height;
        }

        public static Coordinate operator +(Coordinate a, Coordinate b) =>
            new(a.X + b.X, a.Y + b.Y);

        public static Coordinate FromWorldPosition(Vector3 vec)
        {
            return new(
                Mathf.RoundToInt(vec.x / 0.88f) + 15,
                Mathf.RoundToInt(vec.y / 0.5f) + 15
            );
        }

        public static Coordinate FromGameObject(GameObject go)
        {
            return FromWorldPosition(go.transform.position);
        }

        public static IEnumerable<Coordinate> AllCoords()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return new(x, y);
                }
            }
        }

        public static readonly Coordinate[] SurroundOffsets =
        [
            new(0, 2), new(1, 1), new(1, -1), new(0, -2),
            new(-1, -1), new(-1, 1)
        ];

        public static readonly Coordinate[] FlowerOffsets =
        [
            new(0, 2), new(1, 1), new(1, -1), new(0, -2),
            new(-1, -1), new(-1, 1), new(0, 4), new(1, 3),
            new(2, 2), new(2, 0), new(2, -2), new(1, -3),
            new(0, -4), new(-1, -3), new(-2, -2), new(-2, 0),
            new(-2, 2), new(-1, 3),
        ];

        public static IEnumerable<Coordinate> OffsetCoords(Coordinate coord, IEnumerable<Coordinate> offsets)
        {
            return offsets.Select(offset => coord + offset);
        }

        public readonly IEnumerable<Coordinate> SurroundCoords()
        {
            return OffsetCoords(this, SurroundOffsets);
        }
        public readonly IEnumerable<Coordinate> FlowerCoords()
        {
            return OffsetCoords(this, FlowerOffsets);
        }

        public readonly IEnumerable<Coordinate> ColumnCoords(ColumnType columnType)
        {
            var direction = columnType switch
            {
                ColumnType.DiagonalLeft => new Coordinate(-1, -1),
                ColumnType.DiagonalRight => new Coordinate(1, -1),
                _ => new Coordinate(0, -2),
            };
            var coord = this + direction;
            while (coord.IsValid())
            {
                yield return coord;
                coord += direction;
            }
        }
    }
}
