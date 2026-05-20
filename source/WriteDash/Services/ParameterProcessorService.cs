using System.Diagnostics;
using System.Windows;
using RPToolsUI.Models;
using RPToolsUI.Services;
using WriteDash.Models;

namespace WriteDash.Services;

public class ParameterProcessorService
{
    
    private readonly Document _document;

    public ParameterProcessorService(Document document)
    {
        _document = document;
    }

    public void Process(List<ParameterDescriptor> selectedParameters)
    {
        if (selectedParameters.Count == 0)
        {
            var dial = ToadDialogService.Show(
                "Ошибка!",
                "Выберите параметры для записи!",
                DialogButtons.OK,
                DialogIcon.Error
            );
            
            return;
        }

        var parameterNames = selectedParameters
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet();

        var elements = new FilteredElementCollector(_document)
            .WhereElementIsNotElementType()
            .Where(x =>
                x.Category != null &&
                x.Category.CategoryType == CategoryType.Model);

        using var transaction = new Transaction(_document, "Fill empty parameters");

        transaction.Start();

        foreach (var element in elements)
        {
            foreach (Parameter parameter in element.Parameters)
            {
                var definition = parameter.Definition;

                if (definition == null)
                    continue;

                var parameterName = definition.Name;

                if (string.IsNullOrWhiteSpace(parameterName))
                    continue;

                if (!parameterNames.Contains(parameterName))
                    continue;

                if (parameter.IsReadOnly)
                    continue;

                if (parameter.StorageType != StorageType.String)
                    continue;

                var currentValue = parameter.AsString();

                if (!string.IsNullOrWhiteSpace(currentValue))
                    continue;

                parameter.Set("-");
            }
        }

        transaction.Commit();
        
        var dialDone = ToadDialogService.Show(
            "Успех!",
            "Значение записано в выбранные параметры!",
            DialogButtons.OK,
            DialogIcon.Info
        );
    }
}