namespace ParamToFinish.Models;

public sealed class WallInfo
{
    public WallInfo(
        Wall wall,
        string? sourceValue,
        Line? centerLine,
        Box3D? box)
    {
        Wall = wall;
        SourceValue = sourceValue;
        CenterLine = centerLine;
        Box = box;
        Width = wall.Width;
    }

    public Wall Wall { get; }

    public string? SourceValue { get; }

    public Line? CenterLine { get; }

    public Box3D? Box { get; }

    public double Width { get; }
}