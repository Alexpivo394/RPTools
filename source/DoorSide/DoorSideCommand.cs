using System.Diagnostics;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace DoorSide;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class DoorSideCommand : IExternalCommand
{
    private const string SwingParameterName = "Открывание";
    private static readonly Guid SwingParameterGuid = new Guid("0F098FD7-5959-4FF6-AE0D-44E9CC4B7237");

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDoc = commandData.Application.ActiveUIDocument;
        var doc = uiDoc.Document;

        try
        {
            EnsureSwingSharedParameterExists(doc);

            var doors = GetDoorsInActiveView(doc);

            foreach (var door in doors)
            {
                var swing = DetectSwing(door);
                SetSwingParameter(doc, door, swing);
            }

            return Result.Succeeded;
        }
        catch (OperationCanceledException)
        {
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            var dial = ToadDialogService.Show(
                "Ошибка!",
                $"Exception {ex.Message}\n{ex.StackTrace}",
                DialogButtons.OK,
                DialogIcon.Error
            );
            
            return Result.Failed;
        }
        finally
        {
            var dial = ToadDialogService.Show(
                "Успех!",
                $"Параметр открывание заполнен",
                DialogButtons.OK,
                DialogIcon.Info
            );
        }
    }

    private static void EnsureSwingSharedParameterExists(Document doc)
    {
        var spManager = new SharedParametersManager(doc);

        if (spManager.DoesParameterExist(SwingParameterName)) return;

        var categories = new List<BuiltInCategory> { BuiltInCategory.OST_Doors };

        spManager.CreateSharedParameter(
            parameterName: SwingParameterName,
            parameterTypeId: SpecTypeId.String.Text,
            categories: categories,
            parameterGroup: BuiltInParameterGroup.PG_DATA,
            isInstance: true,
            isUserModifiable: true,
            isVisible: true,
            createTempFile: true,
            guid: SwingParameterGuid
        );

    }

    private static IList<FamilyInstance> GetDoorsInActiveView(Document doc)
    {
        return new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Doors)
            .WhereElementIsNotElementType()
            .OfType<FamilyInstance>()
            .ToList();
    }

    private static string DetectSwing(FamilyInstance door)
    {
        bool handFlipped = door.HandFlipped;
        bool facingFlipped = door.FacingFlipped;

        /*
         Логика:
         HandFlipped | FacingFlipped | Результат
         false       | false         | Левая
         true        | false         | Правая
         false       | true          | Правая
         true        | true          | Левая
        */

        if (!handFlipped && !facingFlipped) return "л";

        if (handFlipped && !facingFlipped) return "";

        if (!handFlipped && facingFlipped) return "";

        if (handFlipped && facingFlipped) return "л";

        return "N/A";
    }

    private static void SetSwingParameter(Document doc, FamilyInstance door, string value)
    {
        var param = door.LookupParameter(SwingParameterName);

        if (param == null || param.IsReadOnly) return;

        using (var tx = new Transaction(doc, "Set door swing direction"))
        {
            tx.Start();
            param.Set(value);
            tx.Commit();
        }
    }
}