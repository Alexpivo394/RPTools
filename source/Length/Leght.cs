﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Leght;

//Assembly
[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class StartupCommand : IExternalCommand
{
    //Get application and document objects
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var taskDialog = new TaskDialog("Task dialog");
        //Set application =>
        var uiapp = commandData.Application;
        var uidoc = uiapp.ActiveUIDocument;
        var app = uiapp.Application;
        var doc = uidoc.Document;

        var selection = uidoc.Selection.GetElementIds()
            .Select(elId => doc.GetElement(elId)).ToList();
        double summ = 0;
        foreach (var element in selection)
        {
            var lengthParam = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
            if (lengthParam != null)
            {
                var length = lengthParam.AsDouble();
#if REVIT2021_OR_GREATER
                var reallength = UnitUtils.ConvertFromInternalUnits(length, UnitTypeId.Millimeters);
#else
                var reallength = UnitUtils.ConvertFromInternalUnits(length, DisplayUnitType.DUT_MILLIMETERS);
#endif
                summ += reallength;
            }
        }

        summ = Math.Round(summ);

        taskDialog.MainContent = summ + " мм";
        taskDialog.Show();
        return Result.Succeeded;
    }
}