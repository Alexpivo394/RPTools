using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;
using SSPlan.ViewModels;
using SSPlan.Views;
using SSPlan.Services;

namespace SSPlan.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDoc = commandData.Application.ActiveUIDocument;
        var doc = commandData.Application.ActiveUIDocument.Document;
        var viewService = new ViewService(uiDoc);

        var activeView = viewService.GetActiveDraftingOrSheetView();

        if (activeView == null)
        {
            string? error = ToadDialogService.Show(
                "Ошибка",
                "Активный аид не является чертежным видом",
                DialogButtons.OK,
                DialogIcon.Error
            );
            return Result.Failed;
        }

        // Создаем ViewModel с правильными параметрами
        var vm = new SSPlanViewModel(doc, uiDoc,  activeView);
        vm.LoadPanels();
        vm.LoadFamilies();
        
        // Создаем View и передаем ViewModel
        var view = new SSPlanView(vm);
        
        // Создаем WindowService и передаем окно
        var windowService = new SSPlanWindowService(view);
        
        // Устанавливаем WindowService в ViewModel
        vm.SetWindowService(windowService);
        
        // Показываем окно
        windowService.Show();
        
        return Result.Succeeded;
    }
}