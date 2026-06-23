using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal class ParameterBindingSnapshot
{
    public ElementId? ElementParameterId { get; set; }

    public string? ElementParameterName { get; set; }

    public StorageType ElementParameterStorageType { get; set; }

    public FamilyParameter? FamilyParameter { get; set; }
}
