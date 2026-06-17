using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal class NestedFamilyFinder
{
    private readonly Document _familyDocument;

    public NestedFamilyFinder(Document familyDocument)
    {
        _familyDocument = familyDocument;
    }

    public List<Family> GetSharedNestedFamilies()
    {
        var ownerFamily = _familyDocument.OwnerFamily;

        return new FilteredElementCollector(_familyDocument)
            .OfClass(typeof(Family))
            .Cast<Family>()
            .Where(f => ownerFamily == null || !f.Id.Equals(ownerFamily.Id))
            .Where(f => f.IsEditable)
            .Where(f => !f.IsInPlace)
            .Where(IsSharedFamily)
            .ToList();
    }

    private static bool IsSharedFamily(Family family)
    {
        var parameter = family.get_Parameter(BuiltInParameter.FAMILY_SHARED);

        return parameter != null &&
               parameter.StorageType == StorageType.Integer &&
               parameter.AsInteger() == 1;
    }
}
