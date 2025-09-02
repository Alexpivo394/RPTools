//Command running revit application

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace LotkiColorKrshka;

[Regeneration(RegenerationOption.Manual)]
[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
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

        using (var t = new Transaction(doc, "Покраска"))
        {
            t.Start();

        foreach (var a in alllotkirealall)
        {
            var color4 = new Color(0, 0, 0);

            var ogs4 = new OverrideGraphicSettings();
            ogs4.SetProjectionLineColor(color4);
            ogs4.SetCutForegroundPatternColor(color4);
            ogs4.SetSurfaceForegroundPatternColor(color4);
            ogs4.SetCutLineColor(color4);

            doc.ActiveView.SetElementOverrides(a.Id, ogs4);
            
        }

        foreach (var lotok in alllotkirealall)
        {
            var color1 = new Color(255, 0, 0);

            var ogs1 = new OverrideGraphicSettings();
            ogs1.SetProjectionLineColor(color1);
            ogs1.SetCutForegroundPatternColor(color1);
            ogs1.SetCutLineColor(color1);

            var paramName = "ADSK_Крышка";
            var param = lotok.LookupParameter(paramName);
            
            if (param is not null && param.AsInteger() == 1)
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs1);

        }
        
        t.Commit();
        }

        var dial = ToadDialogService.Show(
            "Успех!",
            $"Лотки покрашены.",
            DialogButtons.OK,
            DialogIcon.Info
        );
        return Result.Succeeded;
    }
}