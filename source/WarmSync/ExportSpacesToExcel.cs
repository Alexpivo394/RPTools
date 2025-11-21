#nullable enable
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using OfficeOpenXml;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace WarmSync;

[Transaction(TransactionMode.Manual)]
public class ExportSpacesToExcel : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Document doc = uidoc.Document;

        var dialog = new SaveFileDialog
        {
            Title = "Сохранить Excel файл",
            Filter = "Excel files (*.xlsx)|*.xlsx",
            DefaultExt = "xlsx",
            AddExtension = true,
            FileName = "spaces_export.xlsx"
        };
        
        string excelPath = "";
        
        if (dialog.ShowDialog() == true) excelPath = dialog.FileName;

        ExcelPackage.License.SetNonCommercialPersonal("RPTools");

        try
        {
            List<Space> spaces = GetSpaces(doc);

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Spaces");

                // Заголовки
                ws.Cells[1, 1].Value = "ID элемента";
                ws.Cells[1, 2].Value = "Уровень";
                ws.Cells[1, 3].Value = "Номер";
                ws.Cells[1, 4].Value = "Имя";
                ws.Cells[1, 5].Value = "Площадь";
                ws.Cells[1, 6].Value = "Объем";
                ws.Cells[1, 7].Value = "ADSK_Температура в помещении";
                ws.Cells[1, 8].Value = "ADSK_Теплопотери";

                int row = 2;

                foreach (var sp in spaces)
                {
                    Debug.WriteLine($"Обрабатываю Space ID {sp.Id}");

                    ws.Cells[row, 1].Value = sp.Id.IntegerValue;
                    ws.Cells[row, 2].Value = sp.Level?.Name ?? "";
                    ws.Cells[row, 3].Value = sp.Number ?? "";
                    ws.Cells[row, 4].Value = sp.Name ?? "";
                    {
                        var p = sp.get_Parameter(BuiltInParameter.ROOM_AREA);
                        ws.Cells[row, 5].Value = p != null ? p.AsValueString() : "";
                    }
                    {
                        var p = sp.get_Parameter(BuiltInParameter.ROOM_VOLUME);
                        ws.Cells[row, 6].Value = p != null ? p.AsValueString() : "";
                    }

                    ws.Cells[row, 7].Value = GetParamValue(sp, "ADSK_Температура в помещении");
                    ws.Cells[row, 8].Value = GetParamValue(sp, "ADSK_Теплопотери");

                    row++;
                }

                // автоширина, чтобы таблица не выглядела как жопа
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                FileInfo fi = new FileInfo(excelPath);
                package.SaveAs(fi);
            }
            
            var dial = ToadDialogService.Show(
                "Успех!",
                $"Выгружено пространств: {GetSpaces(doc).Count}\nФайл:\n{excelPath}",
                DialogButtons.OK,
                DialogIcon.Info
            );

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            var dial = ToadDialogService.Show(
                "Ошибка",
                ex.ToString(),
                DialogButtons.OK,
                DialogIcon.Error
            );
            return Result.Failed;
        }
    }

    private List<Space> GetSpaces(Document doc)
    {
        var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MEPSpaces)
            .WhereElementIsNotElementType();

        var res = new List<Space>();

        foreach (var e in col)
            if (e is Space sp)
                res.Add(sp);

        return res;
    }

    private object? GetParamValue(Element el, string paramName)
    {
        Parameter? p = el.LookupParameter(paramName);
        if (p == null) return null;

        return p.StorageType switch
        {
            StorageType.Double => p.AsDouble(),
            StorageType.Integer => p.AsInteger(),
            StorageType.String => p.AsString(),
            StorageType.ElementId => p.AsElementId()?.IntegerValue,
            _ => null
        };
    }
}