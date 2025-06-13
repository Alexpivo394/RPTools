#nullable enable
using System.Collections.ObjectModel;
using ParamChecker.Models.Parameters;

namespace ParamChecker.ViewModels.Windows;

public partial class ParameterConfigViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ParameterItem> _parameters = new();
    
    public Action<string>? OnApplyRequested { get; set; }

    [RelayCommand]
    private void AddParameter()
    {
        Parameters.Add(new ParameterItem());
    }

    [RelayCommand]
    private void RemoveParameter(ParameterItem parameter)
    {
        Parameters.Remove(parameter);
    }
    
    [RelayCommand]
    private void Apply()
    {
        var json = GetResultJson();
        OnApplyRequested?.Invoke(json);
    }

    public string GetResultJson()
    {
        var result = new ParameterConfigResult
        {
            Parameters = Parameters
        };
        return result.ToJson();
    }
    
    public void LoadFromJson(string json)
    {
        var parsed = ParameterConfigResult.FromJson(json);
        Parameters.Clear();
        foreach (var param in parsed.Parameters)
        {
            Parameters.Add(param);
        }
    }

}