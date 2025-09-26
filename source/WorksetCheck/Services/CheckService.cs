namespace WorksetCheck.Services;

public class CheckService
{
    public List<string> CheckWorksets(
            Document doc,
            string rvtLinksPrefix,
            string gridsAndLevelsWorksetName,
            string openingsWorksetName)
        {
            var errors = new List<string>();

            // 1. Берем список всех рабочих наборов
            var worksetTable = doc.GetWorksetTable();
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