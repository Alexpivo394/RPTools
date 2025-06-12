using ParamChecker.Models.Filters;

namespace ParamChecker.Models;

public class ParamCheckerModel
{
    public static List<Element> FilterElementsFromConfig(Document doc, string viewName, FilterConfigModel config)
    {
        // 🔎 Находим нужный вид
        var view = new FilteredElementCollector(doc).OfClass(typeof(View))
            .Cast<View>()
            .FirstOrDefault(v => v.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase));

        if (view == null) throw new Exception($"Вид с именем '{viewName}' не найден.");

        // 🧱 Начинаем с отбора по категориям
        IEnumerable<Element> elements = new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType()
            .Where(e =>
            {
                bool categoryMatch = config.SelectedCategories.Contains((BuiltInCategory)e.Category.Id.IntegerValue);

                return config.CategoryParameterLogic switch
                {
                    CategoryParameterLogic.CategoriesOnly => categoryMatch,
                    CategoryParameterLogic.CategoriesAndParameters => categoryMatch, // параметр проверим потом
                    CategoryParameterLogic.CategoriesOrParameters => categoryMatch, // параметр проверим потом
                    CategoryParameterLogic.ParametersOnly => true, // фильтрация дальше
                    _ => true
                };
            });

        // 📋 Применяем параметрические условия
        var filtered = elements.Where(e =>
        {
            bool paramResult = EvaluateConditions(config.Conditions, config.ParameterLogic, e);

            return config.CategoryParameterLogic switch
            {
                CategoryParameterLogic.CategoriesOnly => true,
                CategoryParameterLogic.ParametersOnly => paramResult,
                CategoryParameterLogic.CategoriesAndParameters => paramResult,
                CategoryParameterLogic.CategoriesOrParameters => paramResult,
                _ => true
            };
        });

        return filtered.ToList();
    }

    private static bool EvaluateConditions(List<ConditionModelBase> conditions, FilterParameterLogic logic, Element e)
    {
        var results = conditions.Select(c => EvaluateCondition(c, e)).ToList();
        return logic == FilterParameterLogic.And ? results.All(r => r) : results.Any(r => r);
    }

    private static bool EvaluateCondition(ConditionModelBase cond, Element e)
    {
        return cond switch
        {
            SimpleConditionModel simple => EvaluateSimpleCondition(simple, e),
            GroupConditionModel group => group.Children.All(child => EvaluateCondition(child, e)),
            _ => false
        };
    }


    private static bool EvaluateSimpleCondition(SimpleConditionModel cond, Element e)
    {
        var param = e.LookupParameter(cond.ParameterName);
        string? val = param?.AsValueString() ?? param?.AsString();

        // Для чисел пробуем парсить
        bool parsedVal = double.TryParse(val, out double number);
        bool parsedCond = double.TryParse(cond.Value, out double targetNumber);

        switch (cond.SelectedLogic)
        {
            case FilterLogic.Equals:
                return string.Equals(val, cond.Value, StringComparison.OrdinalIgnoreCase);
            case FilterLogic.NotEquals:
                return !string.Equals(val, cond.Value, StringComparison.OrdinalIgnoreCase);
            case FilterLogic.Contains:
                return val?.Contains(cond.Value, StringComparison.OrdinalIgnoreCase) ?? false;
            case FilterLogic.NotContains:
                return !(val?.Contains(cond.Value, StringComparison.OrdinalIgnoreCase) ?? false);
            case FilterLogic.Exists:
                return param != null && (!string.IsNullOrWhiteSpace(val) || param.HasValue);
            case FilterLogic.NotExists:
                return param == null || (string.IsNullOrWhiteSpace(val) && !param.HasValue);
            case FilterLogic.GreaterThan:
                return parsedVal && parsedCond && number > targetNumber;
            case FilterLogic.GreaterThanOrEquals:
                return parsedVal && parsedCond && number >= targetNumber;
            case FilterLogic.LessThan:
                return parsedVal && parsedCond && number < targetNumber;
            case FilterLogic.LessThanOrEquals:
                return parsedVal && parsedCond && number <= targetNumber;
            default:
                return false;
        }
    }
}