using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModelTransplanter.ViewModels;
using ModelTransplanter.Views;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace ModelTransplanter.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class StartupCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData.Application.ActiveUIDocument?.Document?.IsReadOnly == true)
            {
                var dial = ToadDialogService.Show(
                    "Ошибка!",
                    $"Документ открыт в режиме только для чтения",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
                return Result.Cancelled;
            }

            var viewModel = new ModelTransplanterViewModel(commandData.Application);
            var view = new ModelTransplanterView(viewModel);
            view.ShowDialog();

            return Result.Succeeded;
        }
    }
}
