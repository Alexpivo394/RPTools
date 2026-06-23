using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ChangeSharedFamilies.Services;
using ToadTools.UI.Models;
using ToadTools.UI.Services;

namespace ChangeSharedFamilies.Commands;

[Transaction(TransactionMode.Manual)]
public class StartupCommand : IExternalCommand
{
    public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
    {
        var uiDoc = commandData.Application.ActiveUIDocument;

        if (uiDoc == null)
        {
            message = "Нет активного документа.";
            return Result.Failed;
        }

        var doc = uiDoc.Document;

        if (!doc.IsFamilyDocument)
        {
            ToadDialogService.Show(
                "Ошибка",
                "Эту команду надо запускать в редакторе семейства, а не в проекте.",
                DialogButtons.OK,
                DialogIcon.Error);

            return Result.Cancelled;
        }

        try
        {
            var service = new CurrentFamilyNestedUnshareService(doc);

            var report = service.ConvertSharedNestedFamiliesToNonShared();

            var convertedNames = report.ConvertedFamilyNames.Count == 0
                ? "Ничего не найдено."
                : string.Join("\n", report.ConvertedFamilyNames);

            var errors = report.Errors.Count == 0
                ? ""
                : "\n\nОшибки:\n" + string.Join("\n", report.Errors);

            ToadDialogService.Show(
                "Готово",
                $"Текущее семейство: {doc.Title}\n" +
                $"Заменено shared вложенных семейств: {report.ConvertedFamiliesCount}\n\n" +
                convertedNames +
                errors,
                DialogButtons.OK,
                report.Errors.Any() ? DialogIcon.Warning : DialogIcon.Info);

            return report.Errors.Any()
                ? Result.Failed
                : Result.Succeeded;
        }
        catch (Exception exception)
        {
            message = exception.Message;

            ToadDialogService.Show(
                "Ошибка",
                exception.ToString(),
                DialogButtons.OK,
                DialogIcon.Error);

            return Result.Failed;
        }
    }
}
