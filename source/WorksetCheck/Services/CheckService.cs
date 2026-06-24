namespace WorksetCheck.Services;

public class CheckService
{
    private static readonly Dictionary<string[], string[]> SectionMapping = new()
    {
        { new[] { "AR" }, new[] { "АР", "AR", "AI", "АИ" } },
        { new[] { "KR" }, new[] { "КР", "KR" } },
        { new[] { "OVV", "OVO", "OVK" }, new[] { "OVV", "OVO", "OVK", "ОВ" } },
        { new[] { "VK" }, new[] { "ВК", "VK" } },
        { new[] { "PT" }, new[] { "ПТ", "PT" } },
        { new[] { "SS" }, new[] { "СС", "SS" } },
        { new[] { "EOM" }, new[] { "ЭОМ", "EOM" } },
        { new[] { "AVT" }, new[] { "АВТ", "AVT" } },
        { new[] { "ITP" }, new[] { "ИТП", "ITP" } },
    };

    public List<string> CheckWorksets(
        Document? doc,
        string rvtLinksPrefix,
        string gridsAndLevelsWorksetName,
        string openingsWorksetName)
    {
        if (doc == null)
            throw new ArgumentNullException(nameof(doc));

        var errors = new List<string>();

        var allWorksets = new FilteredWorksetCollector(doc)
            .OfKind(WorksetKind.UserWorkset)
            .ToWorksets();

        foreach (var ws in allWorksets)
        {
            var wsName = ws.Name;

            var elementsInWs = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .Where(x => x.WorksetId == ws.Id)
                .ToList();

            // Рабочий набор для связей
            if (wsName.StartsWith(
                    rvtLinksPrefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                foreach (var element in elementsInWs)
                {
                    if (!IsCategory(element, BuiltInCategory.OST_RvtLinks))
                    {
                        errors.Add(
                            $"[WS: {wsName}] Недопустимый элемент (Id {element.Id}) — должны быть только связи");

                        continue;
                    }

                    var linkName = element.Name ?? string.Empty;
                    var matched = false;

                    foreach (var mapping in SectionMapping)
                    {
#if REVIT2025_OR_GREATER
                        var linkMatches = mapping.Key.Any(
                            key => linkName.Contains(
                                key,
                                StringComparison.OrdinalIgnoreCase));

                        var worksetMatches = mapping.Value.Any(
                            value => wsName.Contains(
                                value,
                                StringComparison.OrdinalIgnoreCase));
#else
                        var linkMatches = mapping.Key.Any(
                            key => linkName.IndexOf(
                                key,
                                StringComparison.OrdinalIgnoreCase) >= 0);

                        var worksetMatches = mapping.Value.Any(
                            value => wsName.IndexOf(
                                value,
                                StringComparison.OrdinalIgnoreCase) >= 0);
#endif

                        if (!linkMatches || !worksetMatches)
                            continue;

                        matched = true;
                        break;
                    }

                    if (!matched)
                    {
                        errors.Add(
                            $"[WS: {wsName}] Файл связи '{linkName}' не соответствует разделу рабочего набора");
                    }
                }
            }
            // Рабочий набор для осей и уровней
            else if (string.Equals(
                         wsName,
                         gridsAndLevelsWorksetName,
                         StringComparison.OrdinalIgnoreCase))
            {
                foreach (var element in elementsInWs)
                {
                    if (!IsCategory(element, BuiltInCategory.OST_Grids) &&
                        !IsCategory(element, BuiltInCategory.OST_Levels))
                    {
                        errors.Add(
                            $"[WS: {wsName}] Недопустимый элемент (Id {element.Id}) — должны быть только оси или уровни");
                    }
                }
            }
            // Рабочий набор для отверстий
            else if (string.Equals(
                         wsName,
                         openingsWorksetName,
                         StringComparison.OrdinalIgnoreCase))
            {
                foreach (var element in elementsInWs)
                {
                    if (!IsCategory(element, BuiltInCategory.OST_GenericModel))
                    {
                        errors.Add(
                            $"[WS: {wsName}] Недопустимый элемент (Id {element.Id}) — должны быть только Обобщенные модели (отверстия)");
                    }
                }
            }
            // Остальные рабочие наборы
            else
            {
                foreach (var element in elementsInWs)
                {
                    if (IsCategory(element, BuiltInCategory.OST_RvtLinks) ||
                        IsCategory(element, BuiltInCategory.OST_Grids) ||
                        IsCategory(element, BuiltInCategory.OST_Levels))
                    {
                        errors.Add(
                            $"[WS: {wsName}] Недопустимый элемент (Id {element.Id}) — связи/оси/уровни тут быть не должны");
                    }
                }
            }
        }

        return errors;
    }

    private static bool IsCategory(
        Element element,
        BuiltInCategory builtInCategory)
    {
        var categoryId = element.Category?.Id;

        return categoryId != null &&
               GetElementIdValue(categoryId) == (long)builtInCategory;
    }

    private static long GetElementIdValue(ElementId elementId)
    {
#if REVIT2024_OR_GREATER
        return elementId.Value;
#else
        return elementId.IntegerValue;
#endif
    }
}