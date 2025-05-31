using System.ComponentModel;

namespace ParamChecker.Models.Filters;

public class ParameterCondition : INotifyPropertyChanged
{
    private string _parameterName = "";
    private string _parameterValue = "";

    public string ParameterName
    {
        get => _parameterName;
        set => SetField(ref _parameterName, value);
    }

    public string ParameterValue
    {
        get => _parameterValue;
        set => SetField(ref _parameterValue, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}