namespace ParamChecker.Models.ExportProfiles;

public partial class ExportRule : ObservableObject
{
    [ObservableProperty] private string _filterConfigJson = string.Empty;

    [ObservableProperty] private string _parameterConfigJson = string.Empty;
}