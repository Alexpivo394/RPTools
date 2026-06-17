using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal class FamilyTypeValueSnapshot
{
    public string FamilyTypeName { get; set; }

    public FamilyParameter FamilyParameter { get; set; }

    public string OldSymbolName { get; set; }
}
