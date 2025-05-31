using Newtonsoft.Json;

namespace ParamChecker.Models.ExportProfiles;

public class ExportProfile
{
    public List<ExportModel> Models { get; set; } = new();
    public List<ExportRule> Rules { get; set; } = new();

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static ExportProfile FromJson(string json)
    {
        return JsonConvert.DeserializeObject<ExportProfile>(json) ?? new ExportProfile();
    }
}