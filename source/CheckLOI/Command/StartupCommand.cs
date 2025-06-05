using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckLOI.View;
using System.Windows.Interop;
using System.Windows;
using CheckLOI.Models;
using CheckLOI.Services;

namespace CheckLOI.Command
{
    [Transaction(TransactionMode.Manual)]
    public class StartupCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var categoryService = new CategoryService();
            categoryService.Initialize(doc);
            var vm = new CheckLOI.ViewModels.CheckLoiViewModel(commandData, categoryService);
            var view = new CheckLoiView(vm);
            view.ShowDialog();
            return Result.Succeeded;
        }
    }
}
