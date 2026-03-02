using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using CreateSpaces.Services;
using CreateSpaces.ViewModels;
using CreateSpaces.Views;

namespace CreateSpaces.Commands;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var doc = commandData.Application.ActiveUIDocument.Document;
        var linkProvider = new RevitLinkProvider();
        var getParameterService = new GetParameterService(doc);
        var roomProvider = new RevitRoomProvider(doc);
        var loadParameters = new LoadParametersService(getParameterService, roomProvider);
        var createService = new SpaceCreationService(linkProvider, roomProvider, doc);
        
        
        
        var links = linkProvider.GetLinks(doc);
        var vm = new CreateSpacesViewModel(links, loadParameters, roomProvider, createService);
        
        var view = new CreateSpacesView(vm);
        
        view.ShowDialog();
        return Result.Succeeded;
    }
}