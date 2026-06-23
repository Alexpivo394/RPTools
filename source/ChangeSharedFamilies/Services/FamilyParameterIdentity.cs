using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal static class FamilyParameterIdentity
{
    public static bool IsSame(
        FamilyParameter? left,
        FamilyParameter? right)
    {
        if (left == null || right == null)
            return false;

        if (left.Id != null && right.Id != null)
            return left.Id.Equals(right.Id);

        return string.Equals(
            left.Definition?.Name,
            right.Definition?.Name,
            StringComparison.OrdinalIgnoreCase);
    }
}
