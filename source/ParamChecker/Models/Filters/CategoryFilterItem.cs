#nullable enable
namespace ParamChecker.Models.Filters;

public sealed partial class CategoryFilterItem : ObservableObject
{
    [ObservableProperty]
    private string? _catName;
    
    [ObservableProperty]
    private bool _catIsSelected;
    
    public BuiltInCategory BuiltInCategory { get; set; }
    
}