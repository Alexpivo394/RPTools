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

        var startPoint = curve.GetEndPoint(0);
        var endPoint = curve.GetEndPoint(1);

        var wallDirection = (endPoint - startPoint).Normalize();

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
                    wallDirection,
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
                            wallDirection,
                            result);
                    }
                }
            }
        }

        _endFacesCache[wall.Id] = result;
        return result;
    }

    public bool TryGetWallLine(
        Wall wall,
        out Line line)
    {
        line = null!;

        if (wall.Location is not LocationCurve locationCurve)
            return false;

        if (locationCurve.Curve is not Line wallLine)
            return false;

        line = wallLine;
        return true;
    }

    private static void CollectEndFacesFromSolid(
        Solid solid,
        XYZ wallDirection,
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

            if (Math.Abs(normal.DotProduct(wallDirection)) > tolerance)
                result.Add(planarFace);
        }
    }
}