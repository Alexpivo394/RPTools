using System.Collections.ObjectModel;
using ParamChecker.Models.Parameters;

namespace ParamChecker.ViewModels.Windows;

public partial class ParameterConfigViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ParameterItem> _parameters = new();

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
    private void ApplyParameter()
    {
        var result = new ParameterConfigResult
        {
            Parameters = Parameters
                .Select(p => p.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList()
        };

        var jsonResult = result.ToJson();
        // Здесь нужно вернуть результат в вызывающее окно
        // Например, через событие или callback
    }

    public string GetResultJson()
    {
        var result = new ParameterConfigResult
        {
            // заполните данные
        };
        return result.ToJson();
    }
}