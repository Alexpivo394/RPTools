using Autodesk.Revit.UI;
using SSPlan.ViewModels;

namespace SSPlan.Services;

public class PickPanelHandler : IExternalEventHandler
{
    private readonly SSPlanViewModel _viewModel;
    private readonly IWindowService? _windowService;
    private readonly UIDocument _uidoc;

    public PickPanelHandler(SSPlanViewModel vm, IWindowService? windowService, UIDocument uidoc)
    {
        _viewModel = vm;
        _windowService = windowService;
        _uidoc = uidoc;
    }

    public void Execute(UIApplication app)
    {
        try
        {
            var uiDoc = app.ActiveUIDocument;
            var refElem = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Выбери панель");
            var element = uiDoc.Document.GetElement(refElem);

            if (element is FamilyInstance fi)
            {
                var panelInCollection = _viewModel.Panels.FirstOrDefault(p => p.Id == fi.Id);
                if (panelInCollection != null)
                {
                    _viewModel.SelectedPanel = panelInCollection;
                }
                else
                {
                    // var newPanel = new PanelItem(fi);
                    // _viewModel.Panels.Add(newPanel);
                    // _viewModel.SelectedPanel = newPanel;
                }
            }
        }
        catch
        {
            // юзер нажал ESC
        }
        finally
        {
            _windowService?.Show();
        }
    }

    public string GetName() => nameof(PickPanelHandler);
}
