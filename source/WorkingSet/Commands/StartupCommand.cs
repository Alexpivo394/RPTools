using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WorkingSet.Views;

namespace WorkingSet.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class StartupCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var vm = new WorkingSet.ViewModels.WorkingSetViewModel(commandData);
            var view = new WorkingSetView(vm);
            view.ShowDialog();
            return Result.Succeeded;
        }
    }
}
