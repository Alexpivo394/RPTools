﻿//Command running revit application

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LotkiColorIsp;

[Regeneration(RegenerationOption.Manual)]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        //Task Dialog =>
        var taskDialog = new TaskDialog("Task dialog");
        //Set application =>
        var uiapp = commandData.Application;
        var uidoc = uiapp.ActiveUIDocument;
        var app = uiapp.Application;
        var doc = uidoc.Document;
        var allsoed = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTrayFitting)
            .WhereElementIsNotElementType()
            .ToList();
        var alllotkirealall = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .ToList();
        foreach (var soed in allsoed) alllotkirealall.Add(soed);

        var t0 = new Transaction(doc);

        foreach (var lotok in alllotkirealall)
        {
            t0.Start("Modify color1");
            var color4 = new Color(0, 0, 0);

            var ogs4 = new OverrideGraphicSettings();
            ogs4.SetProjectionLineColor(color4);
            ogs4.SetCutForegroundPatternColor(color4);
            ogs4.SetSurfaceForegroundPatternColor(color4);
            ogs4.SetCutLineColor(color4);

            doc.ActiveView.SetElementOverrides(lotok.Id, ogs4);

            t0.Commit();
        }

        foreach (var lotok in alllotkirealall)
        {
            t0.Start("Modify color2");
            var color1 = new Color(255, 128, 192);

            var ogs1 = new OverrideGraphicSettings();
            ogs1.SetProjectionLineColor(color1);
            ogs1.SetCutForegroundPatternColor(color1);
            ogs1.SetCutLineColor(color1);

            var color2 = new Color(0, 128, 192);
            var ogs2 = new OverrideGraphicSettings();
            ogs2.SetProjectionLineColor(color2);
            ogs2.SetCutForegroundPatternColor(color2);
            ogs2.SetCutLineColor(color2);

            var color3 = new Color(0, 128, 0);
            var ogs3 = new OverrideGraphicSettings();
            ogs3.SetProjectionLineColor(color3);
            ogs3.SetCutForegroundPatternColor(color3);
            ogs3.SetCutLineColor(color3);

            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 1)
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs1);
            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 2)
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs2);
            if (lotok.LookupParameter("ADSK_Исполнение").AsInteger() == 3)
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs3);

            t0.Commit();
        }


        taskDialog.MainContent = "Покрашено!";
        taskDialog.Show();
        return Result.Succeeded;
    }
}