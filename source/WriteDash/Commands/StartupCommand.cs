using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using WriteDash.Services;
using WriteDash.ViewModels;
using WriteDash.Views;

namespace WriteDash.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        var doc = commandData.Application
            .ActiveUIDocument
            .Document;

        var parameterService = new RevitParameterService();
        var processorService = new ParameterProcessorService(doc);

        var viewModel = new WriteDashViewModel(
            doc,
            parameterService,
            processorService);

        var view = new WriteDashView(viewModel);

        view.ShowDialog();

        return Result.Succeeded;
    }
}