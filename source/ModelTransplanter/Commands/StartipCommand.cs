using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using ModelTransplanter.Services;
using ModelTransplanter.ViewModels;
using ModelTransplanter.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
                TaskDialog.Show("Ошибка", "Документ открыт в режиме только для чтения");
                return Result.Cancelled;
            }

            var viewModel = new ModelTransplanterViewModel(commandData.Application);
            var view = new ModelTransplanterView(viewModel);
            view.ShowDialog();

            return Result.Succeeded;
        }
    }
}
