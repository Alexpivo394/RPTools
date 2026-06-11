using ParamToFinish.Models;

namespace ParamToFinish.Services;

public class FinishParameterTransferService : IFinishParameterTransferService
{
    private Document _document;

    public FinishParameterTransferService(Document document)
    {
        _document = document;
    }

    private const double ParallelTolerance = 0.001;
    private const double PlaneDistanceTolerance = 0.05; // ~15 мм — зазор в модели
    private const double MinOverlapLength = 0.1; // ~30 мм, абсолютный минимум
    private const double MinOverlapWidth = 0.05; // ~15 мм — отсечь точечное/corner-касание
    private const double MinOverlapFraction = 0.25; // доля от меньшей стороны грани

    public void Transfer(
        ParameterDescriptor? selectedWallParameter,
        ParameterDescriptor? selectedFinishParameter)
    {
        if (string.IsNullOrWhiteSpace(selectedWallParameter?.Name))
            throw new ArgumentException(nameof(selectedWallParameter));

        if (string.IsNullOrWhiteSpace(selectedFinishParameter?.Name))
            throw new ArgumentException(nameof(selectedFinishParameter));

        var walls = new FilteredElementCollector(_document)
            .OfClass(typeof(Wall))
            .Cast<Wall>()
            .ToList();

        var finishWalls = new List<Wall>();
        var mainWalls = new List<Wall>();

        foreach (var wall in walls)
        {
            var value = GetParameterValue(
                wall,
                selectedWallParameter?.Name);

            bool isFinish =
                !string.IsNullOrWhiteSpace(value) &&
                value != null &&
                value.IndexOf("Отделка", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isFinish)
                finishWalls.Add(wall);
            else
                mainWalls.Add(wall);
        }

        using var transaction =
            new Transaction(_document, "Transfer finish parameters");

        transaction.Start();

        foreach (var finishWall in finishWalls)
        {
            var mainWall =
                FindMainWall(finishWall, mainWalls);

            if (mainWall == null)
                continue;

            var sourceValue =
                GetParameterValue(
                    mainWall,
                    selectedWallParameter?.Name);

            if (string.IsNullOrWhiteSpace(sourceValue))
                continue;

            if (sourceValue != null)
                SetParameterValue(
                    finishWall,
                    selectedFinishParameter?.Name,
                    sourceValue);
        }

        transaction.Commit();
    }

    private Wall? FindMainWall(
        Wall finishWall,
        IReadOnlyCollection<Wall> mainWalls)
    {
        var finishFaces = GetSideFaces(finishWall);

        Wall? bestWall = null;
        double bestDistance = double.MaxValue;

        foreach (var mainWall in mainWalls)
        {
            var mainSideFaces = GetSideFaces(mainWall);
            
            var mainEndFaces = GetEndFaces(mainWall);

            var allMainFaces = new List<PlanarFace>();
            allMainFaces.AddRange(mainSideFaces);
            allMainFaces.AddRange(mainEndFaces);

            if (!AreWallsTouching(finishWall, finishFaces, mainWall, allMainFaces))
                continue;

            var distance = GetTouchDistance(finishWall, finishFaces, mainWall, allMainFaces);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestWall = mainWall;
            }
        }

