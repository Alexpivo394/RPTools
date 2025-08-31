using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using SSPlan.ViewModels;
using SSPlan.Views;

namespace SSPlan.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var doc = commandData.Application.ActiveUIDocument.Document;
        var vm = new SSPlanViewModel(doc);
        vm.LoadPanels();
        var view = new SSPlanView(vm);
        
        view.ShowDialog();
        return Result.Succeeded;
    }
}      