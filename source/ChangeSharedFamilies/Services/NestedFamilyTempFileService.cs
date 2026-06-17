using Autodesk.Revit.DB;

namespace ChangeSharedFamilies.Services;

internal class NestedFamilyTempFileService
{
    private readonly Document _familyDocument;

    public NestedFamilyTempFileService(Document familyDocument)
    {
        _familyDocument = familyDocument;
    }

    public void CreateNonSharedTempFamilyFile(
        Family oldNestedFamily,
        string tempFamilyName,
        string tempFamilyPath)
    {
        Document nestedFamilyDocument = null;

        try
        {
            nestedFamilyDocument = _familyDocument.EditFamily(oldNestedFamily);

            SetOwnerFamilySharedFlag(nestedFamilyDocument, false);

            // Лучше переименовать OwnerFamily до SaveAs, чтобы Revit не спутал
            // временный файл со старым shared-семейством при обратной загрузке.
            TryRenameOwnerFamily(nestedFamilyDocument, tempFamilyName);

            var saveAsOptions = new SaveAsOptions
            {
                OverwriteExistingFile = true
            };

            nestedFamilyDocument.SaveAs(tempFamilyPath, saveAsOptions);
        }
        finally
        {
            if (nestedFamilyDocument != null)
                nestedFamilyDocument.Close(false);
        }
    }

    private static void SetOwnerFamilySharedFlag(Document nestedFamilyDocument, bool shared)
    {
        if (!nestedFamilyDocument.IsFamilyDocument)
        {
            throw new InvalidOperationException(
                "Документ вложенного семейства не является family document.");
        }

        var ownerFamily = nestedFamilyDocument.OwnerFamily;
        var parameter = ownerFamily.get_Parameter(BuiltInParameter.FAMILY_SHARED);

        if (parameter == null)
        {
            throw new InvalidOperationException(
                $"У семейства '{ownerFamily.Name}' не найден параметр FAMILY_SHARED.");
        }

        if (parameter.IsReadOnly)
        {
            throw new InvalidOperationException(
                $"Параметр Shared у семейства '{ownerFamily.Name}' доступен только для чтения.");
        }

        var newValue = shared ? 1 : 0;

        if (parameter.AsInteger() == newValue)
            return;

        using (var transaction = new Transaction(nestedFamilyDocument, "Disable Shared"))
        {
            transaction.Start();
            parameter.Set(newValue);
            transaction.Commit();
        }
    }

    private static void TryRenameOwnerFamily(Document nestedFamilyDocument, string newName)
    {
        try
        {
            using (var transaction = new Transaction(nestedFamilyDocument, "Rename temp family"))
            {
                transaction.Start();

                nestedFamilyDocument.OwnerFamily.Name = newName;

                transaction.Commit();
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"Не удалось переименовать временное семейство в '{newName}'. " +
                $"Без этого Revit может загрузить его как старое shared-семейство. " +
                $"Причина: {exception.Message}",
                exception);
        }
    }
}
