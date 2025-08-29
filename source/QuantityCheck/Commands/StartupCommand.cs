using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using QuantityCheck.Models;
using QuantityCheck.Services;
using QuantityCheck.ViewModels;
using QuantityCheck.Views;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace QuantityCheck.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        // Добавить что-то, возможно DI
        var logger = new Logger();
        var doc = commandData.Application.ActiveUIDocument.Document;
        
        
        
        var vm = new QuantityCheckViewModel(logger, doc);
        var view = new QuantityCheckView(vm);

        view.ShowDialog();
        return Result.Succeeded;
    }
}