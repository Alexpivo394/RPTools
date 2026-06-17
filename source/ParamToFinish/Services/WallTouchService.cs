using ParamToFinish.Models;

namespace ParamToFinish.Services;

public sealed class WallTouchService
{
    private readonly WallGeometryService _geometryService;

    private const double ParallelTolerance = 0.001;
    private const double PlaneDistanceTolerance = 0.05; // ~15 мм
    private const double MinOverlapLength = 0.1;        // ~30 мм
    private const double MinOverlapWidth = 0.05;        // ~15 мм
    private const double MinOverlapFraction = 0.25;
    private const double ArcCenterTolerance = 0.1;
    private const double AngularTolerance = 1e-9;
    private const double TwoPi = Math.PI * 2;

    public WallTouchService(
        WallGeometryService geometryService)
    {
        _geometryService = geometryService;
    }

    public WallInfo? FindMainWall(
        WallInfo finishWall,
        IReadOnlyCollection<WallInfo> candidates)
    {
        if (candidates.Count == 0)
            return null;

        var finishFaces = _geometryService.GetSideFaces(
            finishWall.Wall);

        WallInfo? bestWall = null;
        double bestDistance = double.MaxValue;

        foreach (var mainWall in candidates)
        {
            if (mainWall.Wall.Id == finishWall.Wall.Id)
                continue;

            if (!HaveVerticalOverlap(finishWall, mainWall))
                continue;

            var distance = GetTouchDistanceIfTouching(
                finishWall,
                finishFaces,
                mainWall);

            if (distance == null)
                continue;

            if (distance.Value < bestDistance)
            {
                bestDistance = distance.Value;
                bestWall = mainWall;
            }
        }

        return bestWall;
    }

    private double? GetTouchDistanceIfTouching(
        WallInfo finishWall,
        List<PlanarFace> finishFaces,
        WallInfo mainWall)
    {
        var mainFaces = _geometryService.GetAllRelevantFaces(
            mainWall.Wall);

        if (finishFaces.Count > 0 && mainFaces.Count > 0)
        {
            var faceDistance = GetMinimumFaceDistance(
                finishFaces,
                mainFaces);

            if (faceDistance <= PlaneDistanceTolerance)
                return faceDistance;
        }

        if (AreWallsTouchingByCenterline(finishWall, mainWall))
        {
            return GetCenterlineTouchDistance(
                finishWall,
                mainWall);
        }

        return null;
    }

    private bool AreWallsTouchingByCenterline(
        WallInfo finishWall,
        WallInfo mainWall)
    {
        var finishCurve = finishWall.CenterCurve;
        var mainCurve = mainWall.CenterCurve;

        if (finishCurve == null || mainCurve == null)
            return false;

        if (finishCurve is Arc finishArc && mainCurve is Arc mainArc)
        {
            return AreArcWallsTouchingByCenterline(
                finishWall,
                mainWall,
                finishArc,
                mainArc);
        }

        if (finishCurve is not Line finishLine || mainCurve is not Line mainLine)
            return false;

        return AreLineWallsTouchingByCenterline(
            finishWall,
            mainWall,
            finishLine,
            mainLine);
    }

    private static bool AreLineWallsTouchingByCenterline(
        WallInfo finishWall,
        WallInfo mainWall,
        Line finishLine,
        Line mainLine)
    {
        if (!AreLinesParallel(finishLine, mainLine))
            return false;

        if (!HaveVerticalOverlap(finishWall, mainWall))
            return false;

        var centerlineDistance = GetDistanceBetweenParallelLines(
            finishLine,
            mainLine);

        var maxCenterlineDistance =
            (finishWall.Width + mainWall.Width) / 2 + PlaneDistanceTolerance;

        if (centerlineDistance > maxCenterlineDistance)
            return false;

        var overlapLength = GetParallelLinesOverlapLength(
            finishLine,
            mainLine);

        var minRequiredOverlap = Math.Max(
            MinOverlapLength,
            Math.Min(finishLine.Length, mainLine.Length) * MinOverlapFraction);

        return overlapLength >= minRequiredOverlap;
    }

    private static bool AreArcWallsTouchingByCenterline(
        WallInfo finishWall,
        WallInfo mainWall,
        Arc finishArc,
        Arc mainArc)
    {
        if (!AreArcCentersClose(finishArc, mainArc))
            return false;

        if (!HaveVerticalOverlap(finishWall, mainWall))
            return false;

        var centerlineDistance = Math.Abs(finishArc.Radius - mainArc.Radius);

        var maxCenterlineDistance =
            (finishWall.Width + mainWall.Width) / 2 + PlaneDistanceTolerance;

        if (centerlineDistance > maxCenterlineDistance)
            return false;

        var overlapLength = GetArcOverlapLength(
            finishArc,
            mainArc);

        var minRequiredOverlap = Math.Max(
            MinOverlapLength,
            Math.Min(finishArc.Length, mainArc.Length) * MinOverlapFraction);

        return overlapLength >= minRequiredOverlap;
    }

