namespace WorksetCheck.Services;

public class CheckService
{
    private static readonly Dictionary<string[], string[]> SectionMapping = new()
    {
        { new[] { "AR" }, new[] { "АР", "AR" , "AI", "АИ"} },
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
            var errors = new List<string>();

            // 1. Берем список всех рабочих наборов
            var worksetTable = doc?.GetWorksetTable();
var allWorksets = new FilteredWorksetCollector(doc)         
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksets();

            // 2. Идем по рабочим наборам
            foreach (var ws in allWorksets)
            {
                string wsName = ws.Name;

                // Собираем все элементы из этого рабочего набора
                var elementsInWs = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Where(x => x.WorksetId == ws.Id)
                    .ToList();

                // Если это рабочий набор для связей
                if (wsName.StartsWith(rvtLinksPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var el in elementsInWs)
                    {
                        if (el.Category?.Id.IntegerValue != (int)BuiltInCategory.OST_RvtLinks)
                        {
                            errors.Add($"[WS: {wsName}] Недопустимый элемент (Id {el.Id}) — должны быть только связи");
                            continue;
                        }

                        var linkName = el.Name ?? string.Empty;

                        // Проверяем соответствие имени файла и имени РН
                        bool matched = false;

                        foreach (var mapping in SectionMapping)
                        {
                            if (mapping.Key.Any(k => linkName.Contains(k, StringComparison.OrdinalIgnoreCase)))
                            {
                                if (mapping.Value.Any(v => wsName.Contains(v, StringComparison.OrdinalIgnoreCase)))
                                {
                                    matched = true;
                                    break;
                                }
                            }
                        }

                        if (!matched)
                        {
                            errors.Add($"[WS: {wsName}] Файл связи '{linkName}' не соответствует разделу рабочего набора");
                        }
                    }
                }
                // Если это рабочий набор для осей и уровней
                else if (string.Equals(wsName, gridsAndLevelsWorksetName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var el in elementsInWs)
                    {
                        if (el.Category?.Id.IntegerValue != (int)BuiltInCategory.OST_Grids &&
                            el.Category?.Id.IntegerValue != (int)BuiltInCategory.OST_Levels)
                        {
                            errors.Add($"[WS: {wsName}] Недопустимый элемент (Id {el.Id}) — должны быть только оси или уровни");
                        }
                    }
                }
                // Если это рабочий набор для отверстий
                else if (string.Equals(wsName, openingsWorksetName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var el in elementsInWs)
                    {
                        if (el.Category?.Id.IntegerValue != (int)BuiltInCategory.OST_GenericModel)
                        {
                            errors.Add($"[WS: {wsName}] Недопустимый элемент (Id {el.Id}) — должны быть только Обобщенные модели (отверстия)");
                        }
                    }
                }
                // Все остальные рабочие наборы
                else
                {
                    foreach (var el in elementsInWs)
                    {
                        if (el.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_RvtLinks ||
                            el.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_Grids ||
                            el.Category?.Id.IntegerValue == (int)BuiltInCategory.OST_Levels)
                        {
                            errors.Add($"[WS: {wsName}] Недопустимый элемент (Id {el.Id}) — связи/оси/уровни тут быть не должны");
                        }
                    }
                }
            }

            return errors;
        }
}