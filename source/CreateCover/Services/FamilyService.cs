using System.Diagnostics;
using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using RPToolsUI.Models;
using RPToolsUI.Services;

namespace CreateCover.Services;

public class FamilyService
{
   
    private Document _doc;
    private string _familyName;
    private string _familyPath;

    public FamilyService(Document doc, string familyName, string familyPath)
    {
        _doc = doc;
        _familyName = familyName;
        _familyPath = familyPath;
    }

    public FamilySymbol? GetOrLoadFamilySymbol()
    {
        try
        {
            // 1. Ищем символ с таким FamilyName
            var existingSymbol = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(f => f.FamilyName.Equals(_familyName, StringComparison.OrdinalIgnoreCase));

            if (existingSymbol != null)
                return existingSymbol;

            // 2. Проверяем путь
            if (!File.Exists(_familyPath))
            {
                var dial = ToadDialogService.Show(
                    "Ошибка!",
                    $"Файл семейства не найден по пути:\n{_familyPath}",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
                return null;
            }

            // 3. Загружаем семейство

            Family? family = null;
            
            using (var t = new Transaction(_doc, "Загрузка семейства"))
            {
                t.Start();

                if (!_doc.LoadFamily(_familyPath, out family))
                {
                    var dial = ToadDialogService.Show(
                        "Ошибка!",
                        $"Не удалось загрузить семейство {_familyPath}",
                        DialogButtons.OK,
                        DialogIcon.Error
                    );
                    return null;
                }

                t.Commit();
            }

            // 4. Получаем все типы в семействе
            var symbols = family.GetFamilySymbolIds()
                .Select(id => _doc.GetElement(id) as FamilySymbol)
                .Where(s => s != null)
                .ToList();

            if (symbols.Count == 0)
            {
                var dial = ToadDialogService.Show(
                    "Ошибка!",
                    $"В семействе '{_familyName}' нет типов!",
                    DialogButtons.OK,
                    DialogIcon.Error
                );
                return null;
            }

            // 5. Берём первый символ (или ищем по имени типа, если нужно)
            var loadedSymbol = symbols.First();

            // 6. Активируем, если неактивный
            if (loadedSymbol != null && !loadedSymbol.IsActive)
            {
                using (var t = new Transaction(_doc, "Активация типа"))
                {
                    t.Start();
                    loadedSymbol.Activate();
                    t.Commit();
                }
            }

            return loadedSymbol;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return null;
        }
    }
}