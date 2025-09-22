namespace WorksetCheck.Models;

public partial class ExportModel : ObservableObject
{
    [ObservableProperty] private string _serverPath = string.Empty;

    [ObservableProperty] private string _worksetForWorkSetsName = string.Empty;

    [ObservableProperty] private string _worksetForAxesName = string.Empty;
    
    [ObservableProperty] private string _worksetForHolesName = string.Empty;
}