using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using WorksetCheck.ViewModels;
using WorksetCheck.Views;
using WorksetCheck.Models;

namespace WorksetCheck.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var doc = commandData.Application.ActiveUIDocument.Document;
        var model = new WorksetCheckModel(commandData);
        var vm = new WorksetCheckViewModel();
        var view = new WorksetCheckView(vm);
        
        view.ShowDialog();
        return Result.Succeeded;
    }
}