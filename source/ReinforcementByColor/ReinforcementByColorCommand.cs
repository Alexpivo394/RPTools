using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace ReinforcementByColor;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class ReinforcementByColorCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDoc = commandData.Application.ActiveUIDocument;
        var doc = uiDoc.Document;
        var view = doc.ActiveView;

        var map = new Dictionary<string, string>()
        {
            { "REI_d10_s100", "D10-A500C" },
            { "REI_d10_s125", "D10-A500C" },
            { "REI_d10_s200", "D10-A500C" },
            { "REI_d10_s250", "D10-A500C" },
            { "REI_d12_s100", "D12-A500C" },
            { "REI_d12_s125", "D12-A500C" },
            { "REI_d12_s200", "D12-A500C" },
            { "REI_d12_s250", "D12-A500C" },
            { "REI_d16_s100", "D16-A500C" },
            { "REI_d16_s125", "D16-A500C" },
            { "REI_d16_s200", "D16-A500C" },
            { "REI_d16_s250", "D16-A500C" },
            { "REI_d20_s100", "D20-A500C" },
            { "REI_d20_s125", "D20-A500C" },
            { "REI_d20_s200", "D20-A500C" },
            { "REI_d20_s250", "D20-A500C" },
            { "REI_d25_s100", "D25-A500C" },
            { "REI_d25_s125", "D25-A500C" },
            { "REI_d25_s200", "D25-A500C" },
            { "REI_d25_s250", "D25-A500C" },
            { "REI_d28_s100", "D28-A500C" },
            { "REI_d28_s125", "D28-A500C" },
            { "REI_d28_s200", "D28-A500C" },
            { "REI_d28_s250", "D28-A500C" },
            { "REI_d32_s100", "D32-A500C" },
            { "REI_d32_s125", "D32-A500C" },
            { "REI_d32_s200", "D32-A500C" },
            { "REI_d32_s250", "D32-A500C" },
            { "REI_d36_s100", "D36-A500C" },
            { "REI_d36_s125", "D36-A500C" },
            { "REI_d36_s200", "D36-A500C" },
            { "REI_d36_s250", "D36-A500C" }
        };

        var regions = new FilteredElementCollector(doc, view.Id)
            .OfClass(typeof(FilledRegion))
            .Cast<FilledRegion>()
            .ToList();

        var symbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol))
            .OfCategory(BuiltInCategory.OST_DetailComponents)
            .Cast<FamilySymbol>()
            .ToList();

        int replaced = 0;

        using (var t = new Transaction(doc, "Replace FilledRegions"))
        {
            t.Start();

            foreach (var region in regions)
            {
                var type = doc.GetElement(region.GetTypeId());
                string regionName = type.Name;

                if (!map.TryGetValue(regionName, out string targetName)) continue;

                var targetFamilyName = "R-SHP-01 - Дополнительная";

                var symbol = symbols
                    .FirstOrDefault(x =>
                        x.Family.Name.Equals(targetFamilyName, StringComparison.OrdinalIgnoreCase) &&
                        x.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase));

                if (symbol == null) continue;

                if (!symbol.IsActive) symbol.Activate();
                
                // start
                var loop = region.GetBoundaries().FirstOrDefault();
                if (loop == null)
                    continue;

                var lines = loop.OfType<Line>()
                    .Where(l => l.Length > 0.001)
                    .ToList();

                if (lines.Count < 4)
                    continue;
                
                var verticalLines = lines
                    .Where(l => Math.Abs(l.Direction.X) < 0.001)
                    .ToList();

                var horizontalLines = lines
                    .Where(l => Math.Abs(l.Direction.Y) < 0.001)
                    .ToList();

                if (!verticalLines.Any() || !horizontalLines.Any())
                    continue;
                
                var leftLine = verticalLines
                    .OrderBy(l => l.GetEndPoint(0).X)
                    .First();
                
                var p1 = leftLine.GetEndPoint(0);
                var p2 = leftLine.GetEndPoint(1);
                
                if (p1.Y < p2.Y)
                {
                    (p1, p2) = (p2, p1);
                }

                var verticalLine = Line.CreateBound(p1, p2);
                
                var horizontal = horizontalLines
                    .OrderByDescending(l => l.Length)
                    .First();

                double length = horizontal.Length;
                
                var instance = doc.Create.NewFamilyInstance(
                    verticalLine,
                    symbol,
                    view);
                
                var param = instance.LookupParameter("Длина стержней");

                if (param != null && !param.IsReadOnly)
                {
                    param.Set(length);
                }
                // end

                doc.Delete(region.Id);

                replaced++;
            }

            t.Commit();
        }
        
        
        var dial = ToadDialogService.Show(
            "Успех!",
            $"Заменено: {replaced}",
            DialogButtons.OK,
            DialogIcon.Info
        );

        return Result.Succeeded;
    }
    
}