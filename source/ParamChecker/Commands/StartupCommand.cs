using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using ParamChecker.Services;
using ParamChecker.ViewModels.PagesViewModels;
using ParamChecker.ViewModels.Windows;

namespace ParamChecker.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var logger = new Logger();
        var doc = commandData.Application.ActiveUIDocument.Document;
        var categoryService = new CategoryService();
        categoryService.Initialize(doc);
        var setvm = new SettingsViewModel();
        var exportService = new ExportService(commandData, categoryService, setvm,  logger);
        var vm = new ParamCheckerViewModel(categoryService, exportService, setvm, logger);
        var view = new Views.Windows.ParamChecker(vm, setvm);
        view.ShowDialog();
        return Result.Succeeded;
    }
}