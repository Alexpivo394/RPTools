#nullable enable
namespace ParamChecker.Models.Filters;

public sealed partial class CategoryFilterItem : ObservableObject
{
    [ObservableProperty] private bool _catIsSelected;

    [ObservableProperty] private string? _catName;

    public BuiltInCategory BuiltInCategory { get; set; }
}