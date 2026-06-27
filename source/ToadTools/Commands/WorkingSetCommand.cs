using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using WorkingSet.Views;

namespace ToadTools.Commands;

/// <summary>
///     Opens the worksets-creation window.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class WorkingSetCommand : ExternalCommand
{
    public override void Execute()
    {
        var view = Host.CreateScope<WorkingSetView>();
        view.ShowDialog();
    }
}