    private static double GetCenterlineTouchDistance(
        WallInfo finishWall,
        WallInfo mainWall)
    {
        var finishCurve = finishWall.CenterCurve;
        var mainCurve = mainWall.CenterCurve;

        if (finishCurve == null || mainCurve == null)
            return double.MaxValue;

        if (finishCurve is Arc finishArc && mainCurve is Arc mainArc)
        {
            if (!AreArcCentersClose(finishArc, mainArc))
                return double.MaxValue;

            var arcCenterlineDistance = Math.Abs(finishArc.Radius - mainArc.Radius);
            var arcExpectedDistance = (finishWall.Width + mainWall.Width) / 2;

            return Math.Abs(arcCenterlineDistance - arcExpectedDistance);
        }

        if (finishCurve is not Line finishLine || mainCurve is not Line mainLine)
            return double.MaxValue;

        if (!AreLinesParallel(finishLine, mainLine))
            return double.MaxValue;

        var centerlineDistance = GetDistanceBetweenParallelLines(
            finishLine,
            mainLine);

        var expectedDistance =
            (finishWall.Width + mainWall.Width) / 2;

        return Math.Abs(centerlineDistance - expectedDistance);
    }

    private double GetMinimumFaceDistance(
        List<PlanarFace> finishFaces,
        List<PlanarFace> mainFaces)
    {
        double min = double.MaxValue;

        foreach (var finishFace in finishFaces)
        {
            foreach (var mainFace in mainFaces)
            {
                if (!AreFacesParallel(finishFace, mainFace))
                    continue;

                var distance = GetDistanceBetweenParallelFaces(
                    finishFace,
                    mainFace);

                if (distance > PlaneDistanceTolerance)
                    continue;

                if (!HaveSignificantFaceOverlap(finishFace, mainFace))
                    continue;

                if (distance < min)
                    min = distance;
            }
        }

        return min;
    }

    private static bool AreFacesParallel(
        PlanarFace a,
        PlanarFace b)
    {
        return a.FaceNormal
            .CrossProduct(b.FaceNormal)
            .GetLength() < ParallelTolerance;
    }

    private static double GetDistanceBetweenParallelFaces(
        PlanarFace a,
        PlanarFace b)
    {
        return Math.Abs(
            (a.Origin - b.Origin)
            .DotProduct(a.FaceNormal));
    }

    private bool HaveSignificantFaceOverlap(
        PlanarFace a,
        PlanarFace b)
    {
        var pointsOnB = ProjectFacePointsOntoFace(a, b);
        var pointsOnA = ProjectFacePointsOntoFace(b, a);

        return HasSignificantUvOverlap(pointsOnB, b, a) ||
               HasSignificantUvOverlap(pointsOnA, a, b);
    }

    private static List<UV> ProjectFacePointsOntoFace(
        PlanarFace source,
        PlanarFace target)
    {
        var result = new List<UV>();

        foreach (var point in GetFaceOutlinePoints(source))
        {
            var projection = target.Project(point);

            if (projection == null)
                continue;

            if (projection.Distance > PlaneDistanceTolerance)
                continue;

            if (IsPointOnOrInsideFace(target, projection.UVPoint))
                result.Add(projection.UVPoint);
        }

        return result;
    }

    private static bool IsPointOnOrInsideFace(
        PlanarFace face,
        UV uv)
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

        var onFaceShorter = Math.Min(
            onFaceBox.Max.U - onFaceBox.Min.U,
            onFaceBox.Max.V - onFaceBox.Min.V);

        var sourceShorter = Math.Min(
            sourceBox.Max.U - sourceBox.Min.U,
            sourceBox.Max.V - sourceBox.Min.V);

        var referenceShorter = Math.Min(
            onFaceShorter,
            sourceShorter);

        var requiredLong = Math.Max(
            MinOverlapLength,
            referenceShorter * MinOverlapFraction);

        var requiredShort = Math.Min(
            MinOverlapWidth,
            referenceShorter * MinOverlapFraction);

        if (referenceShorter < 1.0)
        {
            requiredLong = Math.Min(
                requiredLong,
                referenceShorter * 0.5);

            requiredShort = Math.Min(
                requiredShort,
                referenceShorter * 0.25);
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

        var longer = Math.Max(
            maxU - minU,
            maxV - minV);

        var shorter = Math.Min(
            maxU - minU,
            maxV - minV);

        return longer >= requiredLong &&
               shorter >= requiredShort;
    }

