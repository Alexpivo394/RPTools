using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using ParamChecker.Services;
using ParamChecker.ViewModels.Windows;
using ParamChecker.Views.Windows;
using RPToolsUI.Services;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;

namespace ParamChecker.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        ThemeWatcherService.Initialize();
        ThemeWatcherService.ApplyTheme(ApplicationTheme.Light);
        var doc = commandData.Application.ActiveUIDocument.Document;
        var categoryService = new CategoryService();
        categoryService.Initialize(doc);
        var vm = new ParamCheckerViewModel();
        var view = new Views.Windows.ParamChecker(vm);
        view.ShowDialog();
        return Result.Succeeded;
    }
}