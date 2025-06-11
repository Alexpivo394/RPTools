using Newtonsoft.Json.Linq;

namespace ParamChecker.Models.Filters;

public class FilterConfigModel
{
    public List<BuiltInCategory> SelectedCategories { get; set; } = new();
    public CategoryParameterLogic CategoryParameterLogic { get; set; }
    public FilterParameterLogic ParameterLogic { get; set; }
    public List<ConditionModelBase> Conditions { get; set; } = new();
}

public class ConditionModelBase
{
    public string Type { get; set; } = "";
}

public class SimpleConditionModel : ConditionModelBase
{
    public string ParameterName { get; set; } = "";
    public string Value { get; set; } = "";
    public FilterLogic SelectedLogic { get; set; }
}

public class GroupConditionModel : ConditionModelBase
{
    public List<ConditionModelBase> Children { get; set; } = new();
}

public class FilterConfigRaw
{
    public List<BuiltInCategory> SelectedCategories { get; set; }
    public CategoryParameterLogic CategoryParameterLogic { get; set; }
    public FilterParameterLogic ParameterLogic { get; set; }
    public JArray Conditions { get; set; }
}