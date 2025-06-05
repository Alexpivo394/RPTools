using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using ParamChecker.Services;
using ParamChecker.ViewModels.Windows;

namespace ParamChecker.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {

        var doc = commandData.Application.ActiveUIDocument.Document;
        var categoryService = new CategoryService();
        categoryService.Initialize(doc);
        var vm = new ParamCheckerViewModel(categoryService);
        var view = new Views.Windows.ParamChecker(vm);
        view.ShowDialog();
        return Result.Succeeded;
    }
}