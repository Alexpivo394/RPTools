namespace ParamToFinish.Models;

public sealed class Box3D
{
    private Box3D(
        double minX,
        double minY,
        double minZ,
        double maxX,
        double maxY,
        double maxZ)
    {
        MinX = minX;
        MinY = minY;
        MinZ = minZ;
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;
    }

    public double MinX { get; }

    public double MinY { get; }

    public double MinZ { get; }

    public double MaxX { get; }

    public double MaxY { get; }

    public double MaxZ { get; }

    public static Box3D? FromWall(
        Wall wall,
        double expand,
        Line? fallbackLine)
    {
        var box = wall.get_BoundingBox(null);

        if (box != null)
        {
            return new Box3D(
                box.Min.X - expand,
                box.Min.Y - expand,
                box.Min.Z - expand,
                box.Max.X + expand,
                box.Max.Y + expand,
                box.Max.Z + expand);
        }

        if (fallbackLine == null)
            return null;

        var p0 = fallbackLine.GetEndPoint(0);
        var p1 = fallbackLine.GetEndPoint(1);

        return new Box3D(
            Math.Min(p0.X, p1.X) - expand,
            Math.Min(p0.Y, p1.Y) - expand,
            Math.Min(p0.Z, p1.Z) - expand,
            Math.Max(p0.X, p1.X) + expand,
            Math.Max(p0.Y, p1.Y) + expand,
            Math.Max(p0.Z, p1.Z) + expand);
    }

    public bool Intersects(
        Box3D other)
    {
        return MinX <= other.MaxX &&
               MaxX >= other.MinX &&
               MinY <= other.MaxY &&
               MaxY >= other.MinY &&
               MinZ <= other.MaxZ &&
               MaxZ >= other.MinZ;
    }
}