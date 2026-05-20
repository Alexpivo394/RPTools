namespace WriteDash.Models;

public partial class ParameterDescriptor : ObservableObject
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private bool _isSelected;

    public override string? ToString() => Name;
}