        return bestWall;
    }

    /// <summary>
    /// Соприкосновение по боковым граням или по параллельным осевым линиям
    /// (для коротких участков отделки геометрия граней часто ненадёжна).
    /// </summary>
    private bool AreWallsTouching(
        Wall finishWall,
        List<PlanarFace> finishFaces,
        Wall mainWall,
        List<PlanarFace> mainFaces)
    {
        if (finishFaces.Count > 0 &&
            mainFaces.Count > 0 &&
            AreWallsTouchingByFaces(finishFaces, mainFaces))
            return true;

        return AreWallsTouchingByCenterline(finishWall, mainWall);
    }

    private double GetTouchDistance(
        Wall finishWall,
        List<PlanarFace> finishFaces,
        Wall mainWall,
        List<PlanarFace> mainFaces)
    {
        var faceDistance = GetMinimumFaceDistance(finishFaces, mainFaces);
        var centerlineDistance = GetCenterlineTouchDistance(finishWall, mainWall);

        return Math.Min(faceDistance, centerlineDistance);
    }
    
    private bool AreWallsTouchingByFaces(
        List<PlanarFace> aFaces,
        List<PlanarFace> bFaces)
    {
        foreach (var fa in aFaces)
        {
            foreach (var fb in bFaces)
            {
                if (!AreFacesParallel(fa, fb))
                    continue;

                if (GetDistanceBetweenParallelFaces(fa, fb) > PlaneDistanceTolerance)
                    continue;

                if (HaveSignificantFaceOverlap(fa, fb))
                    return true;
            }
        }

        return false;
    }

    private static bool AreFacesParallel(PlanarFace a, PlanarFace b)
    {
        return a.FaceNormal.CrossProduct(b.FaceNormal).GetLength() < ParallelTolerance;
    }

    private static double GetDistanceBetweenParallelFaces(PlanarFace a, PlanarFace b)
    {
        return Math.Abs((a.Origin - b.Origin).DotProduct(a.FaceNormal));
    }

    /// <summary>
    /// Проверяет перекрытие граней в общей системе координат (UV одной из граней),
    /// а не сравнивает локальные BoundingBoxUV — у каждой грани своя UV-система.
    /// </summary>
    private bool HaveSignificantFaceOverlap(PlanarFace a, PlanarFace b)
    {
        var onB = ProjectFacePointsOntoFace(a, b);
        var onA = ProjectFacePointsOntoFace(b, a);

        return HasSignificantUvOverlap(onB, b, a) ||
               HasSignificantUvOverlap(onA, a, b);
    }

    private static List<UV> ProjectFacePointsOntoFace(
        PlanarFace source,
        PlanarFace target)
    {
        var result = new List<UV>();

        foreach (var point in GetFaceOutlinePoints(source))
        {
            var projection = target.Project(point);

            if (projection == null ||
                projection.Distance > PlaneDistanceTolerance)
                continue;

            if (IsPointOnOrInsideFace(target, projection.UVPoint))
                result.Add(projection.UVPoint);
        }

        return result;
    }

    private static bool IsPointOnOrInsideFace(PlanarFace face, UV uv)
    {
        if (face.IsInside(uv))
            return true;

        var bbox = face.GetBoundingBox();

        return uv.U >= bbox.Min.U &&
               uv.U <= bbox.Max.U &&
               uv.V >= bbox.Min.V &&
               uv.V <= bbox.Max.V;
    }

    private static bool HasSignificantUvOverlap(
        IReadOnlyList<UV> points,
        PlanarFace onFace,
        PlanarFace sourceFace)
    {
        if (points.Count == 0)
            return false;

        var onFaceBox = onFace.GetBoundingBox();
        var sourceBox = sourceFace.GetBoundingBox();

        var onShorter = Math.Min(
            onFaceBox.Max.U - onFaceBox.Min.U,
            onFaceBox.Max.V - onFaceBox.Min.V);
        var sourceShorter = Math.Min(
            sourceBox.Max.U - sourceBox.Min.U,
            sourceBox.Max.V - sourceBox.Min.V);

        var referenceShorter = Math.Min(onShorter, sourceShorter);
        var requiredLong = Math.Max(
            MinOverlapLength,
            referenceShorter * MinOverlapFraction);
        var requiredShort = Math.Min(
            MinOverlapWidth,
            referenceShorter * MinOverlapFraction);

        // Короткая отделка (~200 мм): достаточно покрыть половину её длинной стороны.
        if (referenceShorter < 1.0)
        {
            requiredLong = Math.Min(requiredLong, referenceShorter * 0.5);
            requiredShort = Math.Min(requiredShort, referenceShorter * 0.25);
        }

        if (points.Count == 1)
            return referenceShorter <= requiredLong;

        var minU = points[0].U;
        var maxU = points[0].U;
        var minV = points[0].V;
        var maxV = points[0].V;

        for (var i = 1; i < points.Count; i++)
        {
            minU = Math.Min(minU, points[i].U);
            maxU = Math.Max(maxU, points[i].U);
            minV = Math.Min(minV, points[i].V);
            maxV = Math.Max(maxV, points[i].V);
        }

        var longer = Math.Max(maxU - minU, maxV - minV);
        var shorter = Math.Min(maxU - minU, maxV - minV);

        return longer >= requiredLong && shorter >= requiredShort;
    }

    private static IEnumerable<XYZ> GetFaceOutlinePoints(PlanarFace face)
    {
        foreach (EdgeArray loop in face.EdgeLoops)
        {
            foreach (Edge edge in loop)
            {
                yield return edge.AsCurve().GetEndPoint(0);

                foreach (var point in edge.Tessellate())
                    yield return point;
            }
        }
    }

    private bool AreWallsTouchingByCenterline(Wall finishWall, Wall mainWall)
    {
        if (!TryGetWallLine(finishWall, out var finishLine) ||
            !TryGetWallLine(mainWall, out var mainLine))
            return false;

        if (!AreLinesParallel(finishLine, mainLine))
            return false;

        if (!HaveVerticalOverlap(finishWall, mainWall))
            return false;

        var centerlineDistance = GetDistanceBetweenParallelLines(finishLine, mainLine);
        var maxCenterlineDistance =
            (finishWall.Width + mainWall.Width) / 2 + PlaneDistanceTolerance;

        if (centerlineDistance > maxCenterlineDistance)
            return false;

        var overlapLength = GetParallelLinesOverlapLength(finishLine, mainLine);
        var minRequiredOverlap = Math.Max(
            MinOverlapLength,
            Math.Min(finishLine.Length, mainLine.Length) * MinOverlapFraction);

        return overlapLength >= minRequiredOverlap;
    }

    private static double GetCenterlineTouchDistance(Wall finishWall, Wall mainWall)
    {
        if (!TryGetWallLine(finishWall, out var finishLine) ||
            !TryGetWallLine(mainWall, out var mainLine) ||
            !AreLinesParallel(finishLine, mainLine))
            return double.MaxValue;

        var centerlineDistance = GetDistanceBetweenParallelLines(finishLine, mainLine);
        var expectedDistance = (finishWall.Width + mainWall.Width) / 2;

        return Math.Abs(centerlineDistance - expectedDistance);
    }

    private static bool TryGetWallLine(Wall wall, out Line line)
    {
        line = null!;

        if (wall.Location is not LocationCurve locationCurve)
            return false;

        if (locationCurve.Curve is not Line wallLine)
            return false;

        line = wallLine;
        return true;
    }

    private static bool AreLinesParallel(Line a, Line b)
    {
        return a.Direction.CrossProduct(b.Direction).GetLength() < ParallelTolerance;
    }

    private static double GetDistanceBetweenParallelLines(Line a, Line b)
    {
        var offset = a.GetEndPoint(0) - b.GetEndPoint(0);
        var along = offset.DotProduct(a.Direction) * a.Direction;

        return (offset - along).GetLength();
    }

    private static double GetParallelLinesOverlapLength(Line a, Line b)
    {
        var direction = a.Direction.Normalize();
        var origin = a.GetEndPoint(0);

        double Project(XYZ point) => (point - origin).DotProduct(direction);

        var aMin = Math.Min(Project(a.GetEndPoint(0)), Project(a.GetEndPoint(1)));
        var aMax = Math.Max(Project(a.GetEndPoint(0)), Project(a.GetEndPoint(1)));
        var bMin = Math.Min(Project(b.GetEndPoint(0)), Project(b.GetEndPoint(1)));
        var bMax = Math.Max(Project(b.GetEndPoint(0)), Project(b.GetEndPoint(1)));

        return Math.Max(0, Math.Min(aMax, bMax) - Math.Max(aMin, bMin));
    }

    private static bool HaveVerticalOverlap(Wall a, Wall b)
    {
        var boxA = a.get_BoundingBox(null);
        var boxB = b.get_BoundingBox(null);

        if (boxA == null || boxB == null)
            return true;

        return boxA.Min.Z < boxB.Max.Z && boxB.Min.Z < boxA.Max.Z;
    }

    private double GetMinimumFaceDistance(
        List<PlanarFace> aFaces,
        List<PlanarFace> bFaces)
    {
        double min = double.MaxValue;

        foreach (var a in aFaces)
        {
            foreach (var b in bFaces)
            {
                if (!AreFacesParallel(a, b))
                    continue;

                if (!HaveSignificantFaceOverlap(a, b))
                    continue;

                var distance = GetDistanceBetweenParallelFaces(a, b);

                if (distance < min)
                    min = distance;
            }
        }

        return min;
    }

    /// <summary>
    /// Боковые (длинные) грани через HostObjectUtils — без торцов, верха и низа.
    /// </summary>
    private static List<PlanarFace> GetSideFaces(Wall wall)
    {
        var result = new List<PlanarFace>();
        var layers = new[] { ShellLayerType.Exterior, ShellLayerType.Interior };

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

        return result;
    }
    
    /// <summary>
