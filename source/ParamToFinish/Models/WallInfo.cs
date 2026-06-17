namespace ParamToFinish.Models;

public sealed class WallInfo
{
    public WallInfo(
        Wall wall,
        string? sourceValue,
        Curve? centerCurve,
        Box3D? box)
    {
        Wall = wall;
        SourceValue = sourceValue;
        CenterCurve = centerCurve;
        CenterLine = centerCurve as Line;
        Box = box;
        Width = wall.Width;
    }

    public Wall Wall { get; }

    public string? SourceValue { get; }

    public Curve? CenterCurve { get; }

    public Line? CenterLine { get; }

    public Box3D? Box { get; }

    public double Width { get; }
}