    private static IEnumerable<XYZ> GetFaceOutlinePoints(
        PlanarFace face)
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

    private static bool AreLinesParallel(
        Line a,
        Line b)
    {
        return a.Direction
            .CrossProduct(b.Direction)
            .GetLength() < ParallelTolerance;
    }

    private static double GetDistanceBetweenParallelLines(
        Line a,
        Line b)
    {
        var offset = a.GetEndPoint(0) - b.GetEndPoint(0);
        var along = a.Direction.Multiply(offset.DotProduct(a.Direction));

        return (offset - along).GetLength();
    }

    private static double GetParallelLinesOverlapLength(
        Line a,
        Line b)
    {
        var direction = a.Direction.Normalize();
        var origin = a.GetEndPoint(0);

        double Project(XYZ point)
        {
            return (point - origin).DotProduct(direction);
        }

        var a0 = Project(a.GetEndPoint(0));
        var a1 = Project(a.GetEndPoint(1));
        var b0 = Project(b.GetEndPoint(0));
        var b1 = Project(b.GetEndPoint(1));

        var aMin = Math.Min(a0, a1);
        var aMax = Math.Max(a0, a1);
        var bMin = Math.Min(b0, b1);
        var bMax = Math.Max(b0, b1);

        return Math.Max(
            0,
            Math.Min(aMax, bMax) - Math.Max(aMin, bMin));
    }

    private static bool AreArcCentersClose(
        Arc a,
        Arc b)
    {
        return GetXyDistance(a.Center, b.Center) <= ArcCenterTolerance;
    }

    private static double GetArcOverlapLength(
        Arc a,
        Arc b)
    {
        var aIntervals = GetArcAngleIntervals(a);
        var bIntervals = GetArcAngleIntervals(b);
        var overlapAngle = 0.0;

        foreach (var aInterval in aIntervals)
        {
            foreach (var bInterval in bIntervals)
            {
                overlapAngle += Math.Max(
                    0,
                    Math.Min(aInterval.End, bInterval.End) -
                    Math.Max(aInterval.Start, bInterval.Start));
            }
        }

        return overlapAngle * Math.Min(a.Radius, b.Radius);
    }

    private static List<AngleInterval> GetArcAngleIntervals(
        Arc arc)
    {
        var start = GetAngle(arc.GetEndPoint(0), arc.Center);
        var end = GetAngle(arc.GetEndPoint(1), arc.Center);
        var middle = GetAngle(GetArcMiddlePoint(arc), arc.Center);

        if (IsAngleOnCcwInterval(middle, start, end))
            return SplitAngleInterval(start, NormalizePositive(end - start));

        return SplitAngleInterval(end, NormalizePositive(start - end));
    }

    private static XYZ GetArcMiddlePoint(
        Arc arc)
    {
        return arc.Evaluate(0.5, true);
    }

    private static List<AngleInterval> SplitAngleInterval(
        double start,
        double span)
    {
        var result = new List<AngleInterval>();

        if (span <= AngularTolerance)
            return result;

        start = NormalizeAngle(start);
        var end = start + span;

        if (end <= TwoPi + AngularTolerance)
        {
            result.Add(new AngleInterval(start, Math.Min(end, TwoPi)));
            return result;
        }

        result.Add(new AngleInterval(start, TwoPi));
        result.Add(new AngleInterval(0, end - TwoPi));

        return result;
    }

    private static bool IsAngleOnCcwInterval(
        double angle,
        double start,
        double end)
    {
        var span = NormalizePositive(end - start);
        var offset = NormalizePositive(angle - start);

        return offset <= span + AngularTolerance;
    }

    private static double GetAngle(
        XYZ point,
        XYZ center)
    {
        return NormalizeAngle(Math.Atan2(point.Y - center.Y, point.X - center.X));
    }

    private static double NormalizeAngle(
        double angle)
    {
        return NormalizePositive(angle);
    }

    private static double NormalizePositive(
        double angle)
    {
        angle %= TwoPi;

        if (angle < 0)
            angle += TwoPi;

        return angle;
    }

    private static double GetXyDistance(
        XYZ a,
        XYZ b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static bool HaveVerticalOverlap(
        WallInfo a,
        WallInfo b)
    {
        if (a.Box == null || b.Box == null)
            return true;

        return a.Box.MinZ < b.Box.MaxZ &&
               b.Box.MinZ < a.Box.MaxZ;
    }

    private readonly struct AngleInterval
    {
        public AngleInterval(
            double start,
            double end)
        {
            Start = start;
            End = end;
        }

        public double Start { get; }

        public double End { get; }
    }
}
