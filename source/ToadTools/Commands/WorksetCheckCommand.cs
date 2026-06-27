using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using WorksetCheck.Views;

namespace ToadTools.Commands;

/// <summary>
///     Opens the worksets-check window.
/// </summary>
[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class WorksetCheckCommand : ExternalCommand
{
    public override void Execute()
    {
        var view = Host.CreateScope<WorksetCheckView>();
        view.ShowDialog();
    }
}
