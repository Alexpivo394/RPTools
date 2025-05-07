//Command running revit application

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace LotkiColor;

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
        var alllotki = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .Where(lotok =>
                doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() !=
                "Лоток лестничный")
            .ToList();

        var alllotkirealall = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CableTray)
            .WhereElementIsNotElementType()
            .ToList();

        var t0 = new Transaction(doc);

        var color3 = new Color(0, 0, 0);
        var ogs3 = new OverrideGraphicSettings();
        ogs3.SetProjectionLineColor(color3);
        ogs3.SetCutForegroundPatternColor(color3);
        ogs3.SetSurfaceForegroundPatternColor(color3);
        ogs3.SetCutLineColor(color3);


        foreach (var lotok in alllotkirealall)
        {
            t0.Start("Modify color1");

            doc.ActiveView.SetElementOverrides(lotok.Id, ogs3);

            t0.Commit();
        }

        foreach (var soed in allsoed)
        {
            t0.Start("Modify color1");

            doc.ActiveView.SetElementOverrides(soed.Id, ogs3);

            t0.Commit();
        }

        foreach (var lotok in alllotkirealall)
        {
            t0.Start("Modify color2");

            var color1 = new Color(0, 174, 152);
            var ogs1 = new OverrideGraphicSettings();
            ogs1.SetProjectionLineColor(color1);
            ogs1.SetCutForegroundPatternColor(color1);
            ogs1.SetCutLineColor(color1);

            var drugoycvet = new Color(152, 76, 152);
            var ogs2 = new OverrideGraphicSettings();
            ogs2.SetProjectionLineColor(drugoycvet);
            ogs2.SetCutForegroundPatternColor(drugoycvet);
            ogs2.SetCutLineColor(drugoycvet);

            var blin = new Color(255, 255, 0);
            var ogsBLIN = new OverrideGraphicSettings();
            ogsBLIN.SetProjectionLineColor(blin);
            ogsBLIN.SetCutForegroundPatternColor(blin);
            ogsBLIN.SetCutLineColor(blin);

            if (doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток перфорированный")
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs1);

            if (doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток неперфорированный")
                doc.ActiveView.SetElementOverrides(lotok.Id, ogs2);

            if (doc.GetElement(lotok.GetTypeId()).LookupParameter("ADSK_Наименование (по типу)").AsString() ==
                "Лоток проволочный")
                doc.ActiveView.SetElementOverrides(lotok.Id, ogsBLIN);

            t0.Commit();
        }


        taskDialog.MainContent = "Покрашено!";
        taskDialog.Show();
        return Result.Succeeded;
    }
}