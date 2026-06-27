using Nice3point.Revit.Toolkit;
using ToadTools.UI.Models;
using ToadTools.UI.Services;

namespace WarmSync;

public class WriteSpaceIdToParam
{
    public void Run()
    {
        var uiDoc = RevitContext.ActiveUiDocument!;
        var doc = uiDoc.Document;

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
                return;
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
#if REVIT2024_OR_GREATER
                        param.Set(space.Id.Value.ToString());
#else
                        param.Set(space.Id.IntegerValue.ToString());
#endif
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
        }
        catch (Exception)
        {
            var dial = ToadDialogService.Show(
                "Ошибка",
                "Ошибка при записи ID пространств",
                DialogButtons.OK,
                DialogIcon.Error
            );
            throw;
        }
    }
}