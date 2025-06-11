namespace ParamChecker.Models.ExportProfiles;

public partial class ExportModel : ObservableObject
{
    [ObservableProperty] private string _serverPath = string.Empty;

    [ObservableProperty] private string _viewName = string.Empty;

    [ObservableProperty] private string _worksetKeyword = string.Empty;
}