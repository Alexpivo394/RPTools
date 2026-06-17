using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using ParamToFinish.Services;
using ParamToFinish.ViewModels;
using ParamToFinish.Views;

namespace ParamToFinish.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var doc = commandData.Application
            .ActiveUIDocument
            .Document;

        var parameterService = new GetParameterService();
        
        var wallParameters = parameterService.GetWallParameters(doc);

        if (wallParameters.Count == 0)
        {
            return Result.Cancelled;
        }

        var transferService = new FinishParameterTransferService(doc);
        var settingsService = new ParamToFinishSettingsService();

        var viewModel = new ParamToFinishViewModel(wallParameters, transferService, settingsService);

        var view = new ParamToFinishView(viewModel);
        
        view.ShowDialog();
        
        return Result.Succeeded;
    }
}
