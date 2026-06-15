using ParamToFinish.Models;

namespace ParamToFinish.Services;

public sealed class WallSpatialIndex
{
    private readonly Dictionary<CellKey, List<WallInfo>> _cells = new();
    private readonly List<WallInfo> _wallsWithoutBox = new();
    private readonly double _cellSize;

    public WallSpatialIndex(
        IEnumerable<WallInfo> walls,
        double cellSize)
    {
        _cellSize = cellSize;

        foreach (var wall in walls)
        {
            if (wall.Box == null)
            {
                _wallsWithoutBox.Add(wall);
                continue;
            }

            foreach (var cell in GetCells(wall.Box))
            {
                if (!_cells.TryGetValue(cell, out var list))
                {
                    list = new List<WallInfo>();
                    _cells[cell] = list;
                }

                list.Add(wall);
            }
        }
    }

    public List<WallInfo> Query(
        Box3D box)
    {
        var result = new List<WallInfo>();
        var added = new HashSet<ElementId>();

        foreach (var cell in GetCells(box))
        {
            if (!_cells.TryGetValue(cell, out var walls))
                continue;

            foreach (var wall in walls)
            {
                if (wall.Box == null)
                    continue;

                if (!wall.Box.Intersects(box))
                    continue;

                if (!added.Add(wall.Wall.Id))
                    continue;

                result.Add(wall);
            }
        }

        foreach (var wall in _wallsWithoutBox)
        {
            if (!added.Add(wall.Wall.Id))
                continue;

            result.Add(wall);
        }

        return result;
    }

    private IEnumerable<CellKey> GetCells(
        Box3D box)
    {
        var minX = ToCell(box.MinX);
        var maxX = ToCell(box.MaxX);
        var minY = ToCell(box.MinY);
        var maxY = ToCell(box.MaxY);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                yield return new CellKey(x, y);
            }
        }
    }

    private int ToCell(
        double value)
    {
        return (int)Math.Floor(value / _cellSize);
    }

    private readonly struct CellKey : IEquatable<CellKey>
    {
        public CellKey(
            int x,
            int y)
        {
            X = x;
            Y = y;
        }

        private int X { get; }

        private int Y { get; }

        public bool Equals(
            CellKey other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override bool Equals(
            object? obj)
        {
            return obj is CellKey other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}