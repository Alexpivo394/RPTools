using Autodesk.Revit.DB;
using System.IO;

namespace ChangeSharedFamilies.Services;

internal class NestedFamilyLoader
{
    private readonly Document _familyDocument;

    public NestedFamilyLoader(Document familyDocument)
    {
        _familyDocument = familyDocument;
    }

    public Family LoadTempFamily(
        string tempFamilyPath,
        string tempFamilyName)
    {
        if (!File.Exists(tempFamilyPath))
        {
            throw new FileNotFoundException(
                $"Временный файл семейства не найден: {tempFamilyPath}");
        }

        Family loadedFamily;

        using (var transaction = new Transaction(
                   _familyDocument,
                   $"Load temp nested family {tempFamilyName}"))
        {
            transaction.Start();

            var loaded = _familyDocument.LoadFamily(
                tempFamilyPath,
                new ForceFamilyLoadOptions(),
                out loadedFamily);

            if (!loaded || loadedFamily == null)
            {
                transaction.RollBack();

                throw new InvalidOperationException(
                    $"Не удалось загрузить временное семейство '{tempFamilyName}'. " +
                    $"Файл существует, но Revit вернул false. " +
                    $"Путь: {tempFamilyPath}");
            }

            transaction.Commit();
        }

        return loadedFamily;
    }
}
