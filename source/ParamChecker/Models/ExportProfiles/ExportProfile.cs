using Newtonsoft.Json;

namespace ParamChecker.Models.ExportProfiles;

public class ExportProfile
{
    public string ProfileName { get; set; }
    public List<ExportModel> Models { get; set; } = new();
    public List<ExportRule> Rules { get; set; } = new();
    
}