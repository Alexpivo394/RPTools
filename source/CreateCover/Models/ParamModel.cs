using System.Collections.ObjectModel;
using CreateCover.Models;

namespace CreateCover.Models;

public partial class ParamModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ParameterDescriptor>? _trayParams = new();
    
    [ObservableProperty] private ObservableCollection<ParameterDescriptor>? _coverParams = new();
    
    [ObservableProperty] private ParameterDescriptor? _selectedTrayParam;
    
    [ObservableProperty] private ParameterDescriptor? _selectedCoverParam;
}