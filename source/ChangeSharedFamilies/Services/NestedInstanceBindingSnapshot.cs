using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal class NestedInstanceBindingSnapshot
{
    public ElementId InstanceId { get; set; }

    public string OldSymbolName { get; set; }

    public List<ParameterBindingSnapshot> Bindings { get; } =
        new List<ParameterBindingSnapshot>();
}