/// Торцевые грани стены — без боковых граней, верха и низа.
/// </summary>
private static List<PlanarFace> GetEndFaces(Wall wall)
{
    var result = new List<PlanarFace>();

    var locationCurve = wall.Location as LocationCurve;
    if (locationCurve?.Curve == null)
        return result;

    var curve = locationCurve.Curve;

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
        return result;

    foreach (var geometryObject in geometry)
    {
        if (geometryObject is Solid solid)
        {
            CollectEndFacesFromSolid(solid, wallDirection, result);
        }
        else if (geometryObject is GeometryInstance instance)
        {
            var instanceGeometry = instance.GetInstanceGeometry();

            foreach (var instanceObject in instanceGeometry)
            {
                if (instanceObject is Solid instanceSolid)
                    CollectEndFacesFromSolid(instanceSolid, wallDirection, result);
            }
        }
    }

    return result;
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

        // Верх/низ нахуй не нужны
        if (Math.Abs(normal.DotProduct(XYZ.BasisZ)) > tolerance)
            continue;

        // Торец стены — грань, нормаль которой смотрит вдоль оси стены
        if (Math.Abs(normal.DotProduct(wallDirection)) > tolerance)
            result.Add(planarFace);
    }
}

    private string? GetParameterValue(
        Element element,
        string? parameterName)
    {
        var parameter =
            GetParameter(
                element,
                parameterName);

        return parameter?.AsString();
    }

    private void SetParameterValue(
        Element element,
        string? parameterName,
        string value)
    {
        var parameter =
            GetParameter(
                element,
                parameterName);

        if (parameter == null)
            return;

        if (parameter.IsReadOnly)
            return;

        parameter.Set(value);
    }

    private Parameter? GetParameter(
        Element element,
        string? parameterName)
    {
        var parameter =
            element.LookupParameter(parameterName);

        if (parameter != null)
            return parameter;

        var type =
            element.Document.GetElement(
                element.GetTypeId());

        return type?.LookupParameter(parameterName);
    }
}