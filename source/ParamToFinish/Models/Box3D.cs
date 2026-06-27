using Autodesk.Revit.DB;

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
        Curve? fallbackCurve)
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

        if (fallbackCurve == null)
            return null;

        var points = fallbackCurve.Tessellate();

        if (points.Count == 0)
            return null;

        var minX = points[0].X;
        var minY = points[0].Y;
        var minZ = points[0].Z;
        var maxX = points[0].X;
        var maxY = points[0].Y;
        var maxZ = points[0].Z;

        for (var i = 1; i < points.Count; i++)
        {
            minX = Math.Min(minX, points[i].X);
            minY = Math.Min(minY, points[i].Y);
            minZ = Math.Min(minZ, points[i].Z);
            maxX = Math.Max(maxX, points[i].X);
            maxY = Math.Max(maxY, points[i].Y);
            maxZ = Math.Max(maxZ, points[i].Z);
        }

        return new Box3D(
            minX - expand,
            minY - expand,
            minZ - expand,
            maxX + expand,
            maxY + expand,
            maxZ + expand);
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
