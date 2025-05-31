using System.Windows.Documents;
using Newtonsoft.Json;
using ParamChecker.ViewModels.Windows;

namespace ParamChecker.Models.Filters;

public class FilterConfigResult
{
    public List<BuiltInCategory> SelectedCategories { get; set; } = new();
    public List<FilterCondition> Conditions { get; set; } = new();
    public CategoryParameterLogic CategoryParameterLogic { get; set; }
    public FilterLogic ParametersLogic { get; set; }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static FilterConfigResult FromJson(string json)
    {
        return JsonConvert.DeserializeObject<FilterConfigResult>(json) ?? new FilterConfigResult();
    }
}