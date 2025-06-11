using Newtonsoft.Json;

namespace ParamChecker.Models.Parameters;

public partial class ParameterItem : ObservableObject
{
    [ObservableProperty] private string _value = string.Empty;
}

public class ParameterConfigResult
{
    public List<string> Parameters { get; set; } = new();

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static ParameterConfigResult FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ParameterConfigResult>(json) ?? new ParameterConfigResult();
    }
}