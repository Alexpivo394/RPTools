namespace ParamToFinish.Services;

public sealed class WallGeometryService
{
    private readonly Dictionary<ElementId, List<PlanarFace>> _sideFacesCache = new();
    private readonly Dictionary<ElementId, List<PlanarFace>> _endFacesCache = new();
    private readonly Dictionary<ElementId, List<PlanarFace>> _allFacesCache = new();

    /// <summary>
    /// Боковые длинные грани стены.
    /// Через HostObjectUtils получаем exterior/interior, без торцов, верха и низа.
    /// </summary>
    public List<PlanarFace> GetSideFaces(
        Wall wall)
    {
        if (_sideFacesCache.TryGetValue(wall.Id, out var cached))
            return cached;

        var result = new List<PlanarFace>();

        var layers = new[]
        {
            ShellLayerType.Exterior,
            ShellLayerType.Interior
        };

        foreach (var layer in layers)
        {
            IList<Reference> references;

            try
            {
                references = HostObjectUtils.GetSideFaces(wall, layer);
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException)
            {
                continue;
            }

            foreach (var reference in references)
            {
                if (wall.GetGeometryObjectFromReference(reference) is PlanarFace face)
                    result.Add(face);
            }
        }

        _sideFacesCache[wall.Id] = result;
        return result;
    }

    /// <summary>
    /// Боковые грани + торцы. Верх и низ не берём.
    /// </summary>
    public List<PlanarFace> GetAllRelevantFaces(
        Wall wall)
    {
        if (_allFacesCache.TryGetValue(wall.Id, out var cached))
            return cached;

        var result = new List<PlanarFace>();

        result.AddRange(GetSideFaces(wall));
        result.AddRange(GetEndFaces(wall));

        _allFacesCache[wall.Id] = result;
        return result;
    }

    /// <summary>
    /// Торцевые грани стены.
    /// Отсекаем боковые, верх и низ.
    /// </summary>
    public List<PlanarFace> GetEndFaces(
        Wall wall)
    {
        if (_endFacesCache.TryGetValue(wall.Id, out var cached))
            return cached;

        var result = new List<PlanarFace>();

        if (wall.Location is not LocationCurve locationCurve)
        {
            _endFacesCache[wall.Id] = result;
            return result;
        }

        var curve = locationCurve.Curve;

        if (curve == null)
        {
            _endFacesCache[wall.Id] = result;
            return result;
        }

        var endDirections = GetCurveEndDirections(curve);

        var options = new Options
        {
            ComputeReferences = true,
            DetailLevel = ViewDetailLevel.Fine,
            IncludeNonVisibleObjects = false
        };

        var geometry = wall.get_Geometry(options);

        if (geometry == null)
        {
            _endFacesCache[wall.Id] = result;
            return result;
        }

        foreach (var geometryObject in geometry)
        {
            if (geometryObject is Solid solid)
            {
                CollectEndFacesFromSolid(
                    solid,
                    endDirections,
                    result);
            }
            else if (geometryObject is GeometryInstance instance)
            {
                var instanceGeometry = instance.GetInstanceGeometry();

                foreach (var instanceObject in instanceGeometry)
                {
                    if (instanceObject is Solid instanceSolid)
                    {
                        CollectEndFacesFromSolid(
                            instanceSolid,
                            endDirections,
                            result);
                    }
                }
            }
        }

        _endFacesCache[wall.Id] = result;
        return result;
    }

    public bool TryGetWallLocationCurve(
        Wall wall,
        out Curve curve)
    {
        curve = null!;

        if (wall.Location is not LocationCurve locationCurve)
            return false;

        if (locationCurve.Curve == null)
            return false;

        curve = locationCurve.Curve;
        return true;
    }

    public bool TryGetWallLine(
        Wall wall,
        out Line line)
    {
        line = null!;

        if (!TryGetWallLocationCurve(wall, out var curve))
            return false;

        if (curve is not Line wallLine)
            return false;

        line = wallLine;
        return true;
    }

    private static void CollectEndFacesFromSolid(
        Solid solid,
        IReadOnlyList<XYZ> wallEndDirections,
        List<PlanarFace> result)
    {
        if (solid == null || solid.Volume <= 0)
            return;

        const double tolerance = 0.98;

        foreach (Face face in solid.Faces)
        {
            if (face is not PlanarFace planarFace)
                continue;

            var normal = planarFace.FaceNormal.Normalize();

            if (Math.Abs(normal.DotProduct(XYZ.BasisZ)) > tolerance)
                continue;

            if (wallEndDirections.Any(direction =>
                    Math.Abs(normal.DotProduct(direction)) > tolerance))
                result.Add(planarFace);
        }
    }

    private static List<XYZ> GetCurveEndDirections(
        Curve curve)
    {
        var result = new List<XYZ>();
        var points = curve.Tessellate();

        if (points.Count >= 2)
        {
            AddDirection(result, points[1] - points[0]);
            AddDirection(
                result,
                points[points.Count - 1] - points[points.Count - 2]);
        }

        if (result.Count == 0)
        {
            AddDirection(result, curve.GetEndPoint(1) - curve.GetEndPoint(0));
        }

        return result;
    }

    private static void AddDirection(
        List<XYZ> directions,
        XYZ vector)
    {
        if (vector.GetLength() <= 1e-9)
            return;

        directions.Add(vector.Normalize());
    }
}
