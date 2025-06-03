#nullable enable
using System.ComponentModel;

namespace ParamChecker.Models.Filters;

public sealed class CategoryFilterItem : INotifyPropertyChanged
{
    private string? _catName;
    private bool _catIsSelected;

    public string? CatName
    {
        get => _catName;
        set => SetField(ref _catName, value);
    }

    public bool CatIsSelected
    {
        get => _catIsSelected;
        set => SetField(ref _catIsSelected, value);
    }

    public BuiltInCategory BuiltInCategory { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}