using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace WarmSync;

[Transaction(TransactionMode.Manual)]
public class WriteSpaceIdToParam : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIDocument uiDoc = commandData.Application.ActiveUIDocument;
        Document doc = uiDoc.Document;

        // Имя параметра, куда пишем ID
        const string targetParamName = "ID элемента";
        
        

        try
        {
            // Собираем все пространства
            var spaces = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_MEPSpaces)
                .WhereElementIsNotElementType()
                .ToElements();

            if (!spaces.Any())
            {
                var dial = ToadDialogService.Show(
                    "Запись ID",
                    "Не найдено ни одного пространства.",
                    DialogButtons.OK,
                    DialogIcon.Info
                );
                return Result.Succeeded;
            }

            using (Transaction t = new Transaction(doc, "Запись ID в пространства"))
            {
                t.Start();

                int updated = 0;

                foreach (var space in spaces)
                {
                    var param = space.LookupParameter(targetParamName);
                    if (param != null && !param.IsReadOnly)
                    {
                        param.Set(space.Id.IntegerValue.ToString());
                        updated++;
                    }
                }

                t.Commit();
                var dial = ToadDialogService.Show(
                    "Успех!",
                    $"Обновлено пространств: {updated}",
                    DialogButtons.OK,
                    DialogIcon.Info
                );
            }

            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            var dial = ToadDialogService.Show(
                "Ошибка",
                "Ошибка при записи ID пространств",
                DialogButtons.OK,
                DialogIcon.Error
            );
            message = ex.Message;
            return Result.Failed;
        }
    }
}