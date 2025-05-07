//Command running revit application

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LotkiColorKrshka;

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

        foreach (var a in alllotkirealall)
        {
            t0.Start("Modify color1");
            var color4 = new Color(0, 0, 0);

            var ogs4 = new OverrideGraphicSettings();
            ogs4.SetProjectionLineColor(color4);
            ogs4.SetCutForegroundPatternColor(color4);
            ogs4.SetSurfaceForegroundPatternColor(color4);
            ogs4.SetCutLineColor(color4);

            doc.ActiveView.SetElementOverrides(a.Id, ogs4);

            t0.Commit();
        }

        foreach (var lotok in alllotkirealall)
        {
            t0.Start("Modify color2");
            var color1 = new Color(255, 0, 0);

            var ogs1 = new OverrideGraphicSettings();
            ogs1.SetProjectionLineColor(color1);
            ogs1.SetCutForegroundPatternColor(color1);
            ogs1.SetCutLineColor(color1);

            if (lotok.LookupParameter("ADSK_Крышка").AsInteger() == 1)
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs1);

            t0.Commit();
        }

        taskDialog.MainContent = "Покрашено!";
        taskDialog.Show();
        return Result.Succeeded;
    }
}